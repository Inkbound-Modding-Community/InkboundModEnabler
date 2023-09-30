using HarmonyLib;
using Mono.Btls;
using Mono.Net.Security;
using Mono.Unity;
using System;
using System.Reflection;

namespace InkboundModEnabler {
    public static class Patches {
        public static Harmony HarmonyInstance => new Harmony("ADDB.InkboundModEnabler");
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

        public static void Patch() {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}