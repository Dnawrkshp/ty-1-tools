#include <Windows.h>
#include <Psapi.h>

#include "handlers.h"
#include "al.h"
#include "alc.h"

#include <string>


HMODULE GetModule(HANDLE hProc);

uint64_t BaseAddress = 0;
uint64_t BaseEndAddress = 0;
uint64_t PID = 0;

uint64_t LevelEntriesAddress = 0;
uint64_t LoadResourceFileAddress = 0;
uint64_t LoadSaveFileAddress = 0;
uint64_t SaveSaveFileAddress = 0;

uint32_t GameSaveBufferPointer = 0;

static LPALENABLE _alEnable = NULL;
static LPALDISABLE _alDisable = NULL;
static LPALISENABLED _alIsEnabled = NULL;
static LPALGETSTRING _alGetString = NULL;
static LPALGETBOOLEANV _alGetBooleanv = NULL;
static LPALGETINTEGERV _alGetIntegerv = NULL;
static LPALGETFLOATV _alGetFloatv = NULL;
static LPALGETDOUBLEV _alGetDoublev = NULL;
static LPALGETBOOLEAN _alGetBoolean = NULL;
static LPALGETINTEGER _alGetInteger = NULL;
static LPALGETFLOAT _alGetFloat = NULL;
static LPALGETDOUBLE _alGetDouble = NULL;
static LPALGETERROR _alGetError = NULL;
static LPALISEXTENSIONPRESENT _alIsExtensionPresent = NULL;
static LPALGETPROCADDRESS _alGetProcAddress = NULL;
static LPALGETENUMVALUE _alGetEnumValue = NULL;
static LPALLISTENERF _alListenerf = NULL;
static LPALLISTENER3F _alListener3f = NULL;
static LPALLISTENERFV _alListenerfv = NULL;
static LPALLISTENERI _alListeneri = NULL;
static LPALLISTENER3I _alListener3i = NULL;
static LPALLISTENERIV _alListeneriv = NULL;
static LPALGETLISTENERF _alGetListenerf = NULL;
static LPALGETLISTENER3F _alGetListener3f = NULL;
static LPALGETLISTENERFV _alGetListenerfv = NULL;
static LPALGETLISTENERI _alGetListeneri = NULL;
static LPALGETLISTENER3I _alGetListener3i = NULL;
static LPALGETLISTENERIV _alGetListeneriv = NULL;
static LPALGENSOURCES _alGenSources = NULL;
static LPALDELETESOURCES _alDeleteSources = NULL;
static LPALISSOURCE _alIsSource = NULL;
static LPALSOURCEF _alSourcef = NULL;
static LPALSOURCE3F _alSource3f = NULL;
static LPALSOURCEFV _alSourcefv = NULL;
static LPALSOURCEI _alSourcei = NULL;
static LPALSOURCE3I _alSource3i = NULL;
static LPALSOURCEIV _alSourceiv = NULL;
static LPALGETSOURCEF _alGetSourcef = NULL;
static LPALGETSOURCE3F _alGetSource3f = NULL;
static LPALGETSOURCEFV _alGetSourcefv = NULL;
static LPALGETSOURCEI _alGetSourcei = NULL;
static LPALGETSOURCE3I _alGetSource3i = NULL;
static LPALGETSOURCEIV _alGetSourceiv = NULL;
static LPALSOURCEPLAYV _alSourcePlayv = NULL;
static LPALSOURCESTOPV _alSourceStopv = NULL;
static LPALSOURCEREWINDV _alSourceRewindv = NULL;
static LPALSOURCEPAUSEV _alSourcePausev = NULL;
static LPALSOURCEPLAY _alSourcePlay = NULL;
static LPALSOURCESTOP _alSourceStop = NULL;
static LPALSOURCEREWIND _alSourceRewind = NULL;
static LPALSOURCEPAUSE _alSourcePause = NULL;
static LPALSOURCEQUEUEBUFFERS _alSourceQueueBuffers = NULL;
static LPALSOURCEUNQUEUEBUFFERS _alSourceUnqueueBuffers = NULL;
static LPALGENBUFFERS _alGenBuffers = NULL;
static LPALDELETEBUFFERS _alDeleteBuffers = NULL;
static LPALISBUFFER _alIsBuffer = NULL;
static LPALBUFFERDATA _alBufferData = NULL;
static LPALBUFFERF _alBufferf = NULL;
static LPALBUFFER3F _alBuffer3f = NULL;
static LPALBUFFERFV _alBufferfv = NULL;
static LPALBUFFERI _alBufferi = NULL;
static LPALBUFFER3I _alBuffer3i = NULL;
static LPALBUFFERIV _alBufferiv = NULL;
static LPALGETBUFFERF _alGetBufferf = NULL;
static LPALGETBUFFER3F _alGetBuffer3f = NULL;
static LPALGETBUFFERFV _alGetBufferfv = NULL;
static LPALGETBUFFERI _alGetBufferi = NULL;
static LPALGETBUFFER3I _alGetBuffer3i = NULL;
static LPALGETBUFFERIV _alGetBufferiv = NULL;
static LPALDOPPLERFACTOR _alDopplerFactor = NULL;
static LPALDOPPLERVELOCITY _alDopplerVelocity = NULL;
static LPALSPEEDOFSOUND _alSpeedOfSound = NULL;
static LPALDISTANCEMODEL _alDistanceModel = NULL;

