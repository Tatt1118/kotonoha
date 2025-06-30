using UnityEngine;

namespace Utility
{
    public class DebugUtility
    {
        public static bool EnableLog = true;

        public static void Log<T>(T message)
        {
            if (EnableLog) Debug.Log(message);
        }
        public static void LogWarning<T>(T message)
        {
            if (EnableLog) Debug.Log(message);
        }
        public static void LogError<T>(T message)
        {
            if (EnableLog) Debug.Log(message);
        }
    }
}




