using BepInEx;
using HarmonyLib;
using InkboundModEnabler.Vestiges;
using Newtonsoft.Json;
using ShinyShoe;
using ShinyShoe.Ares;
using ShinyShoe.Ares.SharedSOs;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace InkboundModEnabler {
    public static class VestigeUtils {
        private static VestigeDataContainer _instance;
        public static VestigeDataContainer instance {
            get {
                _instance ??= new();
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
            eq.guid = "CB90F870D9FC4F9697F5E18AEF2B7D79";
            eq.id = "CB90F870D9FC4F9697F5E18AEF2B7D79";
            eq.assetAddressLoot = new("2ec049f1ea67928478a73af299b7ee1a");
            return eq;
        }
        public static void AddSmallSprite(this ShinyShoe.Ares.SharedSOs.EquipmentData eq, string guid, string path) {
            eq.assetAddressIcon = new ShinyShoe.SharedDataLoader.UnityEngine.AssetReferenceSprite();
            eq.assetAddressIcon.m_AssetGUID = guid;
            CustomIconLocator.instance.assetGUIDToSmallIconPath[guid] = path;
        }
        public static void ModifyInLootList(this ShinyShoe.Ares.SharedSOs.EquipmentData eq, string LootListName, int weight) {
            var ticket = (getLootListDataByName(LootListName) ?? getLootListDataByGUID(LootListName))?.lootTickets?.First(ticket => ticket.lootData.equipmentData == eq);
            if (ticket != null) ticket.tickets = weight;
        }
        public static void AddToLootList(this ShinyShoe.Ares.SharedSOs.EquipmentData eq, string LootListName, int weight) {
            ShinyShoe.Ares.SharedSOs.LootTicketData ticket = new();
            ticket.tickets = weight;
            ShinyShoe.Ares.SharedSOs.LootData ld = new();
            ld.equipmentData = eq;
            ticket.lootData = ld;
            (getLootListDataByName(LootListName) ?? getLootListDataByGUID(LootListName))?.lootTickets?.Add(ticket);
        }
        public static void RegisterNewManifestEntry(this AssetLibraryManifest.Entry newEntry) {
            foreach (var assetLib in assetLibraryList) {
                assetLib._manifest.entries.Add(newEntry);
                assetLib._nameToEntry[newEntry.name] = newEntry;
                assetLib._assetIDToEntry[newEntry.assetID] = newEntry;
                assetLib._dataIdToEntry[newEntry.dataId] = newEntry;
                List<AssetLibraryManifest.Entry> list;
                if (!assetLib._baseDataTypeToEntries.TryGetValue(newEntry.classType, out list)) {
                    list = new List<AssetLibraryManifest.Entry>();
                    assetLib._baseDataTypeToEntries.Add(newEntry.classType, list);
                }
                list.Add(newEntry);
            }
        }
        public static void RegisterNewVestige(this ShinyShoe.Ares.SharedSOs.EquipmentData data) {
            Assert.IsTrue(data != null);
            Assert.IsTrue(!data.equipmentName.IsNullOrWhiteSpace());
            Assert.IsTrue(!data.Guid.IsNullOrWhiteSpace());
            if (instance.CustomVestigeGUIDs.Contains(data.guid)) {
                InkboundModEnabler.log.LogError($"Encountered duplicate guid <{data.guid}> while registering Vestige {data.equipmentName}; Providing new but you should fix!");
                data.guid = ShinyShoe.GuidProvider.Runtime.CreateLongGuid();
            } else {
                instance.CustomVestigeGUIDs.Add(data.guid);
            }
            if (data.m_Name.IsNullOrWhiteSpace()) data.m_Name = data.equipmentName;
            if (data.id.IsNullOrWhiteSpace()) data.id = data.guid;
            AssetLibraryManifest.Entry newEntry = new();
            newEntry.asset = data;
            newEntry.dataId = data.Guid;
            newEntry.asset.IsLoaded = true;
            var assetID = new AssetID(data.fileID, data.Guid);
            newEntry.classType = typeof(ShinyShoe.Ares.SharedSOs.EquipmentData);
            newEntry.className = "ShinyShoe.Ares.SharedSOs.EquipmentData";
            newEntry.assetID = assetID;
            newEntry.name = data.m_Name;
            newEntry.RegisterNewManifestEntry();
            var key = Assembly.GetCallingAssembly().FullName;
            _instance.EquipmentDataGUID_To_AssetID[data.Guid] = assetID;
            _instance.EquipmentDataName_To_AssetID[data.name] = assetID;
            _instance.EquipmentDisplayName_To_AssetID[data.equipmentName] = assetID;
            if (!instance.CustomVestigesByAssembly.TryGetValue(key, out var lst)) {
                lst = new();
            }
            lst.Add(newEntry.dataId);
            instance.CustomVestigesByAssembly[key] = lst;
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
        public static ShinyShoe.Ares.SharedSOs.StatData getStatDataByDisplayName(string DisplayName) {
            if (instance.StatDataDisplayName_To_AssetID.TryGetValue(DisplayName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.StatData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.StatData getStatDataByName(string InternalName) {
            if (instance.StatDataName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.StatData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.StatData getStatDataByGUID(string GUID) {
            if (instance.StatDataGUID_To_AssetID.TryGetValue(GUID, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.StatData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.StatusEffectData getStatusEffectDataByDisplayName(string DisplayName) {
            if (instance.StatusEffectDataDisplayName_To_AssetID.TryGetValue(DisplayName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.StatusEffectData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.StatusEffectData getStatusEffectDataByName(string InternalName) {
            if (instance.StatusEffectDataName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.StatusEffectData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.StatusEffectData getStatusEffectDataByGUID(string GUID) {
            if (instance.StatusEffectDataGUID_To_AssetID.TryGetValue(GUID, out var assetID)) {
                foreach (var lib in assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.StatusEffectData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        #endregion
        #region internal
        public sealed class VestigeDataContainer {
            public Dictionary<string, AssetID> LootTableName_To_AssetID = new();
            public Dictionary<string, AssetID> LootTableGUID_To_AssetID = new();
            public Dictionary<string, AssetID> LootListName_To_AssetID = new();
            public Dictionary<string, AssetID> LootListGUID_To_AssetID = new();
            public Dictionary<string, AssetID> EquipmentDataName_To_AssetID = new();
            public Dictionary<string, AssetID> EquipmentDisplayName_To_AssetID = new();
            public Dictionary<string, AssetID> EquipmentDataGUID_To_AssetID = new();
            public Dictionary<string, AssetID> StatDataName_To_AssetID = new();
            public Dictionary<string, AssetID> StatDataDisplayName_To_AssetID = new();
            public Dictionary<string, AssetID> StatDataGUID_To_AssetID = new();
            public Dictionary<string, AssetID> StatusEffectDataName_To_AssetID = new();
            public Dictionary<string, AssetID> StatusEffectDataDisplayName_To_AssetID = new();
            public Dictionary<string, AssetID> StatusEffectDataGUID_To_AssetID = new();
            public Dictionary<string, List<string>> CustomVestigesByAssembly = new();
            public HashSet<string> CustomVestigeGUIDs = new();
        }
    }
    #endregion
}
