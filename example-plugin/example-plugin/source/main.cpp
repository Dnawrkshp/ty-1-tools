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


// ---- Revision 1402 Offsets
// -- XZ Run Speed
const uint32_t XZ_RS_1402 = 0x2773FC;
// -- XZ Fall Speed
const uint32_t XZ_FS_1402 = 0x277408;
// -- XZ Glide Speed
const uint32_t XZ_GS_1402 = 0x277410;
// -- Jump Speed
const uint32_t JS_1402 = 0x277424;

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

void SuperSpeed_r1402();


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

		// Now ensure the running version of ty is supported
		switch (revision) {
		case 1402:
			XZFallSpeedAddress = baseAddress + XZ_FS_1402;
			XZRunSpeedAddress = baseAddress + XZ_RS_1402;
			XZGlideSpeedAddress = baseAddress + XZ_GS_1402;
			JumpSpeedAddress = baseAddress + JS_1402;
			break;
		default:
			throw std::exception("Unsupported version of Ty");
		}

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
	std::thread task(SuperSpeed_r1402);
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

void SuperSpeed_r1402() {
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
