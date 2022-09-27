#pragma once

#include <cstdint>
#include <Windows.h>

#define LEVEL_START   900
#define LEVEL_MAX    1000

extern uint64_t BaseAddress;
extern uint64_t BaseEndAddress;
extern uint64_t PID;
extern HWND ActiveWindow;

extern uint64_t LevelEntriesAddress;
extern uint64_t LoadResourceFileAddress;
extern uint64_t LoadSaveFileAddress;
extern uint64_t SaveSaveFileAddress;
extern uint64_t LoadingLevelIdAddress;

extern uint32_t GameSaveBufferPointer;
