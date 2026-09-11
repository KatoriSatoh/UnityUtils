using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityUtils
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Data/Audio Database", order = 0)]
    public class AudioDatabaseSO : ScriptableObject
    {
        [FolderPath(RequireExistingPath = true), SerializeField] private string audioFolderPath;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private List<AudioData> audioDatas = new();

        public AudioMixer Mixer => audioMixer;
        public IReadOnlyList<AudioData> AudioDatas => audioDatas;

#if UNITY_EDITOR

        [Button("Auto Import")]
        private void Reset()
        {
            if (audioMixer == null)
            {
                var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(GetType().Assembly);
                string root = packageInfo != null ? packageInfo.assetPath : "Assets/Utils";
                var mixer = AssetDatabase.LoadAssetAtPath<Texture2D>($"{root}/Editor/DILite/icon_inject.png");
                audioMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>($"{root}/Runtime/AudioSystem/DefaultMixer.mixer");
            }

            if (string.IsNullOrEmpty(audioFolderPath) || !Directory.Exists(audioFolderPath))
            {
                Logger.LogError<AudioDatabaseSO>("Audio folder path is invalid. Please set a valid path.");
                return;
            }

            var audioFiles = Directory.GetFiles(audioFolderPath, "*.*", SearchOption.AllDirectories);
            foreach (var audioFile in audioFiles)
            {
                var extension = Path.GetExtension(audioFile);
                if (extension != ".wav" && extension != ".mp3" && extension != ".ogg") continue;

                var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioFile);
                if (audioClip == null) continue;

                string name = audioClip.name.ToEnumEntry();
                int id = name.GetHashCode();
                if (audioDatas.Any(data => data.id == id))
                {
                    continue;
                }

                audioDatas.Add(new AudioData
                {
                    clip = audioClip,
                    id = id,
                    name = name,
                    group = name.StartsWith(AudioDefaultGroup.BGM.ToString()) ?
                            audioMixer.FindMatchingGroups(AudioDefaultGroup.BGM.ToString())[0] :
                            audioMixer.FindMatchingGroups(AudioDefaultGroup.SFX.ToString())[0],
                    loop = name.StartsWith(AudioDefaultGroup.BGM.ToString()),
                });
            }

            for (int i = 0; i < audioDatas.Count; i++)
            {
                if (audioDatas[i].clip == null)
                {
                    audioDatas.RemoveAt(i);
                    i--;
                }
            }

            audioDatas.Sort((a, b) => string.Compare(a.clip.name.ToEnumEntry(), b.clip.name.ToEnumEntry()));
            Logger.LogSuccess<AudioDatabaseSO>("Auto Import is done.");
        }

        [Button("Export Audio Enum")]
        private void GenerateAudioEnum(string fileName = "AudioEnum")
        {
            string soFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            string filePath = Path.Combine(soFolder, fileName + ".cs");
            string nameSpace = ResolveNamespace(soFolder);

            StringBuilder enumBuilder = new();
            string indent = string.Empty;
            if (!string.IsNullOrEmpty(nameSpace))
            {
                enumBuilder.AppendLine($"namespace {nameSpace}");
                enumBuilder.AppendLine("{");
                indent = "    ";
            }

            enumBuilder.AppendLine($"{indent}public enum {fileName}");
            enumBuilder.AppendLine($"{indent}{{");

            foreach (var audioData in audioDatas)
            {
                if (audioData.clip != null)
                {
                    string enumEntry = audioData.clip.name.ToEnumEntry();
                    enumBuilder.AppendLine($"{indent}    {enumEntry},");
                }
            }

            enumBuilder.AppendLine($"{indent}}}");

            if (!string.IsNullOrEmpty(nameSpace))
            {
                enumBuilder.AppendLine("}");
            }

            File.WriteAllText(filePath, enumBuilder.ToString());
            AssetDatabase.Refresh();

            Logger.LogSuccess<AudioDatabaseSO>("Audio enum generated at: {0}", filePath);
        }

        /// <summary>
        /// Resolves the namespace the generated file should live in, using the root namespace of the
        /// closest assembly definition above <paramref name="folder"/>, then the project wide one.
        /// </summary>
        private static string ResolveNamespace(string folder)
        {
            var directory = folder?.Replace('\\', '/');
            while (!string.IsNullOrEmpty(directory) && directory.StartsWith("Assets"))
            {
                var asmdef = Directory.GetFiles(directory, "*.asmdef", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (asmdef != null)
                {
                    var definition = JsonUtility.FromJson<AssemblyDefinition>(File.ReadAllText(asmdef));
                    return string.IsNullOrEmpty(definition.rootNamespace) ? definition.name : definition.rootNamespace;
                }

                directory = Path.GetDirectoryName(directory)?.Replace('\\', '/');
            }

            return EditorSettings.projectGenerationRootNamespace;
        }

        [System.Serializable]
        private class AssemblyDefinition
        {
            public string name;
            public string rootNamespace;
        }

#endif
    }
}