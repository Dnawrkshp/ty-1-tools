#include "globals.h"
#include "handlers.h"

#include <Windows.h>
#include <Psapi.h>

#include <iostream>
#include <fstream>
#include <string>

using namespace std;


const char * LEVEL_00 = "z1";
const char * LEVEL_01 = "z2";
const char * LEVEL_02 = "z3";
const char * LEVEL_03 = "z4";
const char * LEVEL_10 = "a1";
const char * LEVEL_11 = "a2";
const char * LEVEL_12 = "a3";
const char * LEVEL_13 = "a4";
const char * LEVEL_20 = "b1";
const char * LEVEL_21 = "b2";
const char * LEVEL_22 = "b3";
const char * LEVEL_23 = "b4";
const char * LEVEL_30 = "c1";
const char * LEVEL_31 = "c2";
const char * LEVEL_32 = "c3";
const char * LEVEL_33 = "c4";
const char * LEVEL_40 = "d1";
const char * LEVEL_41 = "d2";
const char * LEVEL_42 = "d3";
const char * LEVEL_43 = "d4";
const char * LEVEL_50 = "e1";
const char * LEVEL_51 = "e2";
const char * LEVEL_52 = "e3";
const char * LEVEL_53 = "e4";

char * CustomIDContainerPointer = NULL;
char * CustomIDContainerStart = NULL;

void SetupLevelEntries(void);

typedef struct LevelEntry {
	const char * levelID;				// ID
	int32_t unk_04;						// Opal Color
	int32_t unk_08;						// 
	int32_t unk_0C;						// 
	int32_t unk_10;						// 
} _levelEntry;


LevelEntry * LevelEntries = NULL;

bool LevelLoad(char * file, char * path) {
	uint32_t len, currentLevelID, x, i;
	char * levelDir;

	if (!file)
		return false;
	if (!LoadingLevelIdAddress)
		return false;

	len = strlen(file);
	currentLevelID = *(uint32_t*)LoadingLevelIdAddress;

	if (currentLevelID >= LEVEL_START && currentLevelID < LEVEL_MAX) {
		levelDir = (char*)(LevelEntries[currentLevelID].levelID + strlen(LevelEntries[currentLevelID].levelID) + 1);

		// Replace the level ID with %l
		for (x = 0; x < len - 3; x++) {
			if ((file[x] == 'f' || file[x] == 'F') && strncmp(file + x + 1, LevelEntries[currentLevelID].levelID + 1, 1) == 0) {
				file[x] = '%';
				file[x + 1] = 'l';

				i = x + 1;
				while (file[i] >= '0' && file[i] <= '9')
					i++;

				if (i > x + 1) {
					memcpy(file + x + 2, file + i, len - i);
					file[x + (len - i) + 1] = 0;
				}
				break;
			}
		}

		// Set full path to level directory
		sprintf(path, "%s%s", levelDir, file);
		return true;
	}

	return false;
}

void Handler_Level(HANDLE hProc)
{
	SIZE_T read = 0;
	char buffer[4];
	uint64_t LEA_4, LEA_8, LEA_C, LEA_10;
	void * MyLE_4, * MyLE_8, * MyLE_C, * MyLE_10;

	if (!hProc)
		return;

	// Setup level entries
	SetupLevelEntries();

	// Define addresses of unk_04, unk_08, unk_0C, unk_10
	LEA_4 = LevelEntriesAddress + 4;
	LEA_8 = LevelEntriesAddress + 8;
	LEA_C = LevelEntriesAddress + 12;
	LEA_10 = LevelEntriesAddress + 16;
	MyLE_4 = &((char*)LevelEntries)[4];
	MyLE_8 = &((char*)LevelEntries)[8];
	MyLE_C = &((char*)LevelEntries)[12];
	MyLE_10 = &((char*)LevelEntries)[16];

	// Replace pointers to existing level entries with ours
	if (!BaseAddress)
		return;

	if (!LevelEntriesAddress)
		throw exception("Unable to determine address of level ids\n");

	if (!LoadingLevelIdAddress)
		throw exception("Unable to determine address of loading level id\n");

	for (uint64_t x = BaseAddress; x < BaseEndAddress; x++) {
		if (ReadProcessMemory(hProc, (LPCVOID)x, (LPVOID)buffer, 4, &read) && read == 4) {
			if (memcmp(buffer, &LevelEntriesAddress, 4) == 0)
				WriteProcessMemory(hProc, (LPVOID)x, (LPVOID)&LevelEntries, 4, NULL);
			if (memcmp(buffer, &LEA_4, 4) == 0)
				WriteProcessMemory(hProc, (LPVOID)x, (LPVOID)&MyLE_4, 4, NULL);
			if (memcmp(buffer, &LEA_8, 4) == 0)
				WriteProcessMemory(hProc, (LPVOID)x, (LPVOID)&MyLE_8, 4, NULL);
			if (memcmp(buffer, &LEA_C, 4) == 0)
				WriteProcessMemory(hProc, (LPVOID)x, (LPVOID)&MyLE_C, 4, NULL);
			if (memcmp(buffer, &LEA_10, 4) == 0)
				WriteProcessMemory(hProc, (LPVOID)x, (LPVOID)&MyLE_10, 4, NULL);
		}
	}
}

