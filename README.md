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
## Version 1.2.4 (Early Access Patch 0.3.0)
- (***Bubbles***): Add some Unity Extensions
- Code Cleanup

## Version 1.2.3 (Early Access Patch 0.3.0)
- (***Bubbles***): Compatability with game version 0.3.0.

## Version 1.2.2 (Early Access Patch 0.2.9)
- Changed ForceOffline to make options clearer.
- Code Cleanup (Moved to new .csproj template; Removed some VestigeUtils dumping code that wasn't used anymore).

## Version 1.2.1 (Early Access Patch 0.2.9)
- Custom items are now saved when runs are stopped and continued later.
- It is now possible to overwrite existing Vestiges with Vestige Template (The internal name or the guid has to match that of the existing Vestige.

## Version 1.2 (Early Access Patch 0.2.9)
- Finished implementation of VestigeUtils.
- Added the possibility to add new Vestiges by just adding a textfile and an image; no need for doing all the work that comes with creating a mod project.

## Version 1.1.3 (Early Access Patch 0.2.9)
- Added opt-out Force Offline; meaning if any unknown mod without the [CosmeticPlugin] Attribute is encountered the game will automatically enter Forced Offline mode.

## Version 1.1.2 (Early Access Patch 0.2.9)
- Integrate force offline functionality.

## Version 1.1.1 (Early Access Patch 0.2.9)
- Added basic VestigeUtils.
- Change project layout.

## Version 1.1 (Early Access Patch 0.2.9)
- Disabled Crash Reports and Analytics.

## Version 1.0 (Early Access Patch 0.2.8)
- Initial Release.

### Config
\#\# Plugin GUID: InkboundModEnabler

[Custom Content]

\#\# This is where the mod will use as root to search for new Vestiges and as base path for their icons.

\# Setting type: String

\# Default value: D:\Games\Steam\steamapps\common\Inkbound\BepInEx\custom\Vestiges

customVestigeRootDirectory = D:\Games\Steam\steamapps\common\Inkbound\BepInEx\custom\Vestiges

\#\# Disable this to ignore possible existing custom Vestiges in the customVestigeRootDirectors.

\# Setting type: Boolean

\# Default value: true

checkForCustomVestiges = true

[Force Offline]

\#\# Force the game to start in offline mode

\# Setting type: Boolean
\# Default value: false

ForceOfflineMode = false

\#\# This is where Force Offline will save your offline runs when it is turned on.

\# Setting type: String

\# Default value: D:\Games\Steam\steamapps\common\Inkbound\BepInEx\plugins\InkboundModEnabler\persistent_data

persistentPath = D:\Games\Steam\steamapps\common\Inkbound\BepInEx\plugins\InkboundModEnabler\persistent_data

\#\# Keep offline saves separate from normal. This allows having offline and online mode to keep track of different progression. This is forced behaviour when using non-cosmetic mods. Best used with OverwriteOfflineSave set to false

\# Setting type: Boolean

\# Default value: false

UseSeparateOfflineSave = false

\#\# If UseSeparateOfflineSave is enabled, setting this to false prevents the mod from overwriting the offline save with online saves, basically treating offline and online as two separate profiles.

\# Setting type: Boolean

\# Default value: true

\OverwriteOfflineSave = true

\#\# Ignored if OverwriteOfflineSave is false. If this is enabled then offline saves will be overwritten even if there is a paused run in the offline save.

\# Setting type: Boolean

\# Default value: false

OverwriteSavedOfflineRun = false

