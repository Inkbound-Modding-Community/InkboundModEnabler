using HarmonyLib;
using Mono.Btls;
using Mono.Net.Security;
using Mono.Unity;
using ShinyShoe;
using ShinyShoe.AnalyticsTracking;
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
    }
}