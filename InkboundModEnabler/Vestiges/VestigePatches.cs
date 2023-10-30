using BepInEx;
using HarmonyLib;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using static InkboundModEnabler.Vestiges.TemplateVestigeCreator.TemplateVestige;

namespace InkboundModEnabler.Vestiges {
    public static class VestigePatches {

        // Initialize the Vestige caches to allow lookup by name/guid
        [HarmonyPatch(typeof(AssetLibrary))]
        public static class AssetLibarary_Patch {
            public static bool needsInitFiles = true;
            [HarmonyPatch(nameof(AssetLibrary.Initialize))]
            [HarmonyPrefix]
            public static void Initialize_Pre(AssetLibrary __instance) {
                AssetLibUtils.assetLibraryList.Add(__instance);
                if (needsInitFiles && InkboundModEnabler.settings.checkForCustomVestiges.Value) {
                    needsInitFiles = false;
                    foreach (var file in Directory.GetFiles(TemplateVestigeCreator.customVestigePath, "*Vestige*.*", SearchOption.AllDirectories)) {
                        if (file.EndsWith("png") || file.EndsWith("jpg")) continue;
                        if (!ForceOffline.saveIsDirty) {
                            ForceOffline.saveIsDirty = true;
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
                var loc = ClientApp.Inst._applicationState.GetLocalizationRo();
                foreach (var pair in __instance._baseDataTypeToEntries) {
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.VestigeSetData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var setData = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.VestigeSetData;
                                VestigeUtils.instance.VestigeSetDataGUID_To_AssetID[setData.Guid] = item.assetID;
                                VestigeUtils.instance.VestigeSetDataName_To_AssetID[setData.name] = item.assetID;
                                if (!setData.helperData.NameKey.IsNullOrEmpty()) {
                                    VestigeUtils.instance.VestigeSetDataDisplayName_To_AssetID[setData.helperData.NameKey] = item.assetID;
                                }
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                    if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.EquipmentData)) {
                        foreach (var item in pair.Value) {
                            try {
                                var equip = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.EquipmentData;
                                VestigeUtils.instance.EquipmentDataGUID_To_AssetID[equip.Guid] = item.assetID;
                                VestigeUtils.instance.EquipmentDataName_To_AssetID[equip.name] = item.assetID;
                                if (!equip.equipmentName.IsNullOrEmpty()) {
                                    VestigeUtils.instance.EquipmentDisplayName_To_AssetID[equip.equipmentName] = item.assetID;
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
                                VestigeUtils.instance.StatDataGUID_To_AssetID[statData.Guid] = item.assetID;
                                VestigeUtils.instance.StatDataName_To_AssetID[statData.name] = item.assetID;
                                if (!statData.statName.IsNullOrEmpty()) {
                                    VestigeUtils.instance.StatDataDisplayName_To_AssetID[statData.statName] = item.assetID;
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
                                VestigeUtils.instance.StatusEffectDataGUID_To_AssetID[statData.Guid] = item.assetID;
                                VestigeUtils.instance.StatusEffectDataName_To_AssetID[statData.name] = item.assetID;
                                if (!statData.helperData.NameKey.IsNullOrEmpty()) {
                                    VestigeUtils.instance.StatusEffectDataDisplayName_To_AssetID[statData.helperData.NameKey] = item.assetID;
                                }
                            } catch (Exception e) {
                                InkboundModEnabler.log.LogError(e.ToString());
                            }
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(MainMenuScreenVisual))]
        public static class MainMenuScreenVisual_Patch {
            public static bool dump = false;
            [HarmonyPatch(nameof(MainMenuScreenVisual.Initialize))]
            [HarmonyPriority(Priority.VeryLow)]
            [HarmonyPostfix]
            public static void Initialize() {
                var loc = ClientApp.Inst._applicationState.GetLocalizationRo();
                Dictionary<string, AssetID> tmp = new();
                foreach (var entry in VestigeUtils.instance.EquipmentDisplayName_To_AssetID) {
                    tmp[loc.Localize(entry.Key)] = entry.Value;
                }
                VestigeUtils.instance.EquipmentDisplayName_To_AssetID = tmp;
                tmp = new();
                foreach (var entry in VestigeUtils.instance.StatDataDisplayName_To_AssetID) {
                    tmp[loc.Localize(entry.Key)] = entry.Value;
                }
                VestigeUtils.instance.StatDataDisplayName_To_AssetID = tmp;
                tmp = new();
                foreach (var entry in VestigeUtils.instance.StatusEffectDataDisplayName_To_AssetID) {
                    tmp[loc.Localize(entry.Key)] = entry.Value;
                }
                VestigeUtils.instance.StatusEffectDataDisplayName_To_AssetID = tmp;
                tmp = new();
                foreach (var entry in VestigeUtils.instance.VestigeSetDataDisplayName_To_AssetID) {
                    tmp[loc.Localize(entry.Key)] = entry.Value;
                }
                VestigeUtils.instance.VestigeSetDataDisplayName_To_AssetID = tmp;
                if (dump) {
                    VestigeUtils.instance.dump();
                }
                if (InkboundModEnabler.settings.checkForCustomVestiges.Value) {
                    TemplateVestigeCreator.loadCustomVestiges();
                }
            }
        }
    }
}
