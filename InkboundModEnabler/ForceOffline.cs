using BepInEx;
using HarmonyLib;
using InkboundModEnabler.Util;
using InkboundModEnabler.Vestiges;
using ShinyShoe;
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
        [HarmonyPatch(typeof(ClientConfig))]
        internal static class ClientConfig_Patch {
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
        internal static class OfflineModeSystem_Patch {
            [HarmonyPatch(nameof(OfflineModeSystem.Initialize))]
            [HarmonyPrefix]
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
            [HarmonyPatch(nameof(OfflineModeSystem.GetFullSaveDir))]
            [HarmonyPrefix]
            public static bool GetFullSaveDir(ref string __result) {
                if (keepSaveSeparate) {
                    __result = Path.GetFullPath(Path.Combine(persistentPath, "latest-player-save"));
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(SaveGameSystem))]
        internal static class SaveGameSystem_Patch {
            [HarmonyPatch(nameof(SaveGameSystem.GetFullSaveGameTopDir))]
            [HarmonyPrefix]
            public static bool GetFullSaveGameTopDir(ref string __result) {
                if (keepSaveSeparate) {
                    __result = Path.GetFullPath(Path.Combine(persistentPath, "save-game"));
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(AssetLibrary))]
        internal static class AssetLibarary_Patch {
            [HarmonyPatch(nameof(AssetLibrary.Initialize))]
            [HarmonyPriority(50)]
            [HarmonyPostfix]
            public static void Initialize() {
                if (!initialized) {
                    if (TemplateVestigeCreator.filesToCheck.Count > 0) {
                        saveIsDirty = true;
                        InkboundModEnabler.log.LogWarning("Set needForceOffline to true because possible Custom Vestiges were detected and checkForCustomVestiges is enabled.");
                        Init();
                    };
                }
            }
        }
    }
}