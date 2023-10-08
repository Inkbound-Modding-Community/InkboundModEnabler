using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InkboundModEnabler {
    public class Settings {
        public ConfigEntry<string> customVestigeRoot;
        public ConfigEntry<string> persistentPath;
        public ConfigEntry<bool> checkForCustomVestiges;
        public ConfigEntry<bool> ForceOfflineMode;
        public ConfigEntry<bool> OverwriteSavedOfflineRun;
        public ConfigEntry<bool> ReportOnline;
        public Settings() {
            customVestigeRoot = InkboundModEnabler.conf.Bind("Custom Content", "customVestigeRootDirectory", Path.Combine(Path.Combine(BepInEx.Paths.BepInExRootPath, "custom"), "Vestiges"),
                new ConfigDescription("This is where the mod will use as root to search for new Vestiges and as base path for their icons."));
            checkForCustomVestiges = InkboundModEnabler.conf.Bind("Custom Content", "checkForCustomVestiges", true, new ConfigDescription("Disable this to ignore possible existing custom Vestiges" +
                " in the customVestigeRootDirectors."));
            ForceOfflineMode = InkboundModEnabler.conf.Bind("Force Offline", "ForceOfflineMode", false, new ConfigDescription("Force the game to start in offline mode"));
            OverwriteSavedOfflineRun = InkboundModEnabler.conf.Bind("Force Offline", "OverwriteSavedOfflineRun", false, new ConfigDescription("Set to true to always copy the latest online data" +
                " even if there is an in-progress offline run (which will then be deleted)."));
            ReportOnline = InkboundModEnabler.conf.Bind("Force Offline", "ReportOnline", false, new ConfigDescription("When this is disabled, offline runs are kept separately in a way that they won't be reported" +
                " after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be disabled!"));
            persistentPath = InkboundModEnabler.conf.Bind("Force Offline", "persistentPath", Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "persistent_data"),
                new ConfigDescription("This is where Force Offline will save your offline runs when it is turned on."));
        }
    }
}
