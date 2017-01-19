#include "io.h"

#include <Windows.h>

int _LastIndexOfSlash(const char * szPath) {
	int l = -1;

	if (!szPath || !(l = strlen(szPath)-1))
		return l;

	for (;l>=0;l--)
		if (szPath[l] == '\\' || szPath[l] == '/')
			return l;

	return -1;
}


bool FileExists(const char * szPath) {
	long dwAttrib = GetFileAttributesA(szPath);
	return (dwAttrib != INVALID_FILE_ATTRIBUTES &&
		!(dwAttrib & FILE_ATTRIBUTE_DIRECTORY));
}

bool DirectoryExists(const char * szPath) {
	long dwAttrib = GetFileAttributesA(szPath);
	return (dwAttrib != INVALID_FILE_ATTRIBUTES &&
		(dwAttrib & FILE_ATTRIBUTE_DIRECTORY));
}

bool DirectoryCreate(const char* szPath) {
	char * leftPath = NULL;
	int lastSlash;

	// If directory already exists, return true
	if (DirectoryExists(szPath))
		return true;
	
	// Get index of last '\\' or '/'
	// If none, try to create directory
	lastSlash = _LastIndexOfSlash(szPath);
	if (lastSlash <= 0)
		goto exitCreate;

	// Create new path from szPath, excluding the last directory
	leftPath = new char[lastSlash+1];
	memcpy(leftPath, szPath, lastSlash);
	leftPath[lastSlash] = 0;

	// Recursive create new path
	if (DirectoryCreate(leftPath))
		goto exitCreate;

	// Clean up
	if (leftPath)
		delete leftPath;
	return false;

exitCreate:
	// Clean up and create directory
	if (leftPath)
		delete leftPath;
	return CreateDirectoryA(szPath, NULL) != 0;
}

bool HasTrailingSlash(const char * szPath) {
	return szPath[strlen(szPath) - 1] == '/' || szPath[strlen(szPath) - 1] == '\\';
}