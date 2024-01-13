using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BetterItemRotation
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            HarmonyPatches.ApplyHarmonyPatches();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }


    [HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
    class ShipBuildPatch
    {
        static void Postfix(ref ShipBuildModeManager __instance)
        {
            bool rotateHeld = IngamePlayerSettings.Instance.playerInput.actions.FindAction("ReloadBatteries", false).IsPressed() || (StartOfRound.Instance.localPlayerUsingController && __instance.playerActions.Movement.InspectItem.IsPressed());
            bool rotatePressed = IngamePlayerSettings.Instance.playerInput.actions.FindAction("ReloadBatteries", false).WasPressedThisFrame() || (StartOfRound.Instance.localPlayerUsingController && __instance.playerActions.Movement.InspectItem.WasPressedThisFrame());
            float angleDelta = Time.deltaTime * 155f;

            if (rotateHeld)
            {
                // Cancel out the original
                __instance.ghostObject.eulerAngles = new Vector3(__instance.ghostObject.eulerAngles.x, __instance.ghostObject.eulerAngles.y - angleDelta, __instance.ghostObject.eulerAngles.z);
            }

            if (IngamePlayerSettings.Instance.playerInput.actions.FindAction("Sprint", false).IsPressed())
            {
                if (rotatePressed)
                {
                    float angle = Mathf.RoundToMultipleOf(__instance.ghostObject.eulerAngles.y + 45f, 45);
                    __instance.ghostObject.eulerAngles = new Vector3(__instance.ghostObject.eulerAngles.x, angle, __instance.ghostObject.eulerAngles.z);
                }
            }
            else if (rotateHeld)
            {
                __instance.ghostObject.eulerAngles = new Vector3(__instance.ghostObject.eulerAngles.x, __instance.ghostObject.eulerAngles.y + angleDelta, __instance.ghostObject.eulerAngles.z);
            }
        }
    }

    [HarmonyPatch(typeof(ShipBuildModeManager), "CreateGhostObjectAndHighlight")]
    class ShipBuildTooltipPatch
    {
        static void Postfix(ref ShipBuildModeManager __instance)
        {
            HUDManager.Instance.buildModeControlTip.text += "\n(Hold sprint to snap object to angles)";
        }
    }
}
