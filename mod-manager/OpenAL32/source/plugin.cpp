#include "globals.h"
#include "handlers.h"

#include <Windows.h>

#include <cstdint>
#include <iostream>
#include <fstream>
#include <string>

// Plugin main type
typedef void (__cdecl *PLUGINENTRY)(uint64_t baseAddress, uint64_t endAddress, uint64_t revision, float version);

bool GetVersion_Match(uint32_t x, uint64_t * revision, float * version) {
	uint32_t start = x, element0 = 0, element1 = 0;
	char buffer[12];

	try {
		if (*(uint8_t*)x++ != 'r')
			return false;

		// Grab length of the number following
		element0 = x;
		while ((*(uint8_t*)x >= 0x30 && *(uint8_t*)x <= 0x39) || *(uint8_t*)x == '.')
			x++;

		// Ensure the length of the number is greater than 0
		if ((x - element0) == 0)
			return false;

		// Copy number into buffer
		memcpy(buffer, (void*)element0, x - element0);
		buffer[x - element0] = 0;

		// Parse value into revision
		*revision = (uint64_t)std::stoull(buffer);

		// Ensure the next two characters are "_v"
		if (!(*(uint8_t*)x++ == '_' && *(uint8_t*)x++ == 'v'))
			return false;

		// Grab length of the number following
		element1 = x;
		while ((*(uint8_t*)x >= 0x30 && *(uint8_t*)x <= 0x39) || *(uint8_t*)x == '.')
			x++;

		// Ensure the length of the number is greater than 0
		if ((x - element1) == 0)
			return false;

		// Copy number into buffer
		memcpy(buffer, (void*)element1, x - element1);
		buffer[x - element1] = 0;

		// Parse value into version
		*version = (float)std::atof(buffer);
	}
	catch (...) { return false; }

	return true;
}

bool GetVersion(uint64_t * revision, float * version) {
	uint32_t x = 0;
	
	for (x = (uint32_t)BaseAddress; x < (BaseEndAddress - 7); x++)
		if (GetVersion_Match(x, revision, version))
			return true;

	return false;
}

void Handler_Plugin(void * hProc) {
	uint64_t r = 0;
	float v = 0;
	FILE * iniFile, * dllFile;
	char filePath[300];
	HMODULE hPlugin;
	PLUGINENTRY pluginMain;

	if (!GetVersion(&r, &v)) {
		throw std::exception("Unable to acquire version from Ty.exe");
	}

	// Parse plugins
	iniFile = fopen("PC_External/plugins.ini", "rb");
	if (!iniFile)
		return;

	while (fscanf(iniFile, "%[^\n]\n", &filePath) == 1) {

		if (filePath[strlen(filePath) - 1] == '\r')
			filePath[strlen(filePath) - 1] = 0;

		// Ensure the file exists
		dllFile = fopen(filePath, "rb");
		if (!dllFile)
			continue;

		fclose(dllFile);


		hPlugin = LoadLibraryA((char*)filePath);
		if (!hPlugin)
			continue;

		pluginMain = (PLUGINENTRY)GetProcAddress(hPlugin, "main");
		if (pluginMain)
			pluginMain(BaseAddress, BaseEndAddress, r, v);
	}

	fclose(iniFile);
}
