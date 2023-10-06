using HarmonyLib;
using Newtonsoft.Json;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace InkboundModEnabler {
    public static class VestigeUtils {
        public static VestigeDataContainer instance;
        public static List<AssetLibrary> assetLibraryList = new();
        public static void RegisterNewEquipmentData(this ShinyShoe.Ares.SharedSOs.EquipmentData data) {
            throw new NotImplementedException();
        }
        public static void Init() {
            instance ??= VestigeDataContainer.VestigeUtilsDumper.loadDump();
        }
        #region getter
        public static ShinyShoe.Ares.SharedSOs.LootTableData getLootTableDataByName(string InternalName) {
            if (instance.LootTableName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.LootTableData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.LootTableData getLootTableDataByGUID(string GUID) {
            if (instance.LootTableGUID_To_AssetID.TryGetValue(GUID, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.LootTableData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.LootListData getLootListDataByName(string InternalName) {
            if (instance.LootListName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.LootListData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.LootListData getLootListDataByGUID(string GUID) {
            if (instance.LootListGUID_To_AssetID.TryGetValue(GUID, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.LootListData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.EquipmentData getEquipmentDataByDisplayName(string DisplayName) {
            if (instance.EquipmentDisplayName_To_AssetID.TryGetValue(DisplayName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.EquipmentData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.EquipmentData getEquipmentDataByName(string InternalName) {
            if (instance.EquipmentDataName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.EquipmentData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.EquipmentData getEquipmentDataByGUID(string GUID) {
            if (instance.EquipmentDataGUID_To_AssetID.TryGetValue(GUID, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.EquipmentData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        #endregion
        #region internal
        [Serializable]
        public sealed class VestigeDataContainer {
            [JsonProperty("LootTableName_To_AssetID")]
            internal SerializableDictionary<string, AssetID> LootTableName_To_AssetID = new();
            [JsonProperty("LootTableGUID_To_AssetID")]
            internal SerializableDictionary<string, AssetID> LootTableGUID_To_AssetID = new();
            [JsonProperty("LootListName_To_AssetID")]
            internal SerializableDictionary<string, AssetID> LootListName_To_AssetID = new();
            [JsonProperty("LootListGUID_To_AssetID")]
            internal SerializableDictionary<string, AssetID> LootListGUID_To_AssetID = new();
            [JsonProperty("EquipmentDataName_To_AssetID")]
            internal SerializableDictionary<string, AssetID> EquipmentDataName_To_AssetID = new();
            [JsonProperty("EquipmentDisplayName_To_AssetID")]
            internal SerializableDictionary<string, AssetID> EquipmentDisplayName_To_AssetID = new();
            [JsonProperty("EquipmentDataGUID_To_AssetID")]
            internal SerializableDictionary<string, AssetID> EquipmentDataGUID_To_AssetID = new();

            internal static class VestigeUtilsDumper {
                internal static bool shouldDump = false;
                internal static void dump() {
                    var dirPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                    var filePath = Path.Combine(dirPath, "VestigeUtils.json");
                    try {
                        using (StreamWriter writter = new StreamWriter(filePath)) {
                            writter.WriteLine(JsonConvert.SerializeObject(instance, typeof(VestigeUtils), Formatting.Indented, new()));
                        }
                    } catch (Exception ex) {
                        InkboundModEnabler.log.LogError(ex.ToString());
                    }
                }
                internal static VestigeDataContainer loadDump() {
                    var dirPath = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName;
                    var filePath = Path.Combine(dirPath, "VestigeUtils.json");
                    VestigeDataContainer ret = null;
                    if (!File.Exists(filePath)) {
                        shouldDump = true;
                    } else {
                        try {
                            using (StreamReader reader = new StreamReader(filePath)) {
                                ret = JsonConvert.DeserializeObject<VestigeDataContainer>(reader.ReadToEnd(), new());
                            }
                        } catch (Exception ex) {
                            InkboundModEnabler.log.LogError("Critical Error: Failed to load Vestige dump!");
                            InkboundModEnabler.log.LogError(ex.ToString());
                        }
                    }
                    return ret ?? new();
                }
            }
        }
        private static class VestigeUtilsPatches {
            [HarmonyPatch(typeof(AssetLibrary))]
            private static class AssetLibarary_Patch {
                [HarmonyPatch(nameof(AssetLibrary.Initialize))]
                [HarmonyPrefix]
                private static void Initialize_Pre(AssetLibrary __instance) {
                    assetLibraryList.Add(__instance);
                }
                [HarmonyPatch(nameof(AssetLibrary.Initialize))]
                [HarmonyPriority(Priority.VeryHigh)]
                [HarmonyPostfix]
                private static void Initialize_Post(AssetLibrary __instance) {
                    if (VestigeDataContainer.VestigeUtilsDumper.shouldDump) {
                        foreach (var pair in __instance._baseDataTypeToEntries) {
                            if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.EquipmentData)) {
                                foreach (var item in pair.Value) {
                                    try {
                                        var equip = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.EquipmentData;
                                        instance.EquipmentDataGUID_To_AssetID[equip.Guid] = item.assetID;
                                        instance.EquipmentDataName_To_AssetID[equip.name] = item.assetID;
                                        if (!equip.Name.IsNullOrEmpty()) {
                                            instance.EquipmentDisplayName_To_AssetID[equip.Name] = item.assetID;
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
                                        instance.LootListGUID_To_AssetID[lootList.Guid] = item.assetID;
                                        instance.LootListName_To_AssetID[lootList.name] = item.assetID;
                                    } catch (Exception e) {
                                        InkboundModEnabler.log.LogError(e.ToString());
                                    }
                                }
                            }
                            if (pair.Key == typeof(ShinyShoe.Ares.SharedSOs.LootTableData)) {
                                foreach (var item in pair.Value) {
                                    try {
                                        var lootTable = __instance.GetOrLoadAsset(item) as ShinyShoe.Ares.SharedSOs.LootTableData;
                                        instance.LootTableGUID_To_AssetID[lootTable.Guid] = item.assetID;
                                        instance.LootTableName_To_AssetID[lootTable.name] = item.assetID;
                                    } catch (Exception e) {
                                        InkboundModEnabler.log.LogError(e.ToString());
                                    }
                                }
                            }
                        }
                        VestigeDataContainer.VestigeUtilsDumper.dump();
                    }
                }
            }
        }
        #endregion
    }
}
