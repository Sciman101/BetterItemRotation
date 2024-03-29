﻿using BepInEx;
using HarmonyLib;
using System.Reflection;

public class HarmonyPatches
{
    private static Harmony instance;

    public static bool IsPatched { get; private set; }
    public const string InstanceId = "test";

    internal static void ApplyHarmonyPatches()
    {
        if (!IsPatched)
        {
            if (instance == null)
            {
                instance = new Harmony(InstanceId);
            }

            instance.PatchAll(Assembly.GetExecutingAssembly());
            IsPatched = true;
        }
    }

    internal static void RemoveHarmonyPatches()
    {
        if (instance != null && IsPatched)
        {
            instance.UnpatchSelf();
            IsPatched = false;
        }
    }
}