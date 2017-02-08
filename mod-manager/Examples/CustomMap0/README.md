# Notice
For copyright reasons, none of the files in this directory contain any data. They exist only to demonstrate the standard files, and the naming convention, needed for a custom map.

To learn what files are loaded by the map on entry, run the [mod-manager](../../mod-manager.sln) solution in Debug configuration and watch the Output window for Load calls.

# Naming Convention
Ty loads specific files for each level based on the given level ID. In order to prevent potential collisions between custom maps of the same ID, the Mod Manager manages all level IDs for the user. Therefore, it is impossible to know what level ID your map will be assigned until runtime.

Any `%l` contained with any file name, ini file, or lv2 file will be replaced with the assigned level ID. It is advised to use this convention for ALL files regardless of uniqueness of name. This does not apply to directories.
