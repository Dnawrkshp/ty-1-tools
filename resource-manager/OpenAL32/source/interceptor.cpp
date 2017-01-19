#include "interceptor.h"

#include <Windows.h>
#include <Psapi.h>

#include <iostream>
#include <fstream>
#include <string>

using namespace std;

HMODULE GetModule(HANDLE hProc);

/*
 * Hardcoded offsets from Ty the Tasmanian Tiger r1332_v1.02
 */
#define OFF_LOADRESOURCEFILE	0x001AD280

#define FUNCOFF_RKVCOMPAREAT	0x20
#define FUNCOFF_RKVSEEK			0x202
#define FUNCOFF_RKVREAD			0x213
#define FUNCOFF_MALLOC			0x1D0
#define FUNCOFF_GETRKVFILEENTRY 0x33
#define FUNCOFF_CHECKCACHE		0x133

#define VAROFF_DATARKV			0x60
#define VAROFF_MUSICRKV			0xA7
#define VAROFF_VIDEORKV			0xEB
#define VAROFF_PATCHRKV			0x19

static uint32_t FUNCADDR_RKVSEEK = 0;
static uint32_t FUNCADDR_RKVREAD = 0;
static uint32_t FUNCADDR_MALLOC = 0;
static uint32_t FUNCADDR_LOADRESOURCEFILE = 0;
static uint32_t FUNCADDR_RKVCOMPAREAT = 0;
static uint32_t FUNCADDR_GETRKVFILEENTRY = 0;
static uint32_t FUNCPTR_CHECKCACHE = 0;

static uint32_t VARADDR_DATARKV = 0;
static uint32_t VARADDR_MUSICRKV = 0;
static uint32_t VARADDR_VIDEORKV = 0;
static uint32_t VARADDR_PATCHRKV = 0;

#define FUNCADDR_CHECHCACHE (*(int32_t*)FUNCPTR_CHECKCACHE)

#define RKVNAME(o) ((char*)(o-0x40))
#define RKVID(o)   (*(int32_t*)(o))
#define RKVFSIZE(o) (*(int32_t*)(o+0x4))
#define RKVDSIZE(o) (*(int32_t*)(o+0x8))
#define RKVFOFFSET(o) (*(int32_t*)(o+0xC))
#define RKVDOFFSET(o) (*(int32_t*)(o+0x10))

typedef struct RKVFileEntry {
	unsigned char fileName[32];
	int32_t dirIndex;
	int32_t fileSize;
	int32_t unk_28;
	int32_t fileOffset;
	int32_t unk_30;
	int32_t unk_34;
	int32_t unk_38;
	int32_t unk_3C;
} _RKVFileEntry;

typedef struct RKVDirectoryEntry {
	unsigned char directoryPath[0x100];
} _RKVDirectoryEntry;

typedef void* (*CHECKCACHE) (char * fileName, int32_t * size, char * buffer, int32_t a4);
typedef void (*RKVSEEK) (int32_t stream, int32_t offset, int32_t origin);
typedef void (*RKVREAD) (int32_t stream, void * buffer, int32_t fileSize, int32_t bufferSize);
typedef void* (*MALLOC) (int32_t size);
typedef int32_t (*RKVCOMPAREAT) (char * fileName, int32_t offset);
typedef RKVFileEntry* (*GETRKVFILEENTRY) (char * fileName, int32_t fBlockOffset, int32_t fBlockSize, int32_t blockSize, RKVCOMPAREAT rkvCompareAt);

static GETRKVFILEENTRY TY_GETRKVFILEENTRY = NULL;
static RKVSEEK TY_RKVSEEK = NULL;
static RKVREAD TY_RKVREAD = NULL;
static MALLOC TY_MALLOC = NULL;

static char FULLPATH[0x121];


// Get Directory Entry in RKV by Index
static RKVDirectoryEntry * GetRKVDirectoryEntry(int32_t rkvAddr, int32_t directoryIndex) {
	int32_t max = RKVDSIZE(rkvAddr);
	int32_t offset = RKVDOFFSET(rkvAddr);

	if (directoryIndex >= max || directoryIndex < 0)
		return NULL;

	return (RKVDirectoryEntry*)(offset + (0x100 * directoryIndex));
}

// Try get File Entry in RKV by Filename
static RKVFileEntry * _LoadResourceFile_GetFileEntry_Item(char * fileName, int32_t rkvAddr) {
	RKVFileEntry * entry = TY_GETRKVFILEENTRY(fileName, RKVFOFFSET(rkvAddr), RKVFSIZE(rkvAddr), 64, (RKVCOMPAREAT)FUNCADDR_RKVCOMPAREAT);
	if (entry && entry->fileOffset < 0)
		entry = NULL;

	return entry;
}

