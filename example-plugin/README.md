# Ty 1 Example Plugin
[![MIT License](https://img.shields.io/npm/l/eslint-find-rules.svg?style=flat-square)](http://opensource.org/licenses/MIT)
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors)

This sample demonstrates how to create a plugin for the Ty Mod Manager.

In this sample, the exported function "main" is called with specific parameters. The function creates a thread to run in the background. This thread changes the run speed, glide speed, and jump height of Ty when control key is held.

Currently, revision 1402 is the only compatible version of Ty.

### Build requirements
* Visual Studio 2017 RC

### Installation
1. Open [example-plugin solution](example-plugin.sln)
2. Build the project under the Release configuration
3. Copy the resulting `example-plugin.dll` and `example-plugin.xml` into the `Mods` directory of your Ty the Tasmanian Tiger installation

### Debugging
1. Set the Windows system environment variable `TY_1_DIR` to the path of your Ty installation
2. Ensure the path ends with a backslash (ex: `Z:\Games\Ty the Tasmanian Tiger\`)
3. Set the Solution Configuration to Debug
4. Ensure the Mod Manager is [installed](../mod-manager#installation)
5. Add a file named plugins.ini to your `PC_External` directory
6. Add an absolute path to `example-plugin.dll`

### Usage
1. [Install](#installation)
2. Run Ty.exe
3. Hold the control key to play Sonic!

### Contributors
Thanks goes to these wonderful people ([emoji key](https://github.com/kentcdodds/all-contributors#emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
| [<img src="https://avatars0.githubusercontent.com/u/2020854?v=3" width="75px;"/><br /><sub>Dan Gerendasy</sub>](https://www.github.com/Dnawrkshp)<br />[📖](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp) [💻](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp) [💡](https://github.com/Dnawrkshp/ty-1-tools/tree/master/mod-manager/Examples)
| :---: |
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/kentcdodds/all-contributors) specification. Contributions of any kind welcome!

### License
Ty 1 Example Plugin is licensed under the MIT License.
