using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceOffline {
    public class Settings {
        public ConfigEntry<bool> doForceOfflineMode;
        public ConfigEntry<bool> CopyOnlineEvenIfExistingInProgressOffline;
        public ConfigEntry<bool> ReportOnline;
        public Settings() {
            doForceOfflineMode = ForceOffline.conf.Bind("", "doForceOfflineMode", true, new ConfigDescription("Force the game to start in offline mode"));
            CopyOnlineEvenIfExistingInProgressOffline = ForceOffline.conf.Bind("", "CopyOnlineEvenIfExistingInProgressOffline", false, new ConfigDescription("Set to true to always copy the latest online data" +
                " even if there is an in-progress offline run (which would be deleted)"));
            ReportOnline = ForceOffline.conf.Bind("", "ReportOnline", true, new ConfigDescription("When this is disabled, offline runs are kept separately in a way that they won't be reported" +
                " after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be enabled!"));
        }
    }
}
