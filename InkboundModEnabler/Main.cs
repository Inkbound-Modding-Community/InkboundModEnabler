using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;



namespace InkboundModEnabler {
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    class InkboundModEnabler : BaseUnityPlugin {
        public const string PLUGIN_GUID = "ADDB.InkboundModEnabler";
        public const string PLUGIN_NAME = "Inkbound Mod Enabler";
        public const string PLUGIN_VERSION = "1.1.0";
        public static Harmony HarmonyInstance => new Harmony(PLUGIN_GUID);
        private void Awake() {
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