static LPALCCREATECONTEXT _alcCreateContext = NULL;
static LPALCMAKECONTEXTCURRENT _alcMakeContextCurrent = NULL;
static LPALCPROCESSCONTEXT _alcProcessContext = NULL;
static LPALCSUSPENDCONTEXT _alcSuspendContext = NULL;
static LPALCDESTROYCONTEXT _alcDestroyContext = NULL;
static LPALCGETCURRENTCONTEXT _alcGetCurrentContext = NULL;
static LPALCGETCONTEXTSDEVICE _alcGetContextsDevice = NULL;
static LPALCOPENDEVICE _alcOpenDevice = NULL;
static LPALCCLOSEDEVICE _alcCloseDevice = NULL;
static LPALCGETERROR _alcGetError = NULL;
static LPALCISEXTENSIONPRESENT _alcIsExtensionPresent = NULL;
static LPALCGETPROCADDRESS _alcGetProcAddress = NULL;
static LPALCGETENUMVALUE _alcGetEnumValue = NULL;
static LPALCGETSTRING _alcGetString = NULL;
static LPALCGETINTEGERV _alcGetIntegerv = NULL;
static LPALCCAPTUREOPENDEVICE _alcCaptureOpenDevice = NULL;
static LPALCCAPTURECLOSEDEVICE _alcCaptureCloseDevice = NULL;
static LPALCCAPTURESTART _alcCaptureStart = NULL;
static LPALCCAPTURESTOP _alcCaptureStop = NULL;
static LPALCCAPTURESAMPLES _alcCaptureSamples = NULL;


static int hasLoadedLibrary = 0;

// Use some patterns to determine the address of important functions/data
// This won't always work but should work better than hardcoding offsets
static void GetAddresses(HANDLE hProc) {
	unsigned char buffer[32];
	SIZE_T read = 0;
	const unsigned char n[] = { 0x90, 0x90, 0x90, 0x90, 0x90 };

	const unsigned char pattern_levelentries[] = {
		0x55,					// push ebp
		0x8B, 0xEC,				// mov ebp, esp
		0x53,					// push ebx
		0x8B, 0x5D, 0x08,		// mov ebx, ebp+8
		0x56,					// push esi
		0x57,					// push edi
		0x33, 0xFF,				// xor edi, edi
		0xBE					// 
	};

	const unsigned char pattern_loadresourcefile[] = {
		0x55, 0x8B, 0xEC, 0x83, 0xEC, 0x08, 0x53, 0x56, 0x33, 0xDB, 0xC7, 0x45, 0xFC, 0xFF, 0xFF, 0xFF, 0xFF, 0x33, 0xF6, 0x57, 0x8B, 0x7D, 0x08
	};

	for (uint64_t x = BaseAddress; x < BaseEndAddress; x++) {
		if (ReadProcessMemory(hProc, (LPCVOID)x, (LPVOID)buffer, 32, &read) && read == 32) {
			if (!LevelEntriesAddress && memcmp(buffer, pattern_levelentries, 12) == 0)
				LevelEntriesAddress = *(UINT32*)(x + 12);
			else if (!LoadResourceFileAddress && memcmp(buffer, pattern_loadresourcefile, 23) == 0)
				LoadResourceFileAddress = x;
			else if (buffer[0] == 0xA1 && buffer[5] == 0x83 && buffer[6] == 0xC4 && buffer[7] == 0x0C && buffer[8] == 0xFF && buffer[9] == 0x35 && buffer[14] == 0xFF && buffer[15] == 0xB0 &&
					 buffer[20] == 0xE8 && buffer[25] == 0x83 && buffer[26] == 0xC4 && buffer[27] == 0x04 && buffer[28] == 0x50 && buffer[29] == 0xE8) {
				WriteProcessMemory(hProc, (LPVOID)(x + 0x14), &n, 5, &read);
				WriteProcessMemory(hProc, (LPVOID)(x + 0x1D), &n, 5, &read);
				GameSaveBufferPointer = *(uint32_t*)(x + 1);
			}
			else if (buffer[0] == 0x74 && buffer[1] == 0x61 && buffer[2] == 0x56 && buffer[3] == 0x8B &&
					 buffer[9] == 0xBA && buffer[10] == 0x2C && buffer[11] == 0x0B && buffer[12] == 0x00 &&
					 buffer[13] == 0x00 && buffer[14] == 0x69 && buffer[15] == 0xCE && buffer[16] == 0xA8 &&
					 buffer[17] == 0x00 && buffer[18] == 0x00 && buffer[19] == 0x00 && buffer[20] == 0x8B &&
				 	 buffer[26] == 0x3B && buffer[27] == 0xCA && buffer[28] == 0x0F && buffer[29] == 0x42 &&
					 buffer[30] == 0xD1 && buffer[31] == 0x52) {
				LoadSaveFileAddress = x - 0x1E;
			}
		}
	}
}