// Try get File Entry in any RKV by Filename
static int32_t _LoadResourceFile_GetFileEntry(char * fileName, RKVFileEntry** entry, char ** rkvName) {
	uint32_t rkvs[] = { VARADDR_PATCHRKV, VARADDR_DATARKV, VARADDR_MUSICRKV, VARADDR_VIDEORKV };
	for (int32_t x = 0; x < sizeof(rkvs) / sizeof(uint32_t); x++) {
		*entry = _LoadResourceFile_GetFileEntry_Item(fileName, rkvs[x]);
		if (*entry) {
			if (rkvName)
				*rkvName = RKVNAME(rkvs[x]);
			return rkvs[x];
		}
	}
	return -1;
}

// A replicate of the original LoadResourceFile function found in Ty
static void * _LoadResourceFile(char * fileName, int32_t * size, char * buffer, int32_t bufferSize, signed char a5) {
	RKVFileEntry * entry = NULL;
	char * rkvName = NULL;
	int32_t count = 0;
	CHECKCACHE TY_CHECKCACHE = (CHECKCACHE)FUNCADDR_CHECHCACHE;

	int32_t rkvAddr = _LoadResourceFile_GetFileEntry(fileName, &entry, &rkvName);

	if (a5 && TY_CHECKCACHE != 0) {
		char * buf = (char*)TY_CHECKCACHE(fileName, size, buffer, (int32_t)&a5);
		if (buf)
			return buf;
	}

	if (entry) {
		int32_t rkvID = RKVID(rkvAddr);
		if (size) {
			*size = entry->fileSize;
		}
		if (buffer) {
			if (bufferSize >= 0)
				count = bufferSize;
			else
				count = entry->fileSize;
		}
		else {
			count = entry->fileSize + 1;
			buffer = (char*)TY_MALLOC(count);
			((char*)buffer)[entry->fileSize] = 0;
		}
		if (entry->fileSize) {
			TY_RKVSEEK(rkvID, entry->fileOffset, 0);
			TY_RKVREAD(rkvID, buffer, entry->fileSize, count);
		}
		return buffer;
	}

	return NULL;
}

// Handles the loading of resource files from ./Resource/%RKV%/...
// Ex: "standard.shader" => "./Resource/Data_PC/Shaders/standard.shader" if the file exists
static void * LoadResourceFile(char * fileName, int32_t * size, char * buffer, int32_t bufferSize, signed char a5) {
	RKVFileEntry * fEntry = NULL;
	RKVDirectoryEntry * dEntry = NULL;
	int32_t fSize = 0;
	char rkvName[16];

	int32_t rkvAddr = _LoadResourceFile_GetFileEntry(fileName, &fEntry, NULL);

	if (fEntry) {
		dEntry = GetRKVDirectoryEntry(rkvAddr, fEntry->dirIndex);
		if (dEntry) {
			// Copy rkv filename and remove extension
			memset(rkvName, 0, 16);
			strcpy_s(rkvName, (const char*)RKVNAME(rkvAddr));
			for (int x = 0; x < (int)strlen(rkvName); x++) {
				if (rkvName[x] == '.') {
					rkvName[x] = '\0';
					break;
				}
			}

			// Get relative path of file
			sprintf_s(FULLPATH, "Resource\\%s\\%s%s", rkvName, dEntry->directoryPath, fEntry->fileName);
			
			// Check if file exists
			std::ifstream in(FULLPATH, std::ios::binary | std::ios::ate);
			if (in) {
				fSize = in.tellg();
				if (fSize > 0) {
					if (size)
						*size = fSize;

					if (buffer) {
						if (bufferSize >= 0)
							fSize = bufferSize;
					}
					else {
						buffer = (char*)TY_MALLOC(fSize + 1);
						((char*)buffer)[fSize] = 0;
					}
					
					in.seekg(0, std::ios::beg);
					if (in.read(buffer, fSize)) {
						in.close();
						return buffer;
					}
				}
				in.close();
			}
		}
	}

	return _LoadResourceFile(fileName, size, buffer, bufferSize, a5);
}

