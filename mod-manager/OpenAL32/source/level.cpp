#include "globals.h"
#include "handlers.h"

#include <Windows.h>
#include <Psapi.h>

#include <iostream>
#include <fstream>
#include <string>

using namespace std;


#define PATTERN_SIZE 12

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

HMODULE GetModule(HANDLE hProc);
void SetupLevelEntries(void);

typedef struct LevelEntry {
	const char * levelID;
	int32_t unk_04;
	int32_t unk_08;
	int32_t unk_0C;
	int32_t unk_10;
} _levelEntry;


LevelEntry * LevelEntries = NULL;

void Handler_Level(HANDLE hProc)
{
	SIZE_T read = 0;
	char buffer[4];

	if (!hProc)
		return;

	// Setup level entries
	SetupLevelEntries();

	// Replace pointers to existing level entries with ours
	if (!BaseAddress)
		return;

	if (!LevelEntriesAddress)
		throw new exception("Unable to determine address of level ids");

	for (DWORD x = BaseAddress; x < TY_SIZE + BaseAddress; x++)
		if (ReadProcessMemory(hProc, (LPCVOID)x, (LPVOID)buffer, 4, &read) && read == 4)
			if (memcmp(buffer, &LevelEntriesAddress, 4) == 0)
				WriteProcessMemory(hProc, (LPVOID)x, (LPVOID)&LevelEntries, 4, NULL);
}

void SetupLevelEntries(void) {
	int i = 0;
	char id[32];
	FILE* pFile;


	// Setup entries
	LevelEntries = new LevelEntry[1000]();

	// Setup custom map id memory container
	CustomIDContainerStart = new char[1000]();
	CustomIDContainerPointer = CustomIDContainerStart;

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
	LevelEntries[i].unk_0C = 0x003;
	LevelEntries[i].unk_10 = 0x000;
	i++;

	LevelEntries[i].levelID = LEVEL_53;
	LevelEntries[i].unk_04 = 0x004;
	LevelEntries[i].unk_08 = 0x005;
	LevelEntries[i].unk_0C = 0x003;
	LevelEntries[i].unk_10 = 0x000;
	i++;

	// Parse custom maps
	pFile = fopen("PC_External/cmaps.ini", "rb");
	if (!pFile)
		return;

	while (fscanf(pFile, "%i", &i) == 1) {
		if (i < 0 || i > 1000) {
			throw new exception("Invalid level index.");
		}
		else {
			fscanf(pFile, "%s", &id);
			if (id && strlen(id) > 0) {
				strcpy(CustomIDContainerPointer, id);
				LevelEntries[i].levelID = CustomIDContainerPointer;
				LevelEntries[i].unk_04 = 0x002;
				LevelEntries[i].unk_08 = 0x003;
				LevelEntries[i].unk_0C = 0x002;
				LevelEntries[i].unk_10 = 0x101;

				CustomIDContainerPointer += strlen(CustomIDContainerPointer) + 1;
			}
			else {
				throw new exception("Invalid level id.");
			}
		}
	}

	fclose(pFile);
}