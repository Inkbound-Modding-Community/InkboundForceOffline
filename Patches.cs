using HarmonyLib;
using ShinyShoe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ForceOffline {
    public static class Patches {
        [HarmonyPatch(typeof(ClientConfig))]
        public static class ClientConfig_Patch {
            [HarmonyPatch(nameof(ClientConfig.IsForceOffline))]
            [HarmonyPrefix]
            public static bool IsForceOffline(ref bool __result) {
                if (ForceOffline.settings.doForceOfflineMode.Value) {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(OfflineModeSystem))]
        public static class OfflineModeSystem_Patch {
            [HarmonyPatch(nameof(OfflineModeSystem.GetFullSaveDir))]
            [HarmonyPrefix]
            public static bool GetFullSaveDir(ref string __result) {
                if (ForceOffline.settings.doForceOfflineMode.Value && !ForceOffline.settings.ReportOnline.Value) {
                    __result = Path.GetFullPath(Path.Combine(ForceOffline.persistentPath, "latest-player-save"));
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(SaveGameSystem))]
        public static class SaveGameSystem_Patch {
            [HarmonyPatch(nameof(SaveGameSystem.GetFullSaveGameTopDir))]
            [HarmonyPrefix]
            public static bool GetFullSaveGameTopDir(ref string __result) {
                if (ForceOffline.settings.doForceOfflineMode.Value && !ForceOffline.settings.ReportOnline.Value) {
                    __result = Path.GetFullPath(Path.Combine(ForceOffline.persistentPath, "save-game"));
                    return false;
                }
                return true;
            }
        }
        /*
        [HarmonyPatch(typeof(Application))]
        public static class Application_Patch {
            [HarmonyPatch(nameof(Application.persistentDataPath))]
            [HarmonyPrefix]
            public static bool persistentDataPath(ref string __result) {
                if (ForceOffline.settings.doForceOfflineMode.Value) {
                    __result = BepInEx.Paths.PluginPath + PluginInfo.PLUGIN_NAME + @"\persistent_data\";
                    return false;
                }
                return true;
            }
        }
        */
    }
}
