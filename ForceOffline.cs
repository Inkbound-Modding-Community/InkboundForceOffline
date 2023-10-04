using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ShinyShoe;
using ShinyShoe.Ares;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ForceOffline {
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class ForceOffline : BaseUnityPlugin {
        public const string PLUGIN_GUID = "ADDB.ForceOffline";
        public const string PLUGIN_NAME = "Force Offline";
        public const string PLUGIN_VERSION = "1.0.0";
        public static ManualLogSource log;
        public static ForceOffline instance;
        public static Settings settings;
        public static ConfigFile conf;
        // public static string persistentPath = Path.Combine(Paths.PluginPath, Path.Combine(PluginInfo.PLUGIN_NAME, "persistent_data"));
        public static string persistentPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "persistent_data");
        public static Harmony HarmonyInstance => new Harmony(PluginInfo.PLUGIN_GUID);
        private void Awake() {
            instance = this;
            log = Logger;
            conf = Config;
            settings = new();
            if ((!UnfinishedRun() || settings.CopyOnlineEvenIfExistingInProgressOffline.Value) && !settings.ReportOnline.Value) {
                CopyDirectory(Application.persistentDataPath + @"\latest-player-save", persistentPath + @"\latest-player-save", true);
                CopyDirectory(Application.persistentDataPath + @"\save-game", persistentPath + @"\save-game", true);
            }
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        public bool UnfinishedRun() {
            if (!Directory.Exists(persistentPath)) {
                Directory.CreateDirectory(persistentPath);
            }
            var pathToDir = Path.GetFullPath(Path.Combine(persistentPath, "latest-player-save"));
            var fullSaveGameDir = Path.GetFullPath(Path.Combine(persistentPath, "save-game"));
            if (!Directory.Exists(pathToDir) || !Directory.Exists(fullSaveGameDir)) return false;
            foreach (var file in Directory.GetFiles(pathToDir)) {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Name.StartsWith("PlayerSave") && fileInfo.Name.EndsWith(".plr") && !fileInfo.Name.Contains("_old")) {
                    OfflineModeSystemHelper.TryGetPlayerSaveFile(fileInfo.FullName, out var saveFile, out var saveFileBytes);
                    int saveGameId = saveFile.PlayerDataRo.GetSaveGameId();
                    if (saveGameId != 0) {
                        var saveGameFilePath = Path.Combine(fullSaveGameDir, "SaveGame.sav");
                        if (!SaveGameFile.TryDeserializeHeader(saveGameFilePath, out var saveGameHeader) && saveGameHeader.SaveGameId == saveGameId) {
                            saveGameFilePath = Path.Combine(fullSaveGameDir, "SaveGame_notack.sav");
                            if (!SaveGameFile.TryDeserializeHeader(saveGameFilePath, out saveGameHeader) && saveGameHeader.SaveGameId == saveGameId) {
                                return false;
                            }
                        }
                        return !saveGameHeader.IsHub;
                    }
                }
            }
            return false;
        }
        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive) {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles()) {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive) {
                foreach (DirectoryInfo subDir in dirs) {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}