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

## Version 1.1.3 (Early Access Patch 0.2.9)
- Added opt-out Force Offline; meaning if any unknown mod without the [CosmeticPlugin] Attribute is encountered the game will automatically enter Forced Offline mode

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
<br />**Default:** false<br />**Description:** Force the game to start in offline mode.

- OverwriteSavedOfflineRun
<br />**Default:** false<br />**Description:** Set to true to always copy the latest online data even if there is an in-progress offline run (which will then be deleted).

- ReportOnline
<br />**Default:** false<br />**Description:** When this is disabled and ForceOfflineMode is enabled, offline runs are kept separately in a way that they won't be reported after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be disabled!