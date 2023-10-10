using HarmonyLib;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace InkboundModEnabler.Vestiges {
    public static class VestigePatches {

        // Initialize the Vestige caches to allow lookup by name/guid
        [HarmonyPatch(typeof(AssetLibrary))]
        public static class AssetLibarary_Patch {
            public static bool needsInitFiles = true;
            [HarmonyPatch(nameof(AssetLibrary.Initialize))]
            [HarmonyPrefix]
            public static void Initialize_Pre(AssetLibrary __instance) {
                VestigeUtils.assetLibraryList.Add(__instance);
                if (needsInitFiles && InkboundModEnabler.settings.checkForCustomVestiges.Value) {
                    needsInitFiles = false;
                    foreach (var file in Directory.GetFiles(TemplateVestigeCreator.customVestigePath, "*Vestige*.*", SearchOption.AllDirectories)) {
                        if (file.EndsWith("png") || file.EndsWith("jpg")) continue;
                        if (!InkboundModEnabler.needForceOffline) {
                            InkboundModEnabler.needForceOffline = true;
                            InkboundModEnabler.log.LogWarning("Set needForceOffline to true because possible Custom Vestiges were detected and checkForCustomVestiges is enabled.");
                        };
                        TemplateVestigeCreator.filesToCheck.Add(file);
                    }
                }
            }
            [HarmonyPatch(nameof(AssetLibrary.Initialize))]
            [HarmonyPriority(Priority.VeryHigh)]
            [HarmonyPostfix]
            public static void Initialize_Post(AssetLibrary __instance) {
                foreach (var pair in __instance._baseDataTypeToEntries) {
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.EquipmentData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var equip = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.EquipmentData;
                                VestigeUtils.instance.EquipmentDataGUID_To_AssetID[equip.Guid] = item.assetID;
                                VestigeUtils.instance.EquipmentDataName_To_AssetID[equip.name] = item.assetID;
                                if (!equip.Name.IsNullOrEmpty()) {
                                    VestigeUtils.instance.EquipmentDisplayName_To_AssetID[equip.Name] = item.assetID;
                                }
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.LootListData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var lootList = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.LootListData;
                                VestigeUtils.instance.LootListGUID_To_AssetID[lootList.Guid] = item.assetID;
                                VestigeUtils.instance.LootListName_To_AssetID[lootList.name] = item.assetID;
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.LootTableData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var lootTable = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.LootTableData;
                                VestigeUtils.instance.LootTableGUID_To_AssetID[lootTable.Guid] = item.assetID;
                                VestigeUtils.instance.LootTableName_To_AssetID[lootTable.name] = item.assetID;
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.StatData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var statData = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.StatData;
                                VestigeUtils.instance.StatDataDisplayName_To_AssetID[statData.Guid] = item.assetID;
                                VestigeUtils.instance.StatDataName_To_AssetID[statData.name] = item.assetID;
                                if (!statData.Name.IsNullOrEmpty()) {
                                    VestigeUtils.instance.StatDataDisplayName_To_AssetID[statData.Name] = item.assetID;
                                }
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.StatusEffectData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var statData = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.StatusEffectData;
                                VestigeUtils.instance.StatusEffectDataDisplayName_To_AssetID[statData.Guid] = item.assetID;
                                VestigeUtils.instance.StatusEffectDataName_To_AssetID[statData.name] = item.assetID;
                                if (!statData.Name.IsNullOrEmpty()) {
                                    VestigeUtils.instance.StatusEffectDataDisplayName_To_AssetID[statData.Name] = item.assetID;
                                }
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                }
            }
        }
        // I'm unsure why exactly this method; the me from half an hour ago thought this was a good spot and who am I to doubt him?
        [HarmonyPatch(typeof(AddressablesImpl))]
        public static class AddressablesImpl_Patch {
            [HarmonyPatch(nameof(AddressablesImpl.InitializeAsync), new Type[] { typeof(string), typeof(string), typeof(bool) })]
            [HarmonyPostfix]
            public static void InitializeAsync(AddressablesImpl __instance) {
                if (InkboundModEnabler.settings.checkForCustomVestiges.Value) {
                    InkboundModEnabler.log.LogInfo("Initializing CustomIcon Infrastructure.");
                    __instance.ResourceManager.ResourceProviders.Add(new CustomIconProvider());
                    __instance.AddResourceLocator(CustomIconLocator.instance);
                }
            }
        }
        [HarmonyPatch(typeof(MainMenuScreenVisual))]
        public static class MainMenuScreenVisual_Patch {
            [HarmonyPatch(nameof(MainMenuScreenVisual.Initialize))]
            [HarmonyPostfix]
            public static void Initialize() {
                if (InkboundModEnabler.settings.checkForCustomVestiges.Value) {
                    TemplateVestigeCreator.loadCustomVestiges();
                }
            }
        }
    }
}
