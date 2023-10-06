using HarmonyLib;
using Mono.Btls;
using Mono.Net.Security;
using Mono.Unity;
using ShinyShoe;
using ShinyShoe.AnalyticsTracking;
using ShinyShoe.SharedDataLoader;
using System;

namespace InkboundModEnabler {
    public static class Patches {
        [HarmonyPatch(typeof(Mono.Net.Security.MonoTlsProviderFactory))]
        public static class MonoTlsProviderFactory_Patch {
            [HarmonyPatch(nameof(Mono.Net.Security.MonoTlsProviderFactory.CreateDefaultProviderImpl))]
            [HarmonyPrefix]
            public static bool CreateDefaultProviderImpl(ref MobileTlsProvider __result) {
                string text = Environment.GetEnvironmentVariable("MONO_TLS_PROVIDER");
                if (string.IsNullOrEmpty(text)) {
                    text = "default";
                }
                if (!(text == "default") && !(text == "legacy")) {
                    if (!(text == "btls")) {
                        if (!(text == "unitytls")) {
                            __result = Mono.Net.Security.MonoTlsProviderFactory.LookupProvider(text, true);
                            return false;
                        }
                        goto IL_6E;
                    }
                } else {
                    if (UnityTls.IsSupported) {
                        goto IL_6E;
                    }
                    if (!Mono.Net.Security.MonoTlsProviderFactory.IsBtlsSupported()) {
                        throw new NotSupportedException("TLS Support not available.");
                    }
                }
                __result = new MonoBtlsProvider();
                return false;
IL_6E:
                __result = new UnityTlsProvider();
                return false;
            }
        }
        // Disable Analytics
        [HarmonyPatch(typeof(AnalyticsHelper))]
        public static class AnalyticsHelper_Patch {
            [HarmonyPatch(nameof(AnalyticsHelper.Configure))]
            [HarmonyPrefix]
            public static bool Configure() {
                Analytics.EnableTracking(false);
                return false;
            }
        }
        // Disable Crash Reports
        [HarmonyPatch(typeof(ErrorTrackingState))]
        public static class ErrorTrackingState_Patch {
            [HarmonyPatch(nameof(ErrorTrackingState.DoesThisBuildReportErrors))]
            public static bool DoesThisBuildReportErrors(ref bool __result) {
                __result = false;
                return false;
            }
        }

        public static class VestigeUtilsPatches {
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
        }
    }
}