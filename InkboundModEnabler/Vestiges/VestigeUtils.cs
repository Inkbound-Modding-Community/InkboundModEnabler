using BepInEx;
using InkboundModEnabler.Vestiges;
using ShinyShoe;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace InkboundModEnabler {
    public static class VestigeUtils {
        public static readonly VestigeDataContainer instance = new();
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
            ClientDBUtils.RegisterAsset(guid, path);
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
            newEntry.RegisterManifestEntry();
            var key = Assembly.GetCallingAssembly().FullName;
            instance.EquipmentDataGUID_To_AssetID[data.Guid] = assetID;
            instance.EquipmentDataName_To_AssetID[data.name] = assetID;
            instance.EquipmentDisplayName_To_AssetID[data.equipmentName] = assetID;
            if (!instance.CustomVestigesByAssembly.TryGetValue(key, out var lst)) {
                lst = new();
            }
            lst.Add(newEntry.dataId);
            instance.CustomVestigesByAssembly[key] = lst;
        }
        #region getter
        public static ShinyShoe.Ares.SharedSOs.LootTableData getLootTableDataByName(string InternalName) {
            if (instance.LootTableName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
                foreach (var lib in AssetLibUtils.assetLibraryList) {
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
        public static ShinyShoe.Ares.SharedSOs.VestigeSetData getVestigeSetDataByDisplayName(string DisplayName) {
            if (instance.VestigeSetDataDisplayName_To_AssetID.TryGetValue(DisplayName, out var assetID)) {
                foreach (var lib in AssetLibUtils.assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.VestigeSetData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.VestigeSetData getVestigeSetDataByName(string InternalName) {
            if (instance.VestigeSetDataName_To_AssetID.TryGetValue(InternalName, out var assetID)) {
                foreach (var lib in AssetLibUtils.assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.VestigeSetData ret) {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
        public static ShinyShoe.Ares.SharedSOs.VestigeSetData getVestigeSetDataByGUID(string GUID) {
            if (instance.VestigeSetDataGUID_To_AssetID.TryGetValue(GUID, out var assetID)) {
                foreach (var lib in AssetLibUtils.assetLibraryList) {
                    var manifest = lib._assetIDToEntry[assetID];
                    if (manifest != null) {
                        var asset = lib.GetOrLoadAsset(manifest);
                        if (asset is ShinyShoe.Ares.SharedSOs.VestigeSetData ret) {
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
            public Dictionary<string, AssetID> VestigeSetDataName_To_AssetID = new();
            public Dictionary<string, AssetID> VestigeSetDataDisplayName_To_AssetID = new();
            public Dictionary<string, AssetID> VestigeSetDataGUID_To_AssetID = new();
            public Dictionary<string, List<string>> CustomVestigesByAssembly = new();
            public HashSet<string> CustomVestigeGUIDs = new();
            public void dump() {
                var loc = ClientApp.Inst._applicationState.GetLocalizationRo();
                InkboundModEnabler.log.LogInfo("---------------------------------------------");
                InkboundModEnabler.log.LogInfo("VestigeSetDatas:");
                foreach (var dumpObject in VestigeUtils.instance.VestigeSetDataGUID_To_AssetID) {
                    var setData = VestigeUtils.getVestigeSetDataByGUID(dumpObject.Key);
                    var dp = setData.NameKey.IsNullOrWhiteSpace() ? "@" : loc.Localize(setData.NameKey);
                    var guid = setData.Guid.IsNullOrWhiteSpace() ? "@" : setData.Guid;
                    var inName = setData.name.IsNullOrWhiteSpace() ? "@" : setData.name;
                    InkboundModEnabler.log.LogInfo($"{dp} : {guid} : {inName}");
                }
                InkboundModEnabler.log.LogInfo("---------------------------------------------");
                InkboundModEnabler.log.LogInfo("StatDatas:");
                foreach (var dumpObject in VestigeUtils.instance.StatDataGUID_To_AssetID) {
                    var statData = VestigeUtils.getStatDataByGUID(dumpObject.Key);
                    var dp = statData.statName.IsNullOrWhiteSpace() ? "@" : loc.Localize(statData.NameKey);
                    var guid = statData.Guid.IsNullOrWhiteSpace() ? "@" : statData.Guid;
                    var inName = statData.name.IsNullOrWhiteSpace() ? "@" : statData.name;
                    InkboundModEnabler.log.LogInfo($"{dp} : {guid} : {inName}");
                }
                InkboundModEnabler.log.LogInfo("---------------------------------------------");
                InkboundModEnabler.log.LogInfo("EquipmentDatas:");
                foreach (var dumpObject in VestigeUtils.instance.EquipmentDataGUID_To_AssetID) {
                    var equip = VestigeUtils.getEquipmentDataByGUID(dumpObject.Key);
                    var dp = equip.equipmentName.IsNullOrWhiteSpace() ? "@" : loc.Localize(equip.equipmentName);
                    var guid = equip.Guid.IsNullOrWhiteSpace() ? "@" : equip.Guid;
                    var inName = equip.name.IsNullOrWhiteSpace() ? "@" : equip.name;
                    InkboundModEnabler.log.LogInfo($"{dp} : {guid} : {inName}");
                }
                InkboundModEnabler.log.LogInfo("---------------------------------------------");
                InkboundModEnabler.log.LogInfo("LootListDatas:");
                foreach (var dumpObject in VestigeUtils.instance.LootListGUID_To_AssetID) {
                    var lootList = VestigeUtils.getLootListDataByGUID(dumpObject.Key);
                    var guid = lootList.Guid.IsNullOrWhiteSpace() ? "@" : lootList.Guid;
                    var inName = lootList.name.IsNullOrWhiteSpace() ? "@" : lootList.name;
                    InkboundModEnabler.log.LogInfo($"{inName} : {guid}");
                }
                InkboundModEnabler.log.LogInfo("---------------------------------------------");
                InkboundModEnabler.log.LogInfo("StatusEffectDatas:");
                foreach (var dumpObject in VestigeUtils.instance.StatusEffectDataGUID_To_AssetID) {
                    var statusEffectData = VestigeUtils.getStatusEffectDataByGUID(dumpObject.Key);
                    var dp = statusEffectData.NameKey.IsNullOrWhiteSpace() ? "@" : loc.Localize(statusEffectData.NameKey);
                    var guid = statusEffectData.Guid.IsNullOrWhiteSpace() ? "@" : statusEffectData.Guid;
                    var inName = statusEffectData.name.IsNullOrWhiteSpace() ? "@" : statusEffectData.name;
                    InkboundModEnabler.log.LogInfo($"{dp} : {guid} : {inName}");
                }
                InkboundModEnabler.log.LogInfo("---------------------------------------------");
                InkboundModEnabler.log.LogInfo("LootTableDatas:");
                foreach (var dumpObject in VestigeUtils.instance.LootTableGUID_To_AssetID) {
                    var lootTable = VestigeUtils.getLootTableDataByGUID(dumpObject.Key);
                    var guid = lootTable.Guid.IsNullOrWhiteSpace() ? "@" : lootTable.Guid;
                    var inName = lootTable.name.IsNullOrWhiteSpace() ? "@" : lootTable.name;
                    InkboundModEnabler.log.LogInfo($"{inName} : {guid}");
                }
            }
        }
    }
    #endregion
}