// Sets up addresses and installs hook into Ty's resource loading function
void Hook(void)
{
	int32_t callOffset = 0x12;
	unsigned char patch[] = {
		0x55,					// push ebp				; 
		0x8B, 0xEC,				// mov ebp, esp			; 
		0xFF, 0x75, 0x18,		// push [ebp+0x18]		; push a5 into stack
		0xFF, 0x75, 0x14,		// push [ebp+0x14]		; push a4 into stack
		0xFF, 0x75, 0x10,		// push [ebp+0x10]		; push a3 into stack
		0xFF, 0x75, 0x0C,		// push [ebp+0x0C]		; push size into stack
		0xFF, 0x75, 0x08,		// push [ebp+0x08]		; push fileName into stack
		0xE8, 0, 0, 0, 0,		// call 0				; call LoadResourceFile()
		0x83, 0xC4, 0x14,		// add esp, 0x14		; 
		0x5D,					// pop ebp				; 
		0xC3					// ret					; exit function
	};

	// Attach this process and write hook
	DWORD pid = GetCurrentProcessId();
	HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, false, pid);
	if (hProc) {
		// Get base address
		DWORD baseAddress = (DWORD)GetModule(hProc);
		
		// Setup addresses based on FUNCADDR_LOADRESOURCEFILE
		FUNCADDR_LOADRESOURCEFILE = baseAddress + OFF_LOADRESOURCEFILE;
		FUNCADDR_RKVCOMPAREAT = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + FUNCOFF_RKVCOMPAREAT);																			// push offset sub_141CF70
		FUNCADDR_RKVSEEK = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + FUNCOFF_RKVSEEK+1) + (FUNCADDR_LOADRESOURCEFILE + FUNCOFF_RKVSEEK) + 5;								// call dword 0x141cb70
		FUNCADDR_RKVREAD = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + FUNCOFF_RKVREAD+1) + (FUNCADDR_LOADRESOURCEFILE + FUNCOFF_RKVREAD) + 5;								// call dword 0x141cb50
		FUNCADDR_MALLOC = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + FUNCOFF_MALLOC+1) + (FUNCADDR_LOADRESOURCEFILE + FUNCOFF_MALLOC) + 5;								// call dword 0x13f9ce0
		FUNCADDR_GETRKVFILEENTRY = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + FUNCOFF_GETRKVFILEENTRY + 1) + (FUNCADDR_LOADRESOURCEFILE + FUNCOFF_GETRKVFILEENTRY) + 5;	// call dword 0x1411650
		FUNCPTR_CHECKCACHE = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + FUNCOFF_CHECKCACHE);																				// mov eax, [0x160a1d0]

		VARADDR_DATARKV = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + VAROFF_DATARKV);																						// cmp [0x160a218], ebx
		VARADDR_MUSICRKV = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + VAROFF_MUSICRKV);																					// cmp [0x160a278], ebx
		VARADDR_VIDEORKV = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + VAROFF_VIDEORKV);																					// cmp [0x160a2d8], ebx
		VARADDR_PATCHRKV = *(uint32_t*)(FUNCADDR_LOADRESOURCEFILE + VAROFF_PATCHRKV);																					// cmp [0x160a338], ebx

		TY_RKVSEEK = (RKVSEEK)FUNCADDR_RKVSEEK;
		TY_RKVREAD = (RKVREAD)FUNCADDR_RKVREAD;
		TY_MALLOC = (MALLOC)FUNCADDR_MALLOC;
		TY_GETRKVFILEENTRY = (GETRKVFILEENTRY)FUNCADDR_GETRKVFILEENTRY;

		// Calculate relative offset and store into patch
		void * funcPtr = &LoadResourceFile;
		*(uint32_t*)(&patch[callOffset + 1]) = (uint32_t)(((uint32_t)funcPtr - FUNCADDR_LOADRESOURCEFILE - callOffset) - 5);

		// write patch
		WriteProcessMemory(hProc, (LPVOID)FUNCADDR_LOADRESOURCEFILE, &patch, (DWORD)(sizeof(patch)/sizeof(char)), NULL);

		CloseHandle(hProc);
	}
}

// Get Base Address of TY.exe
// from http://stackoverflow.com/a/31895513
HMODULE GetModule(HANDLE hProc)
{
	HMODULE hMods[1024];
	DWORD cbNeeded;
	unsigned int i;

	if (EnumProcessModules(hProc, hMods, sizeof(hMods), &cbNeeded))
	{
		for (i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
		{
			TCHAR szModName[MAX_PATH];
			if (GetModuleFileNameEx(hProc, hMods[i], szModName, sizeof(szModName) / sizeof(TCHAR)))
			{
				std::wstring wstrModName = szModName;
				std::wstring wstrModContain = L"TY.exe";
				if (wstrModName.find(wstrModContain) != string::npos)
				{
					return hMods[i];
				}
			}
		}
	}
	return nullptr;
}
