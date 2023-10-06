using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
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
        public const string PLUGIN_VERSION = "1.1.1";
        public static ManualLogSource log;
        public static InkboundModEnabler instance;
        public static ConfigFile conf;
        public static Harmony HarmonyInstance => new Harmony(PLUGIN_GUID);
        private void Awake() {
            instance = this;
            log = Logger;
            conf = Config;
            VestigeUtils.Init();
            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
