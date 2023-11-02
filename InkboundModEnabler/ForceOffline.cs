using BepInEx;
using HarmonyLib;
using InkboundModEnabler.Util;
using InkboundModEnabler.Vestiges;
using ShinyShoe;
using ShinyShoe.AnalyticsTracking;
using ShinyShoe.Ares;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InkboundModEnabler {
    public static class ForceOffline {
        private static bool initialized = false;
        private static readonly List<string> excluded = new() { "BepInEx.BaseUnityPlugin", "UnityExplorer.ExplorerBepInPlugin" };
        public static string persistentPath => InkboundModEnabler.settings.persistentPath.Value;
        internal static bool needForceOffline => InkboundModEnabler.settings.ForceOfflineMode.Value || saveIsDirty;
        internal static bool keepSaveSeparate => InkboundModEnabler.settings.UseSeparateOfflineSave.Value || saveIsDirty;
        internal static bool overwriteSave => keepSaveSeparate && InkboundModEnabler.settings.OverwriteOfflineSave.Value && (!AnyUnfinishedOfflineRun() || InkboundModEnabler.settings.OverwriteSavedOfflineRun.Value);
        internal static bool saveIsDirty = false;
        public static void Init() {
            if (!initialized) {
                initialized = true;
                bool separateSaveExists = new DirectoryInfo(persistentPath + @"\latest-player-save").Exists && new DirectoryInfo(persistentPath + @"\save-game").Exists;
                if (overwriteSave || !separateSaveExists) {
                    UtilityFunctions.CopyDirectory(Application.persistentDataPath + @"\latest-player-save", persistentPath + @"\latest-player-save", true);
                    UtilityFunctions.CopyDirectory(Application.persistentDataPath + @"\save-game", persistentPath + @"\save-game", true);
                }
            }
        }
        public static bool AnyUnfinishedOfflineRun() {
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
        #region ForceOfflinePatches
        internal static void PatchForceOfflinePatches() {
            // Allow forcing of offline mode
            var original = AccessTools.Method(typeof(ClientConfig), nameof(ClientConfig.IsForceOffline));
            var prefix = AccessTools.Method(typeof(ClientConfig_Patch), nameof(ClientConfig_Patch.IsForceOffline));
            InkboundModEnabler.HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
            // Check for other plugins without [CosmeticPlugin] and force offline mode if encountered
            original = AccessTools.Method(typeof(OfflineModeSystem), nameof(OfflineModeSystem.Initialize), new Type[] { typeof(OfflineModeSystem.Components) });
            prefix = AccessTools.Method(typeof(OfflineModeSystem_Patch), nameof(OfflineModeSystem_Patch.Initialize));
            InkboundModEnabler.HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
            // Adjust Save directory
            original = AccessTools.Method(typeof(OfflineModeSystem), nameof(OfflineModeSystem.GetFullSaveDir));
            prefix = AccessTools.Method(typeof(OfflineModeSystem_Patch), nameof(OfflineModeSystem_Patch.GetFullSaveDir));
            InkboundModEnabler.HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
            // Adjust Save directory
            original = AccessTools.Method(typeof(SaveGameSystem), nameof(SaveGameSystem.GetFullSaveGameTopDir));
            prefix = AccessTools.Method(typeof(SaveGameSystem_Patch), nameof(SaveGameSystem_Patch.GetFullSaveGameTopDir));
            InkboundModEnabler.HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
            // Flag Saves because if CustomVestiges; Copy saves if needed
            original = AccessTools.Method(typeof(AssetLibrary), nameof(AssetLibrary.Initialize));
            var postfix = AccessTools.Method(typeof(AssetLibrary_Patch), nameof(AssetLibrary_Patch.Initialize));
            InkboundModEnabler.HarmonyInstance.Patch(original, postfix: new HarmonyMethod(postfix) {
                priority = 50
            });
        }
        internal static class ClientConfig_Patch {
            public static bool IsForceOffline(ref bool __result) {
                if (needForceOffline) {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
        internal static class OfflineModeSystem_Patch {
            public static void Initialize() {
                foreach (var ass in AppDomain.CurrentDomain.GetAssemblies()) {
                    foreach (var t in ass.GetTypes()) {
                        if (typeof(BaseUnityPlugin).IsAssignableFrom(t)) {
                            if (t.IsDefined(typeof(CosmeticPluginAttribute))) {
                                InkboundModEnabler.log.LogInfo($"Encountered plugin {t.FullName} with defined CosmeticPlugin Attribute.");
                            } else {
                                if (excluded.Contains(t.FullName)) {
                                    InkboundModEnabler.log.LogInfo($"Encountered plugin excluded from check {t.FullName}");
                                } else {
                                    InkboundModEnabler.log.LogWarning($"Encountered non-cosmetic plugin {t.FullName}; Forcing offline mode!");
                                    saveIsDirty = true;
                                }
                            }
                        }
                    }
                }
            }
            public static bool GetFullSaveDir(ref string __result) {
                if (keepSaveSeparate) {
                    __result = Path.GetFullPath(Path.Combine(persistentPath, "latest-player-save"));
                    return false;
                }
                return true;
            }
        }
        internal static class SaveGameSystem_Patch {
            public static bool GetFullSaveGameTopDir(ref string __result) {
                if (keepSaveSeparate) {
                    __result = Path.GetFullPath(Path.Combine(persistentPath, "save-game"));
                    return false;
                }
                return true;
            }
        }
        internal static class AssetLibrary_Patch {
            public static void Initialize() {
                if (!initialized) {
                    if (TemplateVestigeCreator.filesToCheck.Count > 0) {
                        saveIsDirty = true;
                        InkboundModEnabler.log.LogWarning("Set needForceOffline to true because possible Custom Vestiges were detected and checkForCustomVestiges is enabled.");
                    };
                    Init();
                }
            }
        }
        #endregion
    }
}