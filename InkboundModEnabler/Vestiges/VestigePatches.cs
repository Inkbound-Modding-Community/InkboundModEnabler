using HarmonyLib;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkboundModEnabler.Vestiges {
    public static class VestigePatches {

        // Initialize the Vestige caches to allow lookup by name/guid
        [HarmonyPatch(typeof(AssetLibrary))]
        public static class AssetLibarary_Patch {
            [HarmonyPatch(nameof(AssetLibrary.Initialize))]
            [HarmonyPrefix]
            public static void Initialize_Pre(AssetLibrary __instance) {
                VestigeUtils.assetLibraryList.Add(__instance);
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
                }
            }
        }
        /* For testint purposes
        [HarmonyPatch(typeof(MainMenuScreenVisual))]
        public static class MainMenuScreenVisual_Patch {
            [HarmonyPatch(nameof(MainMenuScreenVisual.Initialize))]
            [HarmonyPostfix]
            public static void Initialize() {
                try {
                    var v = VestigeUtils.createBlankEquipmentData();
                    v.RegisterNewEquipmentData();
                    VestigeUtils.AddEquipToDefaultVestigeList(v, 1000);
                } catch (Exception ex) {
                    InkboundModEnabler.log.LogError(ex.ToString());
                }
            }
        }
        */
    }
}
