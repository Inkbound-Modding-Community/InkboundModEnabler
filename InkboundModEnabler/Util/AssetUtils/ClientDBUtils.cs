using HarmonyLib;
using InkboundModEnabler.Vestiges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace InkboundModEnabler {
    public static class ClientDBUtils {
        public static void RegisterAsset(string GUID, string path) {
            RegisterAsset(GUID, path, "InkboundModEnablerCustomSpriteProvider");
        }
        public static void RegisterAsset(string GUID, string path, string ProviderName) {
            CustomAssetLocator.instance.assetGUIDToPath[GUID] = path;
        }
        [HarmonyPatch(typeof(AddressablesImpl))]
        public static class AddressablesImpl_Patch {
            [HarmonyPatch(nameof(AddressablesImpl.InitializeAsync), new Type[] { typeof(string), typeof(string), typeof(bool) })]
            [HarmonyPostfix]
            public static void InitializeAsync(AddressablesImpl __instance) {
                if (InkboundModEnabler.settings.checkForCustomVestiges.Value) {
                    InkboundModEnabler.log.LogInfo("Initializing Custom ClientDB Assets Infrastructure.");
                    __instance.ResourceManager.ResourceProviders.Add(new CustomSpriteProvider());
                    __instance.AddResourceLocator(CustomAssetLocator.instance);
                }
            }
        }
    }
}
