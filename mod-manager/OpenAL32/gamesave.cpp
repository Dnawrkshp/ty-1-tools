#include "globals.h"
#include "handlers.h"

#include <Windows.h>
#include <Psapi.h>

#include <iostream>
#include <fstream>
#include <string>
#include <thread>

using namespace std;

static char * CustomSaveFileStart = NULL;

void * LoadSaveOverwriteFunction = NULL;
uint32_t LoadSaveOverwriteFunctionArg = 0;

uint32_t LoadSaveGameIndexAddress = 0;
uint32_t IsLoadAddress = 0;

static uint32_t GameSaveBufferAddress = 0;

void Load(uint32_t gameSaveIndex) {
	char filename[256];
	FILE * fp;
	uint64_t maxSize = 0x70 * (LEVEL_MAX - LEVEL_START), size = 0;

	// Load extra save data for our custom maps
	sprintf(filename, "GameEx %d", gameSaveIndex + 1);

	fp = fopen(filename, "r");
	if (!fp)
		return;

	fseek(fp, 0, SEEK_END);
	size = ftell(fp);
	fseek(fp, 0, SEEK_SET);
	if (size > maxSize)
		size = maxSize;

	fread(CustomSaveFileStart + (0x70 * LEVEL_START) + 0x10, sizeof(char), (size_t)size, fp);
	fclose(fp);
}

void Save(uint32_t gameSaveIndex) {
	char filename[256];
	FILE * fp;

	// Save extra save data for our custom maps
	sprintf(filename, "GameEx %d", gameSaveIndex + 1);

	fp = fopen(filename, "w");
	if (!fp)
		throw exception("Unable to open output GameEx file for writing");

	fwrite(CustomSaveFileStart + (0x70 * LEVEL_START) + 0x10, sizeof(char), 0x70 * (LEVEL_MAX - LEVEL_START), fp);
	fclose(fp);
}

uint32_t Handle() {
	uint32_t result = 0;
	uint8_t gameSaveIndex = *(uint8_t*)LoadSaveGameIndexAddress;

	__asm {
		mov ecx, LoadSaveOverwriteFunctionArg
		call LoadSaveOverwriteFunction
		mov result, eax
	};
	
	// Call respective method based on game save operation
	if (gameSaveIndex < 3) {
		if (*(bool*)IsLoadAddress)
			Load(gameSaveIndex);
		else
			Save(gameSaveIndex);
	}

	return result;
}

static void Install() {
	char buffer[4];

	if (!GameSaveBufferPointer)
		return;


	while (CustomSaveFileStart && !GameSaveBufferAddress && GameSaveBufferPointer) {
		GameSaveBufferAddress = *(uint32_t*)GameSaveBufferPointer;
		Sleep(100);
	}

	// Point to our custom save file buffer
	HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, false, (DWORD)PID);
	if (hProc) {
		for (uint64_t x = BaseAddress; x < BaseEndAddress; x += 4) {
			if (ReadProcessMemory(hProc, (LPCVOID)x, (LPVOID)buffer, 4, NULL)) {
				if (memcmp(buffer, &GameSaveBufferAddress, 4) == 0)
					*(uint32_t*)x = (uint32_t)CustomSaveFileStart;
			}
		}
		CloseHandle(hProc);
	}
}

void Handler_GameSave(HANDLE hProc)
{
	uint32_t patch = (uint32_t)&Handle - ((uint32_t)LoadSaveFileAddress + 5 + 6);

	if (!hProc)
		return;

	// Allocate region for save file
	CustomSaveFileStart = new char[LEVEL_MAX * 0x70 + 0x2C + 0x10]();

	// Replace pointers to existing level entries with ours
	if (!BaseAddress)
		return;

	if (!LoadSaveFileAddress)
		throw exception("Unable to determine address of load save file function");

	LoadSaveOverwriteFunction = (void*)(*(int32_t*)(LoadSaveFileAddress + 7) + LoadSaveFileAddress + 5 + 6);
	LoadSaveOverwriteFunctionArg = *(uint32_t*)(LoadSaveFileAddress + 2);
	LoadSaveGameIndexAddress = *(uint32_t*)(LoadSaveFileAddress + 0x23);
	IsLoadAddress = *(uint32_t*)(LoadSaveFileAddress + 0xD);

	if (IsLoadAddress < BaseAddress || IsLoadAddress > BaseEndAddress)
		throw exception("Unable to determine address of the load/save operation type");
	if (LoadSaveGameIndexAddress < BaseAddress || LoadSaveGameIndexAddress > BaseEndAddress)
		throw exception("Unable to determine address of the save index");
	if (LoadSaveOverwriteFunctionArg < BaseAddress || LoadSaveOverwriteFunctionArg > BaseEndAddress)
		throw exception("Unable to determine address of the patched load function first argument");
	if ((uint32_t)LoadSaveOverwriteFunction < BaseAddress || (uint32_t)LoadSaveOverwriteFunction > BaseEndAddress)
		throw exception("Unable to determine address of the patched load function");
	
	WriteProcessMemory(hProc, (LPVOID)(LoadSaveFileAddress + 7), &patch, 4, NULL);
	
	std::thread task(Install);
	task.detach();
}
