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
        public ConfigEntry<bool> UseSeparateOfflineSave;
        public ConfigEntry<bool> OverwriteOfflineSave;
        public ConfigEntry<bool> OverwriteSavedOfflineRun;
        public Settings() {
            customVestigeRoot = InkboundModEnabler.conf.Bind("Custom Content", "customVestigeRootDirectory", Path.Combine(Path.Combine(BepInEx.Paths.BepInExRootPath, "custom"), "Vestiges"),
                new ConfigDescription("This is where the mod will use as root to search for new Vestiges and as base path for their icons."));
            checkForCustomVestiges = InkboundModEnabler.conf.Bind("Custom Content", "checkForCustomVestiges", true, new ConfigDescription("Disable this to ignore possible existing custom Vestiges" +
                " in the customVestigeRootDirectors."));
            ForceOfflineMode = InkboundModEnabler.conf.Bind("Force Offline", "ForceOfflineMode", false, new ConfigDescription("Force the game to start in offline mode"));
            persistentPath = InkboundModEnabler.conf.Bind("Force Offline", "persistentPath", Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "persistent_data"),
                new ConfigDescription("This is where Force Offline will save your offline runs when it is turned on."));
            UseSeparateOfflineSave = InkboundModEnabler.conf.Bind("Force Offline", "UseSeparateOfflineSave", false,
                new ConfigDescription("Keep offline saves separate from normal. This allows having offline and online mode to keep track of different progression. This is forced behaviour when using non-cosmetic mods." +
                " Best used with OverwriteOfflineSave set to false"));
            OverwriteOfflineSave = InkboundModEnabler.conf.Bind("Force Offline", "OverwriteOfflineSave", true,
                new ConfigDescription("This prevents the mod from overwriting the offline save with the online saves."));
            OverwriteSavedOfflineRun = InkboundModEnabler.conf.Bind("Force Offline", "OverwriteSavedOfflineRun", false, new ConfigDescription("Ignored if OverwriteOfflineSave is false." +
                " If this is enabled then offline saves will be overwritten even if there is a paused run in the offline save."));
        }
    }
}