static int Init() {
	if (hasLoadedLibrary)
		return 1;

	// Load OpenAL32.dll
	HMODULE hModOAL32 = LoadLibrary(L"C:\\Windows\\System32\\OpenAL32.dll");
	if (!hModOAL32)
		hModOAL32 = LoadLibrary(L"C:\\Windows\\SysWOW64\\OpenAL32.dll");
	if (!hModOAL32)
		return 0;

	// Get exported methods
	_alEnable = (LPALENABLE)GetProcAddress(hModOAL32, "alEnable");
	_alDisable = (LPALDISABLE)GetProcAddress(hModOAL32, "alDisable");
	_alIsEnabled = (LPALISENABLED)GetProcAddress(hModOAL32, "alIsEnabled");
	_alGetString = (LPALGETSTRING)GetProcAddress(hModOAL32, "alGetString");
	_alGetBooleanv = (LPALGETBOOLEANV)GetProcAddress(hModOAL32, "alGetBooleanv");
	_alGetIntegerv = (LPALGETINTEGERV)GetProcAddress(hModOAL32, "alGetIntegerv");
	_alGetFloatv = (LPALGETFLOATV)GetProcAddress(hModOAL32, "alGetFloatv");
	_alGetDoublev = (LPALGETDOUBLEV)GetProcAddress(hModOAL32, "alGetDoublev");
	_alGetBoolean = (LPALGETBOOLEAN)GetProcAddress(hModOAL32, "alGetBoolean");
	_alGetInteger = (LPALGETINTEGER)GetProcAddress(hModOAL32, "alGetInteger");
	_alGetFloat = (LPALGETFLOAT)GetProcAddress(hModOAL32, "alGetFloat");
	_alGetDouble = (LPALGETDOUBLE)GetProcAddress(hModOAL32, "alGetDouble");
	_alGetError = (LPALGETERROR)GetProcAddress(hModOAL32, "alGetError");
	_alIsExtensionPresent = (LPALISEXTENSIONPRESENT)GetProcAddress(hModOAL32, "alIsExtensionPresent");
	_alGetProcAddress = (LPALGETPROCADDRESS)GetProcAddress(hModOAL32, "alGetProcAddress");
	_alGetEnumValue = (LPALGETENUMVALUE)GetProcAddress(hModOAL32, "alGetEnumValue");
	_alListenerf = (LPALLISTENERF)GetProcAddress(hModOAL32, "alListenerf");
	_alListener3f = (LPALLISTENER3F)GetProcAddress(hModOAL32, "alListener3f");
	_alListenerfv = (LPALLISTENERFV)GetProcAddress(hModOAL32, "alListenerfv");
	_alListeneri = (LPALLISTENERI)GetProcAddress(hModOAL32, "alListeneri");
	_alListener3i = (LPALLISTENER3I)GetProcAddress(hModOAL32, "alListener3i");
	_alListeneriv = (LPALLISTENERIV)GetProcAddress(hModOAL32, "alListeneriv");
	_alGetListenerf = (LPALGETLISTENERF)GetProcAddress(hModOAL32, "alGetListenerf");
	_alGetListener3f = (LPALGETLISTENER3F)GetProcAddress(hModOAL32, "alGetListener3f");
	_alGetListenerfv = (LPALGETLISTENERFV)GetProcAddress(hModOAL32, "alGetListenerfv");
	_alGetListeneri = (LPALGETLISTENERI)GetProcAddress(hModOAL32, "alGetListeneri");
	_alGetListener3i = (LPALGETLISTENER3I)GetProcAddress(hModOAL32, "alGetListener3i");
	_alGetListeneriv = (LPALGETLISTENERIV)GetProcAddress(hModOAL32, "alGetListeneriv");
	_alGenSources = (LPALGENSOURCES)GetProcAddress(hModOAL32, "alGenSources");
	_alDeleteSources = (LPALDELETESOURCES)GetProcAddress(hModOAL32, "alDeleteSources");
	_alIsSource = (LPALISSOURCE)GetProcAddress(hModOAL32, "alIsSource");
	_alSourcef = (LPALSOURCEF)GetProcAddress(hModOAL32, "alSourcef");
	_alSource3f = (LPALSOURCE3F)GetProcAddress(hModOAL32, "alSource3f");
	_alSourcefv = (LPALSOURCEFV)GetProcAddress(hModOAL32, "alSourcefv");
	_alSourcei = (LPALSOURCEI)GetProcAddress(hModOAL32, "alSourcei");
	_alSource3i = (LPALSOURCE3I)GetProcAddress(hModOAL32, "alSource3i");
	_alSourceiv = (LPALSOURCEIV)GetProcAddress(hModOAL32, "alSourceiv");
	_alGetSourcef = (LPALGETSOURCEF)GetProcAddress(hModOAL32, "alGetSourcef");
	_alGetSource3f = (LPALGETSOURCE3F)GetProcAddress(hModOAL32, "alGetSource3f");
	_alGetSourcefv = (LPALGETSOURCEFV)GetProcAddress(hModOAL32, "alGetSourcefv");
	_alGetSourcei = (LPALGETSOURCEI)GetProcAddress(hModOAL32, "alGetSourcei");
	_alGetSource3i = (LPALGETSOURCE3I)GetProcAddress(hModOAL32, "alGetSource3i");
	_alGetSourceiv = (LPALGETSOURCEIV)GetProcAddress(hModOAL32, "alGetSourceiv");
	_alSourcePlayv = (LPALSOURCEPLAYV)GetProcAddress(hModOAL32, "alSourcePlayv");
	_alSourceStopv = (LPALSOURCESTOPV)GetProcAddress(hModOAL32, "alSourceStopv");
	_alSourceRewindv = (LPALSOURCEREWINDV)GetProcAddress(hModOAL32, "alSourceRewindv");
	_alSourcePausev = (LPALSOURCEPAUSEV)GetProcAddress(hModOAL32, "alSourcePausev");
	_alSourcePlay = (LPALSOURCEPLAY)GetProcAddress(hModOAL32, "alSourcePlay");
	_alSourceStop = (LPALSOURCESTOP)GetProcAddress(hModOAL32, "alSourceStop");
	_alSourceRewind = (LPALSOURCEREWIND)GetProcAddress(hModOAL32, "alSourceRewind");
	_alSourcePause = (LPALSOURCEPAUSE)GetProcAddress(hModOAL32, "alSourcePause");
	_alSourceQueueBuffers = (LPALSOURCEQUEUEBUFFERS)GetProcAddress(hModOAL32, "alSourceQueueBuffers");
	_alSourceUnqueueBuffers = (LPALSOURCEUNQUEUEBUFFERS)GetProcAddress(hModOAL32, "alSourceUnqueueBuffers");
	_alGenBuffers = (LPALGENBUFFERS)GetProcAddress(hModOAL32, "alGenBuffers");
	_alDeleteBuffers = (LPALDELETEBUFFERS)GetProcAddress(hModOAL32, "alDeleteBuffers");
	_alIsBuffer = (LPALISBUFFER)GetProcAddress(hModOAL32, "alIsBuffer");
	_alBufferData = (LPALBUFFERDATA)GetProcAddress(hModOAL32, "alBufferData");
	_alBufferf = (LPALBUFFERF)GetProcAddress(hModOAL32, "alBufferf");
	_alBuffer3f = (LPALBUFFER3F)GetProcAddress(hModOAL32, "alBuffer3f");
	_alBufferfv = (LPALBUFFERFV)GetProcAddress(hModOAL32, "alBufferfv");
	_alBufferi = (LPALBUFFERI)GetProcAddress(hModOAL32, "alBufferi");
	_alBuffer3i = (LPALBUFFER3I)GetProcAddress(hModOAL32, "alBuffer3i");
	_alBufferiv = (LPALBUFFERIV)GetProcAddress(hModOAL32, "alBufferiv");
	_alGetBufferf = (LPALGETBUFFERF)GetProcAddress(hModOAL32, "alGetBufferf");
	_alGetBuffer3f = (LPALGETBUFFER3F)GetProcAddress(hModOAL32, "alGetBuffer3f");
	_alGetBufferfv = (LPALGETBUFFERFV)GetProcAddress(hModOAL32, "alGetBufferfv");
	_alGetBufferi = (LPALGETBUFFERI)GetProcAddress(hModOAL32, "alGetBufferi");
	_alGetBuffer3i = (LPALGETBUFFER3I)GetProcAddress(hModOAL32, "alGetBuffer3i");
	_alGetBufferiv = (LPALGETBUFFERIV)GetProcAddress(hModOAL32, "alGetBufferiv");
	_alDopplerFactor = (LPALDOPPLERFACTOR)GetProcAddress(hModOAL32, "alDopplerFactor");
	_alDopplerVelocity = (LPALDOPPLERVELOCITY)GetProcAddress(hModOAL32, "alDopplerVelocity");
	_alSpeedOfSound = (LPALSPEEDOFSOUND)GetProcAddress(hModOAL32, "alSpeedOfSound");
	_alDistanceModel = (LPALDISTANCEMODEL)GetProcAddress(hModOAL32, "alDistanceModel");

	_alcCreateContext = (LPALCCREATECONTEXT)GetProcAddress(hModOAL32, "alcCreateContext");
	_alcMakeContextCurrent = (LPALCMAKECONTEXTCURRENT)GetProcAddress(hModOAL32, "alcMakeContextCurrent");
	_alcProcessContext = (LPALCPROCESSCONTEXT)GetProcAddress(hModOAL32, "alcProcessContext");
	_alcSuspendContext = (LPALCSUSPENDCONTEXT)GetProcAddress(hModOAL32, "alcSuspendContext");
	_alcDestroyContext = (LPALCDESTROYCONTEXT)GetProcAddress(hModOAL32, "alcDestroyContext");
	_alcGetCurrentContext = (LPALCGETCURRENTCONTEXT)GetProcAddress(hModOAL32, "alcGetCurrentContext");
	_alcGetContextsDevice = (LPALCGETCONTEXTSDEVICE)GetProcAddress(hModOAL32, "alcGetContextsDevice");
	_alcOpenDevice = (LPALCOPENDEVICE)GetProcAddress(hModOAL32, "alcOpenDevice");
	_alcCloseDevice = (LPALCCLOSEDEVICE)GetProcAddress(hModOAL32, "alcCloseDevice");
	_alcGetError = (LPALCGETERROR)GetProcAddress(hModOAL32, "alcGetError");
	_alcIsExtensionPresent = (LPALCISEXTENSIONPRESENT)GetProcAddress(hModOAL32, "alcIsExtensionPresent");
	_alcGetProcAddress = (LPALCGETPROCADDRESS)GetProcAddress(hModOAL32, "alcGetProcAddress");
	_alcGetEnumValue = (LPALCGETENUMVALUE)GetProcAddress(hModOAL32, "alcGetEnumValue");
	_alcGetString = (LPALCGETSTRING)GetProcAddress(hModOAL32, "alcGetString");
	_alcGetIntegerv = (LPALCGETINTEGERV)GetProcAddress(hModOAL32, "alcGetIntegerv");
	_alcCaptureOpenDevice = (LPALCCAPTUREOPENDEVICE)GetProcAddress(hModOAL32, "alcCaptureOpenDevice");
	_alcCaptureCloseDevice = (LPALCCAPTURECLOSEDEVICE)GetProcAddress(hModOAL32, "alcCaptureCloseDevice");
	_alcCaptureStart = (LPALCCAPTURESTART)GetProcAddress(hModOAL32, "alcCaptureStart");
	_alcCaptureStop = (LPALCCAPTURESTOP)GetProcAddress(hModOAL32, "alcCaptureStop");
	_alcCaptureSamples = (LPALCCAPTURESAMPLES)GetProcAddress(hModOAL32, "alcCaptureSamples");

	try {
		PID = GetCurrentProcessId();
		HANDLE hProc = OpenProcess(PROCESS_ALL_ACCESS, false, (DWORD)PID);
		if (hProc) {
			BaseAddress = (DWORD)GetModule(hProc);

			// Error checking
			if (BaseEndAddress == 0)
				throw std::exception("Unable to determine size of module");
			if (BaseAddress == 0)
				throw std::exception("Unable to determine base address of module");

			GetAddresses(hProc);

			Handler_Resource(hProc);
			Handler_Level(hProc);
			Handler_Plugin(hProc);
			Handler_GameSave(hProc);

			CloseHandle(hProc);
		}
		else {
			throw std::exception("Unable to install handlers into TY process (unable to OpenProcess)");
		}
	}
	catch (std::exception & e) {
		OutputDebugStringA(e.what());
	}

	hasLoadedLibrary = 1;
	return 1;
}

