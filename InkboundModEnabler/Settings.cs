using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkboundModEnabler {
    public class Settings {
        public ConfigEntry<bool> ForceOfflineMode;
        public ConfigEntry<bool> OverwriteSavedOfflineRun;
        public ConfigEntry<bool> ReportOnline;
        public Settings() {
            ForceOfflineMode = InkboundModEnabler.conf.Bind("Force Offline", "ForceOfflineMode", false, new ConfigDescription("Force the game to start in offline mode"));
            OverwriteSavedOfflineRun = InkboundModEnabler.conf.Bind("Force Offline", "OverwriteSavedOfflineRun", false, new ConfigDescription("Set to true to always copy the latest online data" +
                " even if there is an in-progress offline run (which will then be deleted)"));
            ReportOnline = InkboundModEnabler.conf.Bind("Force Offline", "ReportOnline", false, new ConfigDescription("When this is disabled, offline runs are kept separately in a way that they won't be reported" +
                " after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be disabled!"));
        }
    }
}
