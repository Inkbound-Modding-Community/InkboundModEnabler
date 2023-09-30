using BepInEx;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;



namespace InkboundModEnabler {
    [BepInPlugin("ADDB.InkboundModEnabler", "Inkbound Plugin Enabler", "1.0.0.0")]
    class InkboundModEnabler : BaseUnityPlugin {
        private void Awake() {
            Patches.Patch();
        }
    }
}
