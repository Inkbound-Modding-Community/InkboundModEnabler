using HarmonyLib;
using ShinyShoe;
using ShinyShoe.Ares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InkboundModEnabler {
    public class ForceOffline {
        public static ForceOffline instance;
        public static string persistentPath = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "persistent_data");
        internal static bool needForceOffline => InkboundModEnabler.settings.ForceOfflineMode.Value || InkboundModEnabler.needForceOffline;
        internal static bool shouldReportOnline => InkboundModEnabler.settings.ReportOnline.Value && !InkboundModEnabler.needForceOffline;
        public ForceOffline() {
            instance = this;
            bool replaceExistingRun = !AnyUnfinishedOfflineRun() || InkboundModEnabler.settings.OverwriteSavedOfflineRun.Value;
            if (replaceExistingRun && !shouldReportOnline) {
                Util.UtilityFunctions.CopyDirectory(Application.persistentDataPath + @"\latest-player-save", persistentPath + @"\latest-player-save", true);
                Util.UtilityFunctions.CopyDirectory(Application.persistentDataPath + @"\save-game", persistentPath + @"\save-game", true);
            }
        }
        public bool AnyUnfinishedOfflineRun() {
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
        [HarmonyPatch(typeof(ClientConfig))]
        public static class ClientConfig_Patch {
            [HarmonyPatch(nameof(ClientConfig.IsForceOffline))]
            [HarmonyPrefix]
            public static bool IsForceOffline(ref bool __result) {
                if (needForceOffline) {
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
                if (needForceOffline && !shouldReportOnline) {
                    __result = Path.GetFullPath(Path.Combine(persistentPath, "latest-player-save"));
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
                if (needForceOffline && !shouldReportOnline) {
                    __result = Path.GetFullPath(Path.Combine(persistentPath, "save-game"));
                    return false;
                }
                return true;
            }
        }
    }
}
