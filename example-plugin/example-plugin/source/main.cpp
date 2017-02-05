#include <cstdint>
#include <thread>
#include <string>

#include <Windows.h>
#include <TlHelp32.h>

/*
 * This is an example plugin for the Ty Mod Manager
 * 
 * This plugin simply adjusts some floats based on the revision number.
 * Specifically, this makes you run, glide, and jump faster.
 *
 * NOTE: In order to debug this, you must have the following:
 *   * TY_1_DIR system environment variable set to the directory of your Ty installation.
 *   * The Mod Manager installed to that directory
 *   * 'plugins.ini' file placed in the folder $(TY_1_DIR)/PC_External with a line containing the fullpath to this plugin (ex: Z:/Games/Ty the Tasmanian Tiger/Mods/example-plugin.dll)
 */

#define TyRunSpeed (*(float*)XZRunSpeedAddress)
#define TyFallSpeed (*(float*)XZFallSpeedAddress)
#define TyGlideSpeed (*(float*)XZGlideSpeedAddress)
#define TyJumpSpeed (*(float*)JumpSpeedAddress)

// Search pattern
// the float at offset 0x24 changes slightly, so we skip that when scanning
const unsigned char pattern[256] = {
	0x00, 0x00, 0xE0, 0x40, 0x33, 0x33, 0xB3, 0x3F, 0x00, 0x00, 0x20, 0x41,
	0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xE0, 0x40, 0x00, 0x00, 0xC0, 0x40,
	0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x80, 0x41, 0x00, 0x00, 0xA0, 0x40,
	0x5C, 0x8F, 0x94, 0x41, 0x00, 0x00, 0x40, 0x3F, 0x00, 0x00, 0x80, 0x42,
	0x00, 0x00, 0x66, 0x43, 0x98, 0xDD, 0x05, 0x41, 0x00, 0x00, 0xE0, 0x3F,
	0x00, 0x00, 0x80, 0x42, 0x00, 0x00, 0xA0, 0x41, 0x00, 0x00, 0x00, 0x00,
	0x66, 0x66, 0x66, 0x3F, 0x00, 0x00, 0xB0, 0x40, 0x00, 0x00, 0xA0, 0x41,
	0x00, 0x00, 0x7A, 0x44, 0x00, 0x00, 0x0C, 0xC2, 0x0A, 0xD7, 0x23, 0x3D,
	0x0A, 0x00, 0x00, 0x00, 0x0A, 0xD7, 0xA3, 0x3C, 0x0A, 0xD7, 0xA3, 0x3C,
	0xCD, 0xCC, 0xCC, 0x3E, 0x0A, 0xD7, 0x23, 0x3D, 0x2C, 0x01, 0x00, 0x00,
	0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x16, 0x43, 0x50, 0xD5, 0x2A, 0x41,
	0x33, 0x33, 0x73, 0x3F, 0x00, 0x00, 0x80, 0x42, 0x00, 0x00, 0x70, 0x42,
	0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xC8, 0x41, 0x00, 0x00, 0xC0, 0x40,
	0x00, 0x00, 0x70, 0x41, 0x00, 0x00, 0xC0, 0x40, 0x00, 0x00, 0x40, 0x3F,
	0xCD, 0xCC, 0x4C, 0x3E, 0x00, 0x00, 0x00, 0x3F, 0x66, 0x66, 0x66, 0x3F,
	0x66, 0x66, 0xA6, 0xBF, 0x8F, 0xC2, 0x75, 0x3C, 0x00, 0x00, 0x80, 0x3E,
	0xCD, 0xCC, 0x4C, 0x3E, 0x00, 0x00, 0x00, 0x3F, 0xAC, 0xC5, 0x27, 0x37,
	0xEC, 0x51, 0x78, 0x3F, 0x33, 0x33, 0x33, 0x3F, 0x00, 0x00, 0x70, 0x41,
	0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFA, 0x43, 0x00, 0x00, 0xFA, 0x43,
	0x66, 0x66, 0x66, 0x3F, 0x00, 0x00, 0xF0, 0x41, 0x00, 0x00, 0x70, 0x41,
	0x00, 0x00, 0xA0, 0x41, 0x00, 0x00, 0x70, 0x41, 0x00, 0x00, 0xC0, 0x40,
	0x00, 0x00, 0x20, 0x41
};

// Keyboard Handler
static HHOOK KeyboardHandle = 0;
static DWORD ProcessID = 0;
static bool ControlToggle = false;

// Base address of Ty.exe
static uint64_t BaseAddress = 0;
static uint64_t EndAddress = 0;

// Active addresses
static uint64_t XZFallSpeedAddress = 0;
static uint64_t XZRunSpeedAddress = 0;
static uint64_t XZGlideSpeedAddress = 0;
static uint64_t JumpSpeedAddress = 0;

