using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine.Assertions;

namespace InkboundModEnabler {
    public static class VestigeUtils {
        internal static VestigeDataContainer _instance;
        public static VestigeDataContainer instance {
            get {
                _instance ??= VestigeDataContainer.VestigeUtilsDumper.loadDump();
                return _instance;
            }
        }
        public static List<AssetLibrary> assetLibraryList = new();
        public static ShinyShoe.Ares.SharedSOs.EquipmentData createBlankEquipmentData() {
            ShinyShoe.Ares.SharedSOs.EquipmentData eq = new();
            eq.rarity = ShinyShoe.Ares.RarityType.Common;
            eq.equipmentName = "Test Name";
            eq.description = "Test Description";
            eq.equipmentType = ShinyShoe.Ares.EquipmentType.Accessory;
            eq.fileID = 0;
            eq.guid = "CB90F870D9FC4F9697F6E18AEF2B7D79";
            return eq;
        }
        public static void AddEquipToDefaultVestigeList(ShinyShoe.Ares.SharedSOs.EquipmentData eq, int weight) {
            ShinyShoe.Ares.SharedSOs.LootTicketData ticket = new();
            ticket.tickets = weight;
            ShinyShoe.Ares.SharedSOs.LootData ld = new();
            ld.equipmentData = eq;
            ticket.lootData = ld;
            getLootListDataByName("AllVestigeList").lootTickets.Add(ticket);
        }
        public static void RegisterNewEquipmentData(this ShinyShoe.Ares.SharedSOs.EquipmentData data) {
            Assert.IsTrue(data != null);
            Assert.IsTrue(!data.Name.IsNullOrWhiteSpace());
            Assert.IsTrue(!data.Guid.IsNullOrWhiteSpace());
            if (data.m_Name.IsNullOrWhiteSpace()) data.m_Name = data.Name;
            AssetLibraryManifest.Entry newEntry = new();
            newEntry.asset = data;
            newEntry.asset.IsLoaded = true;
            var assetID = new AssetID(data.fileID, data.Guid);
            newEntry.classType = typeof(ShinyShoe.Ares.SharedSOs.EquipmentData);
            newEntry.className = "???";
            newEntry.assetID = assetID;
            newEntry.name = data.m_Name;
            foreach (var assetLib in assetLibraryList) {
                assetLib._manifest.entries.Add(newEntry);
                assetLib._nameToEntry[data.name] = newEntry;
                assetLib._assetIDToEntry[assetID] = newEntry;
                List<AssetLibraryManifest.Entry> list;
                if (!assetLib._baseDataTypeToEntries.TryGetValue(newEntry.classType, out list)) {
                    list = new List<AssetLibraryManifest.Entry>();
                    assetLib._baseDataTypeToEntries.Add(newEntry.classType, list);
                }
                list.Add(newEntry);
                _instance.EquipmentDataGUID_To_AssetID[data.Guid] = assetID;
                _instance.EquipmentDataName_To_AssetID[data.name] = assetID;
                _instance.EquipmentDisplayName_To_AssetID[data.Name] = assetID;

            }
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
                        if (InkboundModEnabler.log != null) {
                            InkboundModEnabler.log.LogError(ex.ToString());
                        } else {
                            InkboundModEnabler.logBuffer += ex.ToString() + "\n";
                        }
                    }
                }
                internal static VestigeDataContainer loadDump() {
                    /*
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
                            if (InkboundModEnabler.log != null) {
                                InkboundModEnabler.log.LogError("Critical Error: Failed to load Vestige dump!");
                                InkboundModEnabler.log.LogError(ex.ToString());
                            } else {
                                InkboundModEnabler.logBuffer += "Critical Error: Failed to load Vestige dump!\n";
                                InkboundModEnabler.logBuffer += ex.ToString() + "\n";
                            }
                        }
                    }
                    return ret ?? new();
                    */
                    return new();
                }
            }
        }
    }
    #endregion
}
