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

## Version 1.2 (Early Access Patch 0.2.9)
- Finished implementation of VestigeUtils.
- Added the possibility to add new Vestiges by just adding a textfile and an image; no need for doing all the work that comes with creating a mod project!
- TODO: Saving custom items in runs. Currently, while it does save the offline run, after loading into the run the custom Vestiges will have vanished. + Modyfing existing Vestiges.

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
Plugin GUID: ADDB.InkboundModEnabler

[Custom Content]

<b>customVestigeRootDirectory =</b> `D:\Games\Steam\steamapps\common\Inkbound\BepInEx\Custom\Vestiges`
<br/>This is where the mod will use as root to search for new Vestiges and as base path for their icons.

<b>checkForCustomVestiges =</b> `true`
<br/>Disable this to ignore possible existing custom Vestiges in the customVestigeRootDirectors.

[Force Offline]

<b>ForceOfflineMode =</b> `false`
<br/>Force the game to start in offline mode

<b>OverwriteSavedOfflineRun =</b> `false`
<br/>Set to true to always copy the latest online data even if there is an in-progress offline run (which will then be deleted).

<b>ReportOnline =</b> `false`
<br/>When this is disabled, offline runs are kept separately in a way that they won't be reported after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be disabled!

<b>persistentPath =</b> `D:\Games\Steam\steamapps\common\Inkbound\BepInEx\plugins\InkboundModEnabler\persistent_data`
<br/>When this is disabled, offline runs are kept separately in a way that they won't be reported after going back online. This is to prevent any kind of data/synchronization/verification problems. If you're playing with cheats/game changing mods this should be disabled!