// Run Speed
const float RunSpeed = 20;
const float DefaultRunSpeed = 10;
// Glide Speed
const float GlideSpeed = 40;
const float DefaultGlideSpeed = 7;
// Jump Speed
const float JumpSpeed = 23;
const float DefaultJumpSpeed = 18.57f;


LRESULT CALLBACK KeyboardEvent(int nCode, WPARAM wParam, LPARAM lParam);
void SuperSpeed(void);


/*
 * Define and export our main function
 * This is called on load
 *
 * This method takes a few arguments as well.
 *   baseAddress    -  Where Ty.exe was loaded in memory
 *   endAddress     -  The end of the Ty.exe module in memory
 *   revision       -  The revision number of Ty  (rX)
 *   version        -  The version number of Ty   (vX.XX)
 */
__declspec(dllexport) void __cdecl main(uint64_t baseAddress, uint64_t endAddress, uint64_t revision, float version) {

	// In here we can do some neat stuff
	// Let's start by moving the baseAddress and endAddress into our global variables
	BaseAddress = baseAddress;
	EndAddress = endAddress;

	try {
		// Ensure both are valid
		if (!baseAddress || !endAddress)
			throw std::exception("Invalid baseAddress and/or endAddress");

		// Get Current Process ID
		ProcessID = GetCurrentProcessId();
		if (!ProcessID)
			throw std::exception("Unable to retrieve current process id");

		// Hook Windows Keyboard events
		KeyboardHandle = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardEvent, (HMODULE)baseAddress, NULL);
		if (KeyboardHandle == NULL)
			throw std::exception("Unabled to hook keyboard events");
	}
	catch (std::exception& e) {
		OutputDebugStringA(e.what());
		return;
	}

	// Create new thread to run independently
	// When Ty exits, Windows should terminate the thread
	std::thread task(SuperSpeed);
	task.detach();
}

bool IsActiveApplication()
{
	DWORD activeProcId;
	HWND activatedHandle = GetForegroundWindow();

	// No active window
	if (activatedHandle == NULL)
		return false;

	// Get process id of active window
	GetWindowThreadProcessId(activatedHandle, &activeProcId);

	// Check if pid is ours
	return activeProcId == ProcessID;
}

LRESULT CALLBACK KeyboardEvent(int nCode, WPARAM wParam, LPARAM lParam) {
	KBDLLHOOKSTRUCT * kb = (KBDLLHOOKSTRUCT *)lParam;

	if (!nCode && IsActiveApplication()) {
		// Check if control state changed
		if (kb->vkCode == 0xA2 || kb->vkCode == 0xA3) {
			if (wParam == WM_KEYDOWN)
				ControlToggle = true;
			else if (wParam == WM_KEYUP)
				ControlToggle = false;
		}
	}

	return CallNextHookEx(KeyboardHandle, nCode, wParam, lParam);
}


uint32_t GetAddress(const unsigned char * pattern) {
	if (!BaseAddress || !EndAddress || BaseAddress > EndAddress)
		return 0;

	// Just hope we don't get an access violation error
	// If that becomes a problem with users, implement the slower ReadProcessMemory()
	for (uint32_t x = (uint32_t)BaseAddress; x < (EndAddress - 255); x++)
		if (memcmp((void*)x, pattern, 0x24) == 0 && memcmp((void*)(x + 0x28), (void*)&pattern[0x28], 0xD8) == 0)
			return x;

	return 0;
}

void SuperSpeed(void) {
	uint32_t address = 0;

	while (!(address = GetAddress(pattern)))
		Sleep(5000);

	XZRunSpeedAddress = address - 0x04;
	XZFallSpeedAddress = address + 0x08;
	XZGlideSpeedAddress = address + 0x10;
	JumpSpeedAddress = address + 0x24;

	// Loop as long as BaseAddress and KeyboardHandle are valid (or an exception is thrown)
	while (BaseAddress && KeyboardHandle) {
		try {
			if (ControlToggle) {
				TyRunSpeed = RunSpeed;
				TyFallSpeed = RunSpeed;
				TyGlideSpeed = GlideSpeed;
				TyJumpSpeed = JumpSpeed;
			}
			else {
				TyRunSpeed = DefaultRunSpeed;
				TyFallSpeed = DefaultRunSpeed;
				TyGlideSpeed = DefaultGlideSpeed;
				TyJumpSpeed = DefaultJumpSpeed;
			}

			Sleep(10);
		}
		catch (...) { break; }
	}

	if (KeyboardHandle) {
		UnhookWindowsHookEx(KeyboardHandle);
		KeyboardHandle = NULL;
	}
}