// Get Base Address of TY.exe
// from http://stackoverflow.com/a/31895513
HMODULE GetModule(HANDLE hProc)
{
	HMODULE hMods[1024];
	DWORD cbNeeded;
	MODULEINFO moduleInfo = { 0 };
	unsigned int i;

	if (EnumProcessModules(hProc, hMods, sizeof(hMods), &cbNeeded))
	{
		for (i = 0; i < (cbNeeded / sizeof(HMODULE)); i++)
		{
			TCHAR szModName[MAX_PATH];
			if (GetModuleFileNameEx(hProc, hMods[i], szModName, sizeof(szModName) / sizeof(TCHAR)))
			{
				std::wstring wstrModName = szModName;
				if (wstrModName.find(L"TY.exe") != std::string::npos)
				{
					if (GetModuleInformation(hProc, hMods[i], &moduleInfo, sizeof(moduleInfo)))
						BaseEndAddress = (uint64_t)moduleInfo.SizeOfImage + (uint64_t)hMods[i];
					
					return hMods[i];
				}
			}
		}
	}
	return nullptr;
}



AL_API void AL_APIENTRY alEnable(ALenum capability) {
	if (Init())
		_alEnable(capability);
}
AL_API void AL_APIENTRY alDisable(ALenum capability) {
	if (Init())
		_alDisable(capability);
}
AL_API ALboolean AL_APIENTRY alIsEnabled(ALenum capability) {
	if (Init())
		return _alIsEnabled(capability);
	return false;
}
AL_API const ALchar* AL_APIENTRY alGetString(ALenum param) {
	if (Init())
		return _alGetString(param);
	return NULL;
}
AL_API void AL_APIENTRY alGetBooleanv(ALenum param, ALboolean* data) {
	if (Init())
		_alGetBooleanv(param, data);
}
AL_API void AL_APIENTRY alGetIntegerv(ALenum param, ALint* data) {
	if (Init())
		_alGetIntegerv(param, data);
}
AL_API void AL_APIENTRY alGetFloatv(ALenum param, ALfloat* data) {
	if (Init())
		_alGetFloatv(param, data);
}
AL_API void AL_APIENTRY alGetDoublev(ALenum param, ALdouble* data) {
	if (Init())
		_alGetDoublev(param, data);
}
AL_API ALboolean AL_APIENTRY alGetBoolean(ALenum param) {
	if (Init())
		return _alGetBoolean(param);
	return false;
}
AL_API ALint AL_APIENTRY alGetInteger(ALenum param) {
	if (Init())
		return _alGetInteger(param);
	return 0;
}
AL_API ALfloat AL_APIENTRY alGetFloat(ALenum param) {
	if (Init())
		return _alGetFloat(param);
	return 0;
}
AL_API ALdouble AL_APIENTRY alGetDouble(ALenum param) {
	if (Init())
		return _alGetDouble(param);
	return 0;
}
AL_API ALenum AL_APIENTRY alGetError(void) {
	if (Init())
		return _alGetError();
	return 0;
}
AL_API ALboolean AL_APIENTRY alIsExtensionPresent(const ALchar* extname) {
	if (Init())
		return _alIsExtensionPresent(extname);
	return false;
}
AL_API void* AL_APIENTRY alGetProcAddress(const ALchar* fname) {
	if (Init())
		return _alGetProcAddress(fname);
	return NULL;
}
AL_API ALenum AL_APIENTRY alGetEnumValue(const ALchar* ename) {
	if (Init())
		return _alGetEnumValue(ename);
	return 0;
}
AL_API void AL_APIENTRY alListenerf(ALenum param, ALfloat value) {
	if (Init())
		_alListenerf(param, value);
}
AL_API void AL_APIENTRY alListener3f(ALenum param, ALfloat value1, ALfloat value2, ALfloat value3) {
	if (Init())
		_alListener3f(param, value1, value2, value3);
}
AL_API void AL_APIENTRY alListenerfv(ALenum param, const ALfloat* values) {
	if (Init())
		_alListenerfv(param, values);
}
AL_API void AL_APIENTRY alListeneri(ALenum param, ALint value) {
	if (Init())
		_alListeneri(param, value);
}
AL_API void AL_APIENTRY alListener3i(ALenum param, ALint value1, ALint value2, ALint value3) {
	if (Init())
		_alListener3i(param, value1, value2, value3);
}
AL_API void AL_APIENTRY alListeneriv(ALenum param, const ALint* values) {
	if (Init())
		_alListeneriv(param, values);
}
AL_API void AL_APIENTRY alGetListenerf(ALenum param, ALfloat* value) {
	if (Init())
		_alGetListenerf(param, value);
}
AL_API void AL_APIENTRY alGetListener3f(ALenum param, ALfloat *value1, ALfloat *value2, ALfloat *value3) {
	if (Init())
		_alGetListener3f(param, value1, value2, value3);
}
AL_API void AL_APIENTRY alGetListenerfv(ALenum param, ALfloat* values) {
	if (Init())
		_alGetListenerfv(param, values);
}
AL_API void AL_APIENTRY alGetListeneri(ALenum param, ALint* value) {
	if (Init())
		_alGetListeneri(param, value);
}
AL_API void AL_APIENTRY alGetListener3i(ALenum param, ALint *value1, ALint *value2, ALint *value3) {
	if (Init())
		_alGetListener3i(param, value1, value2, value3);
}
AL_API void AL_APIENTRY alGetListeneriv(ALenum param, ALint* values) {
	if (Init())
		_alGetListeneriv(param, values);
}
AL_API void AL_APIENTRY alGenSources(ALsizei n, ALuint* sources) {
	if (Init())
		_alGenSources(n, sources);
}
AL_API void AL_APIENTRY alDeleteSources(ALsizei n, const ALuint* sources) {
	if (Init())
		_alDeleteSources(n, sources);
}
AL_API ALboolean AL_APIENTRY alIsSource(ALuint sid) {
	if (Init())
		return _alIsSource(sid);
	return false;
}
AL_API void AL_APIENTRY alSourcef(ALuint sid, ALenum param, ALfloat value) {
	if (Init())
		_alSourcef(sid, param, value);
}
AL_API void AL_APIENTRY alSource3f(ALuint sid, ALenum param, ALfloat value1, ALfloat value2, ALfloat value3) {
	if (Init())
		_alSource3f(sid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alSourcefv(ALuint sid, ALenum param, const ALfloat* values) {
	if (Init())
		_alSourcefv(sid, param, values);
}
AL_API void AL_APIENTRY alSourcei(ALuint sid, ALenum param, ALint value) {
	if (Init())
		_alSourcei(sid, param, value);
}
AL_API void AL_APIENTRY alSource3i(ALuint sid, ALenum param, ALint value1, ALint value2, ALint value3) {
	if (Init())
		_alSource3i(sid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alSourceiv(ALuint sid, ALenum param, const ALint* values) {
	if (Init())
		_alSourceiv(sid, param, values);
}
AL_API void AL_APIENTRY alGetSourcef(ALuint sid, ALenum param, ALfloat* value) {
	if (Init())
		_alGetSourcef(sid, param, value);
}
AL_API void AL_APIENTRY alGetSource3f(ALuint sid, ALenum param, ALfloat* value1, ALfloat* value2, ALfloat* value3) {
	if (Init())
		_alGetSource3f(sid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alGetSourcefv(ALuint sid, ALenum param, ALfloat* values) {
	if (Init())
		_alGetSourcefv(sid, param, values);
}
AL_API void AL_APIENTRY alGetSourcei(ALuint sid, ALenum param, ALint* value) {
	if (Init())
		_alGetSourcei(sid, param, value);
}
AL_API void AL_APIENTRY alGetSource3i(ALuint sid, ALenum param, ALint* value1, ALint* value2, ALint* value3) {
	if (Init())
		_alGetSource3i(sid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alGetSourceiv(ALuint sid, ALenum param, ALint* values) {
	if (Init())
		_alGetSourceiv(sid, param, values);
}
AL_API void AL_APIENTRY alSourcePlayv(ALsizei ns, const ALuint *sids) {
	if (Init())
		_alSourcePlayv(ns, sids);
}
AL_API void AL_APIENTRY alSourceStopv(ALsizei ns, const ALuint *sids) {
	if (Init())
		_alSourceStopv(ns, sids);
}
AL_API void AL_APIENTRY alSourceRewindv(ALsizei ns, const ALuint *sids) {
	if (Init())
		_alSourceRewindv(ns, sids);
}
AL_API void AL_APIENTRY alSourcePausev(ALsizei ns, const ALuint *sids) {
	if (Init())
		_alSourcePausev(ns, sids);
}
AL_API void AL_APIENTRY alSourcePlay(ALuint sid) {
	if (Init())
		_alSourcePlay(sid);
}
AL_API void AL_APIENTRY alSourceStop(ALuint sid) {
	if (Init())
		_alSourceStop(sid);
}
AL_API void AL_APIENTRY alSourceRewind(ALuint sid) {
	if (Init())
		_alSourceRewind(sid);
}
AL_API void AL_APIENTRY alSourcePause(ALuint sid) {
	if (Init())
		_alSourcePause(sid);
}
AL_API void AL_APIENTRY alSourceQueueBuffers(ALuint sid, ALsizei numEntries, const ALuint *bids) {
	if (Init())
		_alSourceQueueBuffers(sid, numEntries, bids);
}
AL_API void AL_APIENTRY alSourceUnqueueBuffers(ALuint sid, ALsizei numEntries, ALuint *bids) {
	if (Init())
		_alSourceUnqueueBuffers(sid, numEntries, bids);
}
AL_API void AL_APIENTRY alGenBuffers(ALsizei n, ALuint* buffers) {
	if (Init())
		_alGenBuffers(n, buffers);
}
AL_API void AL_APIENTRY alDeleteBuffers(ALsizei n, const ALuint* buffers) {
	if (Init())
		_alDeleteBuffers(n, buffers);
}
AL_API ALboolean AL_APIENTRY alIsBuffer(ALuint bid) {
	if (Init())
		return _alIsBuffer(bid);
	return false;
}
AL_API void AL_APIENTRY alBufferData(ALuint bid, ALenum format, const ALvoid* data, ALsizei size, ALsizei freq) {
	if (Init())
		_alBufferData(bid, format, data, size, freq);
}
AL_API void AL_APIENTRY alBufferf(ALuint bid, ALenum param, ALfloat value) {
	if (Init())
		_alBufferf(bid, param, value);
}
AL_API void AL_APIENTRY alBuffer3f(ALuint bid, ALenum param, ALfloat value1, ALfloat value2, ALfloat value3) {
	if (Init())
		_alBuffer3f(bid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alBufferfv(ALuint bid, ALenum param, const ALfloat* values) {
	if (Init())
		_alBufferfv(bid, param, values);
}
AL_API void AL_APIENTRY alBufferi(ALuint bid, ALenum param, ALint value) {
	if (Init())
		_alBufferi(bid, param, value);
}
AL_API void AL_APIENTRY alBuffer3i(ALuint bid, ALenum param, ALint value1, ALint value2, ALint value3) {
	if (Init())
		_alBuffer3i(bid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alBufferiv(ALuint bid, ALenum param, const ALint* values) {
	if (Init())
		_alBufferiv(bid, param, values);
}
AL_API void AL_APIENTRY alGetBufferf(ALuint bid, ALenum param, ALfloat* value) {
	if (Init())
		_alGetBufferf(bid, param, value);
}
AL_API void AL_APIENTRY alGetBuffer3f(ALuint bid, ALenum param, ALfloat* value1, ALfloat* value2, ALfloat* value3) {
	if (Init())
		_alGetBuffer3f(bid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alGetBufferfv(ALuint bid, ALenum param, ALfloat* values) {
	if (Init())
		_alGetBufferfv(bid, param, values);
}
AL_API void AL_APIENTRY alGetBufferi(ALuint bid, ALenum param, ALint* value) {
	if (Init())
		_alGetBufferi(bid, param, value);
}
AL_API void AL_APIENTRY alGetBuffer3i(ALuint bid, ALenum param, ALint* value1, ALint* value2, ALint* value3) {
	if (Init())
		_alGetBuffer3i(bid, param, value1, value2, value3);
}
AL_API void AL_APIENTRY alGetBufferiv(ALuint bid, ALenum param, ALint* values) {
	if (Init())
		_alGetBufferiv(bid, param, values);
}
AL_API void AL_APIENTRY alDopplerFactor(ALfloat value) {
	if (Init())
		_alDopplerFactor(value);
}
AL_API void AL_APIENTRY alDopplerVelocity(ALfloat value) {
	if (Init())
		_alDopplerVelocity(value);
}
AL_API void AL_APIENTRY alSpeedOfSound(ALfloat value) {
	if (Init())
		_alSpeedOfSound(value);
}
AL_API void AL_APIENTRY alDistanceModel(ALenum distanceModel) {
	if (Init())
		_alDistanceModel(distanceModel);
}

ALC_API ALCcontext *    ALC_APIENTRY alcCreateContext(ALCdevice *device, const ALCint* attrlist) {
	if (Init())
		return _alcCreateContext(device, attrlist);
	return NULL;
}
ALC_API ALCboolean      ALC_APIENTRY alcMakeContextCurrent(ALCcontext *context) {
	if (Init())
		return _alcMakeContextCurrent(context);
	return false;
}
ALC_API void            ALC_APIENTRY alcProcessContext(ALCcontext *context) {
	if (Init())
		_alcProcessContext(context);
}
ALC_API void            ALC_APIENTRY alcSuspendContext(ALCcontext *context) {
	if (Init())
		_alcSuspendContext(context);
}
ALC_API void            ALC_APIENTRY alcDestroyContext(ALCcontext *context) {
	if (Init())
		_alcDestroyContext(context);
}
ALC_API ALCcontext *    ALC_APIENTRY alcGetCurrentContext(void) {
	if (Init())
		return _alcGetCurrentContext();
	return NULL;
}
ALC_API ALCdevice*      ALC_APIENTRY alcGetContextsDevice(ALCcontext *context) {
	if (Init())
		return _alcGetContextsDevice(context);
	return NULL;
}
ALC_API ALCdevice *     ALC_APIENTRY alcOpenDevice(const ALCchar *devicename) {
	if (Init())
		return _alcOpenDevice(devicename);
	return NULL;
}
ALC_API ALCboolean      ALC_APIENTRY alcCloseDevice(ALCdevice *device) {
	if (Init())
		return _alcCloseDevice(device);
	return false;
}
ALC_API ALCenum         ALC_APIENTRY alcGetError(ALCdevice *device) {
	if (Init())
		return _alcGetError(device);
	return 0;
}
ALC_API ALCboolean      ALC_APIENTRY alcIsExtensionPresent(ALCdevice *device, const ALCchar *extname) {
	if (Init())
		return _alcIsExtensionPresent(device, extname);
	return false;
}
ALC_API void  *         ALC_APIENTRY alcGetProcAddress(ALCdevice *device, const ALCchar *funcname) {
	if (Init())
		return _alcGetProcAddress(device, funcname);
	return NULL;
}
ALC_API ALCenum         ALC_APIENTRY alcGetEnumValue(ALCdevice *device, const ALCchar *enumname) {
	if (Init())
		return _alcGetEnumValue(device, enumname);
	return 0;
}
ALC_API const ALCchar * ALC_APIENTRY alcGetString(ALCdevice *device, ALCenum param) {
	if (Init())
		return _alcGetString(device, param);
	return NULL;
}
ALC_API void            ALC_APIENTRY alcGetIntegerv(ALCdevice *device, ALCenum param, ALCsizei size, ALCint *data) {
	if (Init())
		_alcGetIntegerv(device, param, size, data);
}
ALC_API ALCdevice*      ALC_APIENTRY alcCaptureOpenDevice(const ALCchar *devicename, ALCuint frequency, ALCenum format, ALCsizei buffersize) {
	if (Init())
		return _alcCaptureOpenDevice(devicename, frequency, format, buffersize);
	return NULL;
}
ALC_API ALCboolean      ALC_APIENTRY alcCaptureCloseDevice(ALCdevice *device) {
	if (Init())
		return _alcCaptureCloseDevice(device);
	return false;
}
ALC_API void            ALC_APIENTRY alcCaptureStart(ALCdevice *device) {
	if (Init())
		_alcCaptureStart(device);
}
ALC_API void            ALC_APIENTRY alcCaptureStop(ALCdevice *device) {
	if (Init())
		_alcCaptureStop(device);
}
ALC_API void            ALC_APIENTRY alcCaptureSamples(ALCdevice *device, ALCvoid *buffer, ALCsizei samples) {
	if (Init())
		_alcCaptureSamples(device, buffer, samples);
}
