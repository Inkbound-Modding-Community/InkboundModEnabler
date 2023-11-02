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
using System.Diagnostics;
using System.IO;
using System.Reflection;



namespace InkboundModEnabler {
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [CosmeticPlugin]
    class InkboundModEnabler : BaseUnityPlugin {
        public const string PLUGIN_GUID = "InkboundModEnabler";
        public const string PLUGIN_NAME = "Inkbound Mod Enabler";
        public const string PLUGIN_VERSION = "1.2.8";
        internal static ManualLogSource log;
        public static InkboundModEnabler instance;
        public static Settings settings;
        public static ConfigFile conf;
        public static WorldServer worldServer;
        internal static readonly Harmony HarmonyInstance = new Harmony(PLUGIN_GUID);
        private void Awake() {
            try {
                instance = this;
                log = Logger;
                conf = Config;
                settings = new();
                EnsureDirectories();
                PatchEnablerPatches();
                ForceOffline.PatchForceOfflinePatches();
                HarmonyInstance.PatchAll();
            } catch (Exception e) {
                log.LogFatal(e);
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
        internal static void PatchEnablerPatches() {
            // Make unstripped assemblies work
            var original = AccessTools.Method(typeof(Mono.Net.Security.MonoTlsProviderFactory), nameof(Mono.Net.Security.MonoTlsProviderFactory.CreateDefaultProviderImpl));
            var prefix = AccessTools.Method(typeof(MonoTlsProviderFactory_Patch), nameof(MonoTlsProviderFactory_Patch.CreateDefaultProviderImpl));
            HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
            try {
                // Disable Analytics
                original = AccessTools.Method(typeof(AnalyticsHelper), nameof(AnalyticsHelper.Configure));
                prefix = AccessTools.Method(typeof(AnalyticsHelper_Patch), nameof(AnalyticsHelper_Patch.Configure));
                HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
                // Disable Crash Reports
                original = AccessTools.Method(typeof(ErrorTrackingState), nameof(ErrorTrackingState.DoesThisBuildReportErrors));
                prefix = AccessTools.Method(typeof(ErrorTrackingState_Patch), nameof(ErrorTrackingState.DoesThisBuildReportErrors));
                HarmonyInstance.Patch(original, new HarmonyMethod(prefix));
            } // Crash Reports and Analytics are non-essential patches
            catch (Exception ex) {
                log.LogError(ex);
            }
        }
        internal static class MonoTlsProviderFactory_Patch {
            internal static bool CreateDefaultProviderImpl(ref MobileTlsProvider __result) {
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
        internal static class AnalyticsHelper_Patch {
            internal static bool Configure() {
                Analytics.EnableTracking(false);
                return false;
            }
        }
        internal static class ErrorTrackingState_Patch {
            internal static bool DoesThisBuildReportErrors(ref bool __result) {
                __result = false;
                return false;
            }
        }
        #endregion
        #region HelpfulAccess
        [HarmonyPatch(typeof(WorldServer))]
        internal static class WorldServer_Patch {
            [HarmonyPatch(nameof(WorldServer.Update))]
            [HarmonyPostfix]
            public static void Update(WorldServer __instance) {
                worldServer = __instance;
            }
        }
        #endregion
    }
}