void SetupLevelEntries(void) {
	int i = 0;
	char id[32];
	char path[256];
	FILE* pFile;

	// Setup entries
	LevelEntries = new LevelEntry[LEVEL_MAX]();

	// Setup custom map id memory container
	CustomIDContainerStart = new char[1024 * 4]();
	CustomIDContainerPointer = CustomIDContainerStart;

	if (LevelEntriesAddress)
	{
		i = 24;
		memcpy(LevelEntries, (void*)LevelEntriesAddress, 20 * i);
	}
	else
	{
		// z
		LevelEntries[i].levelID = LEVEL_00;
		LevelEntries[i].unk_04 = 0x003;
		LevelEntries[i].unk_08 = 0x000;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x100;
		i++;

		LevelEntries[i].levelID = LEVEL_01;
		LevelEntries[i].unk_04 = 0x003;
		LevelEntries[i].unk_08 = 0x000;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x100;
		i++;

		LevelEntries[i].levelID = LEVEL_02;
		LevelEntries[i].unk_04 = 0x003;
		LevelEntries[i].unk_08 = 0x000;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_03;
		LevelEntries[i].unk_04 = 0x003;
		LevelEntries[i].unk_08 = 0x000;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		// a
		LevelEntries[i].levelID = LEVEL_10;
		LevelEntries[i].unk_04 = 0x000;
		LevelEntries[i].unk_08 = 0x001;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_11;
		LevelEntries[i].unk_04 = 0x000;
		LevelEntries[i].unk_08 = 0x001;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_12;
		LevelEntries[i].unk_04 = 0x000;
		LevelEntries[i].unk_08 = 0x001;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_13;
		LevelEntries[i].unk_04 = 0x000;
		LevelEntries[i].unk_08 = 0x001;
		LevelEntries[i].unk_0C = 0x000;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		// b
		LevelEntries[i].levelID = LEVEL_20;
		LevelEntries[i].unk_04 = 0x001;
		LevelEntries[i].unk_08 = 0x002;
		LevelEntries[i].unk_0C = 0x001;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_21;
		LevelEntries[i].unk_04 = 0x001;
		LevelEntries[i].unk_08 = 0x002;
		LevelEntries[i].unk_0C = 0x001;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_22;
		LevelEntries[i].unk_04 = 0x001;
		LevelEntries[i].unk_08 = 0x002;
		LevelEntries[i].unk_0C = 0x001;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_23;
		LevelEntries[i].unk_04 = 0x001;
		LevelEntries[i].unk_08 = 0x002;
		LevelEntries[i].unk_0C = 0x001;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		// c
		LevelEntries[i].levelID = LEVEL_30;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x003;
		LevelEntries[i].unk_0C = 0x002;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_31;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x003;
		LevelEntries[i].unk_0C = 0x002;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_32;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x003;
		LevelEntries[i].unk_0C = 0x002;
		LevelEntries[i].unk_10 = 0x101;
		i++;

		LevelEntries[i].levelID = LEVEL_33;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x003;
		LevelEntries[i].unk_0C = 0x002;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		// d
		LevelEntries[i].levelID = LEVEL_40;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x005;
		LevelEntries[i].unk_0C = 0x003;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_41;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x005;
		LevelEntries[i].unk_0C = 0x003;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_42;
		LevelEntries[i].unk_04 = 0x002;
		LevelEntries[i].unk_08 = 0x004;
		LevelEntries[i].unk_0C = 0x003;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_43;
		LevelEntries[i].unk_04 = 0x001;
		LevelEntries[i].unk_08 = 0x002;
		LevelEntries[i].unk_0C = 0x001;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		// e
		LevelEntries[i].levelID = LEVEL_50;
		LevelEntries[i].unk_04 = 0x004;
		LevelEntries[i].unk_08 = 0x005;
		LevelEntries[i].unk_0C = 0x003;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_51;
		LevelEntries[i].unk_04 = 0x004;
		LevelEntries[i].unk_08 = 0x005;
		LevelEntries[i].unk_0C = 0x004;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_52;
		LevelEntries[i].unk_04 = 0x004;
		LevelEntries[i].unk_08 = 0x005;
		LevelEntries[i].unk_0C = 0x004;
		LevelEntries[i].unk_10 = 0x000;
		i++;

		LevelEntries[i].levelID = LEVEL_53;
		LevelEntries[i].unk_04 = 0x004;
		LevelEntries[i].unk_08 = 0x005;
		LevelEntries[i].unk_0C = 0x004;
		LevelEntries[i].unk_10 = 0x000;
		i++;
	}

	// Parse custom maps
	pFile = fopen("PC_External/cmaps.ini", "rb");
	if (!pFile)
		return;

	while (fscanf(pFile, "%i", &i) == 1) {
		if (i < 0 || i > LEVEL_MAX) {
			throw exception("Invalid level index.");
		}
		else {
			fscanf(pFile, "%s %[^\n]\n", &id, &path);
			if (id && strlen(id) > 0) {
				strcpy(CustomIDContainerPointer, id);
				LevelEntries[i].levelID = CustomIDContainerPointer;
				LevelEntries[i].unk_04 = 0x004;
				LevelEntries[i].unk_08 = 0x000;
				LevelEntries[i].unk_0C = 0x000;
				LevelEntries[i].unk_10 = 0x000;

				CustomIDContainerPointer += strlen(CustomIDContainerPointer) + 1;
				strcpy(CustomIDContainerPointer, path);
				CustomIDContainerPointer += strlen(CustomIDContainerPointer) + 1;
			}
			else {
				throw exception("Invalid level id.");
			}
		}
	}

	fclose(pFile);
}
