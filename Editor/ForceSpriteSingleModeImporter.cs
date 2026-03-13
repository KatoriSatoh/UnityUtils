#if UNITY_EDITOR

using UnityEditor;

namespace UnityUtils
{
    public class ForceSpriteSingleModeImporter : AssetPostprocessor
    {
        private const int kPostProcessOrder = 0;
        public override int GetPostprocessOrder() => kPostProcessOrder;

        private void OnPreprocessTexture()
        {
            var importer = assetImporter as TextureImporter;
            if (importer.importSettingsMissing is false) return;

            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
        }
    }
}

#endif