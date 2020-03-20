// Debug.cs
// 
// Part of BloodBank - BloodBank
// 
// Created by: Anthony Chenevier on // 
// Last edited by: Anthony Chenevier on 2020/03/16 11:23 PM


using Verse;

namespace BloodBank {
    public static class Debug
    {
        public static void Log(string msg)
        {
#if (DEBUG)
            Verse.Log.Message($"BloodBank :: {msg}");
#endif
        }

        public static void Warning(string msg)
        {
#if (DEBUG)
            Verse.Log.Warning($"BloodBank :: {msg}");
#endif
        }

        public static void Error(string msg)
        {
#if (DEBUG)
            Verse.Log.Error($"BloodBank :: {msg}");
#endif
        }
    }
}
