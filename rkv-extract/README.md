# Ty 1 RKV Extract
[![MIT License](https://img.shields.io/npm/l/eslint-find-rules.svg?style=flat-square)](http://opensource.org/licenses/MIT)
[![All Contributors](https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square)](#contributors)

RKV Extract is a windows command-line application for extracting the contents of Krome Studios' proprietary RKV archives.

### Usage
RKV Extract is a command-line application.

Open a Command Prompt in the directory of `rkv-extract.exe`. The command parameters are as so:
```sh
rkv-extract.exe <in_file.rkv> <out_path>
```

If `rkv-extract.exe` is placed within the Ty 1 game directory, the following command would extract the contents of `Data_PC.rkv` into `Resource\Data_PC`.
```sh
rkv-extract.exe "Data_PC.rkv" "Resource\Data_PC"
```

### Build requirements
* Visual Studio 2015

### Building
1. Open [rkv-extract solution](rkv-extract.sln)
2. Build the project

### How it works
Krome Studios developed a proprietary archive file format for Ty the Tasmanian Tiger. It's straightforward and unencrypted. The approximate file format is below.
```
RKV Archive
│   Data Block                   ; Contains the contents of all files
│   
└───File Entry Block             ; Contains a collection of file entries
│      File Name                 ; Name of the file (not a path)
│      File Size                 ; Size of the file
│      Directory Entry Index     ; Index of the parent directory with the Directory Entry Block
│      Data Block Offset         ; Pointer to file contents in Data Block
│   
└───Directory Entry Block        ; Contains a collection of directory entries
│      Directory Path            ; Directory path (i.e. "Shaders\")
│   
│   File Count                   ; Number of entries in File Entry Block
│   Directory Count              ; Number of entries in Directory Entry Block
```

### Contributors
Thanks goes to these wonderful people ([emoji key](https://github.com/kentcdodds/all-contributors#emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
| [<img src="https://avatars0.githubusercontent.com/u/2020854?v=3" width="75px;"/><br /><sub>Dan Gerendasy</sub>](https://www.github.com/Dnawrkshp)<br />[📖](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp) [💻](https://www.github.com/Dnawrkshp/ty-1-tools/commits?author=Dnawrkshp)
| :---: |
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/kentcdodds/all-contributors) specification. Contributions of any kind welcome!

### License
Ty 1 RKV Extract is licensed under the MIT License.
