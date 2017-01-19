#pragma once

// Returns true if the specified file exists
bool FileExists(const char * szPath);

// Returns true if the specified directory exists
bool DirectoryExists(const char * szPath);

// Returns true if the directory is created
// Recursively creates directory, ensuring all previously non-existent directories in the path are created
bool DirectoryCreate(const char* szPath);

// Returns true if the path ends with a '\\' or '/'
bool HasTrailingSlash(const char * szPath);
