using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using InkboundModEnabler.Util;
using Mono.Btls;
using Mono.Net.Security;
using Mono.Unity;
using ShinyShoe;
using ShinyShoe.AnalyticsTracking;
using ShinyShoe.Ares;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;



namespace InkboundModEnabler {
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [CosmeticPlugin]
    class InkboundModEnabler : BaseUnityPlugin {
        public const string PLUGIN_GUID = "InkboundModEnabler";
        public const string PLUGIN_NAME = "Inkbound Mod Enabler";
        public const string PLUGIN_VERSION = "1.2.4";
        public static ManualLogSource log;
        public static InkboundModEnabler instance;
        public static Settings settings;
        public static ConfigFile conf;
        public static WorldServer worldServer;
        public static readonly Harmony HarmonyInstance = new Harmony(PLUGIN_GUID);
        private void Awake() {
            try {
                instance = this;
                log = Logger;
                conf = Config;
                settings = new();
                EnsureDirectories();
                HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                ForceOffline.Init();
            } catch (Exception e) {
                log.LogError(e);
            }
        }
        private void Update() {
        }
        private void EnsureDirectories() {
            var persistent = new DirectoryInfo(settings.persistentPath.Value);
            var vestiges = new DirectoryInfo(settings.customVestigeRoot.Value);
            if (!persistent.Exists) persistent.Create();
            if (!vestiges.Exists) vestiges.Create();
        }
        #region ModEnablerPatches
        // Make unstripped assemblies work
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
        #endregion
        #region HelpfulAccess
        [HarmonyPatch(typeof(WorldServer))]
        public static class WorldServer_Patch {
            [HarmonyPatch(nameof(WorldServer.Update))]
            [HarmonyPostfix]
            public static void Update(WorldServer __instance) {
                worldServer = __instance;
            }
        }
        #endregion
    }
}
