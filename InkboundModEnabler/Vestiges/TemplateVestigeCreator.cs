using BepInEx;
using InkboundModEnabler.Util;
using Newtonsoft.Json;
using ShinyShoe;
using ShinyShoe.Ares;
using ShinyShoe.Ares.SharedSOs;
using ShinyShoe.SharedDataLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkboundModEnabler.Vestiges {
    public static class TemplateVestigeCreator {
        public static List<string> filesToCheck = new();
        public static string customVestigePath => InkboundModEnabler.settings.customVestigeRoot.Value;
        public static Dictionary<string, RarityType> stringToRarity = new() { { "none", RarityType.None }, { "common", RarityType.Common }, { "uncommon", RarityType.Uncommon },
                                                                              { "rare", RarityType.Rare }, { "epic", RarityType.Epic }, { "legendary", RarityType.Legendary } };
        [Serializable]
        public class TemplateVestige {
            [Serializable]
            public class StatChange {
                public string identifier;
                public int value;
            }
            [Serializable]
            public class StatusEffect {
                public string identifier;
                public int stackCount;
                public bool isPrimaryAttribute;
            }
            public string InternalName;
            public string DisplayName;
            public string GUID;
            public string Description;
            public string Rarity;
            public string SmallIconPath;
            public int Weight;
            public List<StatChange> statChanges;
            public List<StatusEffect> statusEffects;
        }
        public static void INEEDTEMPLATE() {
            try {
                var v = new TemplateVestige();
                v.DisplayName = "Test Vestige";
                v.InternalName = "Test Vestige Int";
                v.Description = "Test Vestige Desc";
                v.Rarity = "Common";
                v.SmallIconPath = "MyTestSprite.png";
                v.GUID = "CB90F870D9FC4F9697F6E18AEF2B7D79";
                v.Weight = 99999;
                v.statChanges = new();
                v.statusEffects = new();
                using (var f = new StreamWriter(Path.Combine(customVestigePath, "MyTestVestige.txt"))) {
                    f.Write(JsonConvert.SerializeObject(v, typeof(TemplateVestige), Formatting.Indented, new()));
                }
            } catch (Exception e) {
                InkboundModEnabler.log.LogError(e);
            }
        }
        public static void loadCustomVestiges() {
            foreach (var file in filesToCheck) {
                TemplateVestige template = null;
                try {
                    template = JsonConvert.DeserializeObject<TemplateVestige>(new StreamReader(file).ReadToEnd(), new());
                    InkboundModEnabler.log.LogInfo($"Parsing custom vestige {file}");
                } catch (Exception ex) {
                    InkboundModEnabler.log.LogError($"Tried parsing custom vestige {file} and encountered the error:\n{ex}");
                }
                if (template == null) continue;
                var newVestige = VestigeUtils.createBlankEquipmentData();
                if (!template.Description.IsNullOrWhiteSpace()) {
                    newVestige.description = template.Description;
                } else {
                    InkboundModEnabler.log.LogWarning($"Custom Vestige {file} is missing a Description!");
                }
                if (!template.DisplayName.IsNullOrWhiteSpace()) {
                    newVestige.equipmentName = template.DisplayName;
                } else {
                    InkboundModEnabler.log.LogWarning($"Custom Vestige {file} is missing a Display Name!");
                    newVestige.equipmentName ??= template.InternalName;
                }
                newVestige.m_Name = template.InternalName ?? template.DisplayName;
                if (!template.GUID.IsNullOrWhiteSpace()) {
                    newVestige.guid = template.GUID;
                } else {
                    InkboundModEnabler.log.LogWarning($"Custom Vestige {file} is missing a GUID!");
                    newVestige.guid = ShinyShoe.GuidProvider.Runtime.CreateLongGuid();
                }
                if (!template.Rarity.IsNullOrWhiteSpace() && stringToRarity.ContainsKey(template.Rarity.ToLower())) {
                    newVestige.rarity = stringToRarity[template.Rarity.ToLower()];
                } else {
                    InkboundModEnabler.log.LogWarning($"Custom Vestige {file} is missing a Rarity!");
                }
                if (!template.SmallIconPath.IsNullOrWhiteSpace()) {
                    newVestige.AddSmallSprite($"{newVestige.m_Name}_SmallSprite", template.SmallIconPath);
                } else {
                    InkboundModEnabler.log.LogWarning($"Custom Vestige {file} is missing a SmallIconPath!");
                }
                if (!template.statChanges.IsNullOrEmpty()) {
                    foreach (var sc in template.statChanges) {
                        ShinyShoe.Ares.SharedSOs.StatEntryData sed = new();
                        ShinyShoe.Ares.SharedSOs.StatData sd = VestigeUtils.getStatDataByGUID(sc.identifier) ?? VestigeUtils.getStatDataByDisplayName(sc.identifier) ?? VestigeUtils.getStatDataByName(sc.identifier);
                        if (sd == null) {
                            InkboundModEnabler.log.LogWarning($"Custom Vestige {file} references unknown stat {sc.identifier}!");
                            continue;
                        }
                        sed.entryStatData = sd;
                        sed.entryValue = sc.value;
                        newVestige.statEntries.statEntries.Add(sed);
                    }
                }
                if (!template.statusEffects.IsNullOrEmpty()) {
                    foreach (var effect in template.statusEffects) {
                        ShinyShoe.Ares.SharedSOs.StatusEffectEntryData ssed = new();
                        ShinyShoe.Ares.SharedSOs.StatusEffectData ssd = VestigeUtils.getStatusEffectDataByGUID(effect.identifier) ?? VestigeUtils.getStatusEffectDataByName(effect.identifier) ?? VestigeUtils.getStatusEffectDataByDisplayName(effect.identifier);
                        if (ssd == null) {
                            InkboundModEnabler.log.LogWarning($"Custom Vestige {file} references unknown status effect {effect.identifier}!");
                            continue;
                        }
                        ssed.statusEffect = ssd;
                        ssed.stackCount = effect.stackCount;
                        ssed.isPrimaryAttribute = effect.isPrimaryAttribute;
                        newVestige.statusEffectEntries.StatusEffectEntries.Add(ssed);
                    }
                }
                newVestige.RegisterNewVestige();
                if (template.Weight > 0) {
                    newVestige.AddToLootList("AllVestigeList", template.Weight);
                } else {
                    InkboundModEnabler.log.LogWarning($"Custom Vestige {file} Weight is <= 0!");
                    newVestige.AddToLootList("AllVestigeList", 1);
                }
                InkboundModEnabler.log.LogInfo($"Enabled custom vestige {newVestige.Name}");
            }
        }
    }
}
