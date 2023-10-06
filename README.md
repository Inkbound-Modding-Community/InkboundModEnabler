BepInEx Plugin to allow loading into the game with unstripped Assemblies.

### Installation

Download the BepInEx Package from the Releases and unzip into your game directory to set it up

Inkbound directory:
Inkbound/

├── BepInEx/ (Put here by you)

│ └── (a few subdirectories)

├── doorstop_config.ini			(Put here by you)

├── winhttp.dll					(Put here by you)

├── UnityPlayer.dll

├── UnityCrashHandler64.exe

├── Inkbound.exe

├── MonoBleedingEdge/

├── Inkbound_Data/

└── Inkbound_BurstDebugInformation_DoNotShip/

### Changelog:

## Version 1.1.2 (Early Access Patch 0.2.9)
- Integrate force offline functionality

## Version 1.1.1 (Early Access Patch 0.2.9)
- Added basic VestigeUtils
- Change project layout

## Version 1.1 (Early Access Patch 0.2.9)
- Disabled Crash Reports and Analytics

## Version 1.0 (Early Access Patch 0.2.8)
- Initial Release

### Config
This mod provides the following config settings:

- ForceOfflineMode
Default: false. Force the game to start in offline mode.

- OverwriteSavedOfflineRun
Default: false. Set to true to always copy the latest online data even if there is an in-progress offline run (which will then be deleted).

- ReportOnline
Default: false. When this is disabled, offline runs are kept separately in a way that they won't be reported after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be disabled!