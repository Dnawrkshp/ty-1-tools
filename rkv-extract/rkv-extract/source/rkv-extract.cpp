// rkv-extract.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "io.h"

#include <Windows.h>
#include <time.h>

#include <cstdint>
#include <iostream>
#include <fstream>

using namespace std;

// File entry in RKV archive
typedef struct RKVFileEntry {
	char fileName[0x20];
	int32_t dirIndex;
	int32_t fileSize;
	int32_t unk_28;
	int32_t fileOffset;
	int32_t unk_30;
	int32_t lastWrite;
	int32_t unk_38;
	int32_t unk_3C;
} _RKVFileEntry;

// Directory entry in RKV archive
typedef struct RKVDirectoryEntry {
	char directoryPath[0x100];
} _RKVDirectoryEntry;


#define DIRENTRY(f,fl,c,i)		((RKVDirectoryEntry*)(&f[(fl - (c * sizeof(RKVDirectoryEntry))) + (i * sizeof(RKVDirectoryEntry))]))
#define BUFSIZE					(1024 * 1024 * 8)

int main(int argc, const char * argv[])
{
	char fullPath[260];
	char * footer, * out = NULL, * buffer = new char[BUFSIZE]();
	const char * in;
	int32_t dirCount, fileCount, footerLength, writeSize;
	long fileLength;
	RKVDirectoryEntry * d;
	RKVFileEntry * f;
	FILE * fin, * fout;
	clock_t start;

	start = clock();

	if (argc != 3) {
		invalid_input: cout << "rkv-extract.exe <in_file.rkv> <out_dir>" << endl;
		delete buffer;
		if (out)
			delete out;
		return 0;
	}

	in = argv[1];
	out = new char[strlen(argv[2]) + 1];
	strcpy(out, argv[2]);

	// Remove any potential trailing slashes from out
	fileLength = strlen(out) - 1;
	while (fileLength >= 0 && (out[fileLength] == '\\' || out[fileLength] == '/'))
		out[fileLength--] = 0;

	// Ensure valid input file
	if (!FileExists(in)) {
		cout << "Input file does not exist!" << endl << endl;
		goto invalid_input;
	}

	// Create output directory
	if (!DirectoryExists(out)) {
		if (!DirectoryCreate(out)) {
			cout << "Failed to access directory " << out << endl << endl;
			goto invalid_input;
		}
	}

	// Open input RKV
	fin = fopen(in, "rb");
	if (!fin) {
		cout << "Failed to access file " << in << endl << endl;
		goto invalid_input;
	}

	// Get length of file
	fseek(fin, 0, SEEK_END);
	fileLength = ftell(fin);
	if (fileLength <= 0) {
		cout << "Input file is not valid" << endl << endl;
		goto invalid_input;
	}

	// Get directory count
	fseek(fin, -4, SEEK_END);
	fread((void*)&dirCount, 4, 1, fin);

	// Get file count
	fseek(fin, -8, SEEK_END);
	fread((void*)&fileCount, 4, 1, fin);

	// Allocate room for footer
	footerLength = sizeof(RKVDirectoryEntry)*dirCount + sizeof(RKVFileEntry)*fileCount;
	if (footerLength > fileLength || footerLength <= 0) {
		cout << "Invalid number of directory/file entries in " << in << endl << endl;
		goto invalid_input;
	}
	footer = new char[footerLength]();

	// Read footer into buffer
	fseek(fin, fileLength - 8 - footerLength, SEEK_SET);
	if (fread(footer, sizeof(char), footerLength, fin) != (footerLength * sizeof(char))) {
		cout << "Error reading RKV footer" << endl << endl;
		goto invalid_input;
	}

	// Create all directories
	for (int x = 0; x < dirCount; x++) {
		d = DIRENTRY(footer, footerLength, dirCount, x); // (RKVDirectoryEntry*)(&footer[footerLength - ((x - 1) * sizeof(RKVDirectoryEntry))]);
		sprintf_s(fullPath, "%s\\%s\0", out, d->directoryPath);
		if (strlen(d->directoryPath) > 0 && !DirectoryExists(fullPath))
			DirectoryCreate(fullPath);
	}

	// Create all files
	for (int x = 0; x < fileCount; x++) {
		f = (RKVFileEntry*)(&footer[x * sizeof(RKVFileEntry)]);
		if (f->fileOffset >= 0 && f->fileSize > 0 && strlen(f->fileName) > 0) {
			d = DIRENTRY(footer, footerLength, dirCount, f->dirIndex);
			sprintf_s(fullPath, "%s\\%s%s\0", out, d->directoryPath, f->fileName);

			// Open output file
			fout = fopen(fullPath, "wb");
			if (!fout) {
				cout << (x+1) << "/" << fileCount << ": Failed to access \"" << fullPath << "\". Continuing." << endl;
				continue;
			}

			// Go to file offset in stream
			fseek(fin, f->fileOffset, SEEK_SET);
			for (int y = 0; y < f->fileSize; y += BUFSIZE) {
				writeSize = f->fileSize - y;
				if (writeSize > BUFSIZE)
					writeSize = BUFSIZE;

				// Read file into buffer and write buffer into output file
				if (fread(buffer, sizeof(char), writeSize, fin) == (sizeof(char) * writeSize)) {
					if (fwrite(buffer, sizeof(char), writeSize, fout) != (sizeof(char) * writeSize)) {
						cout << (x+1) << "/" << fileCount << ": Failed to write to \"" << fullPath << "\". Continuing." << endl;
						fclose(fout);
						continue;
					}
				}
			}

			fclose(fout);
			cout << (x+1) << "/" << fileCount << ": " << fullPath << endl;
		}
	}

	fclose(fin);
	delete footer;
	delete buffer;
	delete out;

	cout << "Extracted " << fileCount << " files in " << ((double)(clock() - start)) / CLOCKS_PER_SEC << " seconds" << endl;

    return 0;
}

