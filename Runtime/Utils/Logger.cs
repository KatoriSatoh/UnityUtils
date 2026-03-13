using System;
using UnityEngine;

namespace UnityUtils
{
    public class Logger
    {
        private const string colorCategory = nameof(Color.gray);
        private const string colorSuccess = nameof(Color.cyan);
        private const string colorWarning = nameof(Color.yellow);
        private const string colorError = nameof(Color.red);

        public static void Log<T>(string message, params object[] args)
            => Log(typeof(T).Name, message, args);

        public static void Log(string category, string message, params object[] args)
        {
            Debug.Log($"{Category(category)} {MessageWithFormat(message, "", args)}");
        }

        public static void Log(string message, params object[] args)
        {
            Debug.Log(MessageWithFormat(message, "", args));
        }

        public static void LogSuccess<T>(string message, params object[] args)
            => LogSuccess(typeof(T).Name, message, args);

        public static void LogSuccess(string category, string message, params object[] args)
        {
            Debug.Log($"{Category(category)} {MessageWithFormat(message, colorSuccess, args)}");
        }

        public static void LogSuccess(string message, params object[] args)
        {
            Debug.Log(MessageWithFormat(message, colorSuccess, args));
        }

        public static void LogWarning<T>(string message, params object[] args)
            => LogWarning(typeof(T).Name, message, args);

        public static void LogWarning(string category, string message, params object[] args)
        {
            Debug.Log($"{Category(category)} {MessageWithFormat(message, colorWarning, args)}");
        }

        public static void LogWarning(string message, params object[] args)
        {
            Debug.Log(MessageWithFormat(message, colorWarning, args));
        }

        public static void LogError<T>(string message, params object[] args)
            => LogError(typeof(T).Name, message, args);

        public static void LogError(string category, string message, params object[] args)
        {
            Debug.Log($"{Category(category)} {MessageWithFormat(message, colorError, args)}");
        }

        public static void LogError(string message, params object[] args)
        {
            Debug.Log(MessageWithFormat(message, colorError, args));
        }

        private static string Category(string category)
        {
            return Application.isEditor ? $"<color={colorCategory}>[{category}]</color>" : $"[{category}]";
        }

        private static string MessageWithFormat(string message, string color, params object[] args)
        {
            if (!Application.isEditor || string.IsNullOrEmpty(color)) return string.Format(message, args);

            string[] argsString = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                argsString[i] = Application.isEditor ? $"<color={color}>{args[i]}</color>" : args[i].ToString();
            }

            if (args.Length == 0) message = Application.isEditor ? $"<color={color}>{message}</color>" : message;
            return string.Format(message, argsString);
        }
    }
}