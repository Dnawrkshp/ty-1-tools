# Ty 1 Mod Manager
[![MIT License](https://img.shields.io/npm/l/eslint-find-rules.svg?style=flat-square)](http://opensource.org/licenses/MIT)
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors)

Ty Mod Manager is a tool that manages user-created XML mods for Ty the Tasmanian Tiger.

### How it works
The project consists of a .NET Application and a native library OpenAL32. For more information on how the OpenAL32 manages resources, refer to the [resource-manager](../resource-manager) project.

The .NET Application parses user made mods from the `Mods` folder placed in the root of any Ty installation and installs them into the `PC_External` directory.

The updated OpenAL32 project uses patterns in an attempt to safely install into all current and future releases of Ty. While also handling resource management, this library installs an array of level definitions allowing for the proper loading of custom levels.

### Build requirements
* Visual Studio 2017 RC

### Installation
For precompiled builds refer to the [Wiki](https://github.com/Dnawrkshp/ty-1-tools/wiki/Install).

1. Open [mod-manager solution](mod-manager.sln)
2. Build the project under the Release configuration
3. Copy the resulting `OpenAL32.dll` and `ty-mod-manager.exe` into the root directory of your Ty the Tasmanian Tiger installation

### Debugging
1. Set the Windows system environment variable `TY_1_DIR` to the path of your Ty installation
2. Ensure the path ends with a backslash (ex: `Z:\Games\Ty the Tasmanian Tiger\`)
3. Set the Solution Configuration to Debug

### Usage
1. [Install](#installation)
2. Create the directories `Mods` at the root of your Ty installation
3. Place your XML mods at the root of this directory ([check out some examples](Examples))
4. Launch `ty-mod-manager.exe`

### Creating XML Mods
Refer to the [Wiki](wiki) for details on how to create an XML mod.

### Contributors
Thanks goes to these wonderful people ([emoji key](https://github.com/kentcdodds/all-contributors#emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
| [<img src="https://avatars0.githubusercontent.com/u/2020854?v=3" width="75px;"/><br /><sub>Dan Gerendasy</sub>](https://www.github.com/Dnawrkshp)<br />[📖](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp) [💻](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp) [💡](https://github.com/Dnawrkshp/ty-1-tools/tree/master/mod-manager/Examples)
| :---: |
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/kentcdodds/all-contributors) specification. Contributions of any kind welcome!

### License
Ty 1 Mod Manager is licensed under the MIT License.
