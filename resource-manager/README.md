# [Obsolete] Ty 1 Resource Manager (r1332_v1.02)
[![MIT License](https://img.shields.io/npm/l/eslint-find-rules.svg?style=flat-square)](http://opensource.org/licenses/MIT)
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors)

A little proxy DLL that hooks Ty the Tasmanian Tiger's resource loading method. Once hooked, all load calls can be redirected to another resource.

For version r1332_v1.02 of Ty 1 (and onward presumably), resources can be gathered from `/TY the Tasmanian Tiger/PC_External/` by filename without third-party modification. Note that Ty ignores all subdirectories within `/PC_External/`. For instance, `Shaders/standard.shader` from `Data_PC.rkv` would be placed in `/PC_External/ (/PC_External/standard.shader`) despite originating from a subdirectory.

### How it works
The project consists of a single native dll named OpenAL32. The library acts as a proxy, redirecting any Open AL calls to the real OpenAL32.dll.
When the library gets loaded, it inserts some assembly code into the game; intercepting calls and redirecting to our `LoadResourceFile()`. From there, whenever a file is requested, our method attempts to access the file within the appropriate folder `/TY the Tasmanian Tiger/Resource/NAME_OF_RKV/` where `NAME_OF_RKV` is `Data_PC`, `Music_PC`, `Video_PC` or `Patch_PC`. If the file does not exist, it loads the standard resource.

### Build requirements
* Visual Studio 2015

### Installation
1. Open [resource-manager solution](resource-manager.sln)
2. Build the project
3. Copy the output dll into the directory of your Ty the Tasmanian Tiger installation

### Usage
1. [Install](#installation)
2. Create the directories `/Resource/Data_PC/`, `/Resource/Music_PC/`, `/Resource/Video_PC/`
3. Use the [RKV Extractor](../rkv-extract/) to extract the rkv archives into their respective directories
4. Mod!

### Contributors
Thanks goes to these wonderful people ([emoji key](https://github.com/kentcdodds/all-contributors#emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
| [<img src="https://avatars0.githubusercontent.com/u/2020854?v=3" width="75px;"/><br /><sub>Dan Gerendasy</sub>](https://www.github.com/Dnawrkshp)<br />[📖](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp) [💻](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp)
| :---: |
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/kentcdodds/all-contributors) specification. Contributions of any kind welcome!

### License
Ty 1 Resource Manager is licensed under the MIT License.
