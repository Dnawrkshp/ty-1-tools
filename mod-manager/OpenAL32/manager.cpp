
#include "globals.h"
#include <cstdint>
#include <thread>
#include <string>

#include <Windows.h>
#include <TlHelp32.h>

#include "imgui.h"
#include "imgui_impl_win32.h"
#include "imgui_impl_opengl2.h"

#if defined(__APPLE__)
#define GL_SILENCE_DEPRECATION
#include <OpenGL/gl.h>
#else
#include <GL/gl.h>
#endif

typedef bool* (*SWAPBUFFERS) (void);
typedef bool* (*HANDLEINPUT) (void);
typedef bool* (*SETCURSOR) (void);
typedef LRESULT(CALLBACK* WndProc_t) (HWND, UINT, WPARAM, LPARAM);

// Data
static bool hasInitializedIMGUI = false;
static bool ContextCreated = false;
static bool showMenu = false;
static HGLRC g_Context = (HGLRC)NULL;
static WndProc_t oWndProc = NULL;

static HANDLEINPUT TY_SETCURSOR = NULL;
static HANDLEINPUT TY_HANDLEINPUT = NULL;
static SWAPBUFFERS TY_SWAPBUFFERS = NULL;
static HDC* TY_HDC = NULL;
extern LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

LRESULT CALLBACK WndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	switch (uMsg)
	{
		case WM_KEYDOWN:
			switch (wParam)
			{
				case VK_INSERT:
					showMenu = !showMenu;
					break;
			}
	}

	if (showMenu && hasInitializedIMGUI)
	{
		if (ImGui_ImplWin32_WndProcHandler(hWnd, uMsg, wParam, lParam))
			return true;
	}

	return CallWindowProc(oWndProc, hWnd, uMsg, wParam, lParam);
}

void ManagerInitIMGUI(HDC hdc)
{
    auto hwnd = WindowFromDC(hdc); //GetActiveWindow();
    if (!hwnd)
    {

        return;
    }

    IMGUI_CHECKVERSION();
    ImGui::CreateContext();
    ImGuiIO& io = ImGui::GetIO();
	io.ConfigFlags |= ImGuiConfigFlags_NavEnableKeyboard;
    ImGui::StyleColorsDark();

    ImGui_ImplWin32_Init(hwnd);
    ImGui_ImplOpenGL2_Init();


    hasInitializedIMGUI = true;
}

void ManagerDrawIMGUI(HDC hdc)
{
	auto hWindow = WindowFromDC(hdc);
	if (!hWindow)
		return;
	HGLRC oContext = wglGetCurrentContext();

	if (!oWndProc)
		oWndProc = (WndProc_t)SetWindowLongPtr(hWindow, GWL_WNDPROC, (LONG_PTR)WndProc);

	if (!ContextCreated)
	{
		g_Context = wglCreateContext(hdc);
		wglMakeCurrent(hdc, g_Context);
		glMatrixMode(GL_PROJECTION);
		glLoadIdentity();

		GLint m_viewport[4];
		glGetIntegerv(GL_VIEWPORT, m_viewport);

		glOrtho(0, m_viewport[2], m_viewport[3], 0, 1, -1);
		glMatrixMode(GL_MODELVIEW);
		glLoadIdentity();
		glClearColor(0, 0, 0, 0);
		ContextCreated = true;
	}

	wglMakeCurrent(hdc, g_Context);
    if (!hasInitializedIMGUI)
    {
		if (ContextCreated)
			ManagerInitIMGUI(hdc);
    }

    ImVec4 clear_color = ImVec4(0.0f, 0.0f, 0.0f, 0.0f);

	if (showMenu)
	{
		ImGuiIO& io = ImGui::GetIO();
		io.MouseDrawCursor = true;

		// Start the Dear ImGui frame
		ImGui_ImplOpenGL2_NewFrame();
		ImGui_ImplWin32_NewFrame();
		ImGui::NewFrame();

		// 2. Show a simple window that we create ourselves. We use a Begin/End pair to created a named window.
		{
			static float f = 0.0f;
			static int counter = 0;
			static char textInputBuf[128];

			ImGui::Begin("Hello, world!");                          // Create a window called "Hello, world!" and append into it.

			ImGui::Text("This is some useful text.");               // Display some text (you can use a format strings too)

			ImGui::InputTextMultiline("Label", textInputBuf, sizeof(textInputBuf));

			ImGui::SliderFloat("float", &f, 0.0f, 1.0f);            // Edit 1 float using a slider from 0.0f to 1.0f
			ImGui::ColorEdit3("clear color", (float*)&clear_color); // Edit 3 floats representing a color

			if (ImGui::Button("Button"))                            // Buttons return true when clicked (most widgets return true when edited/activated)
				counter++;
			ImGui::SameLine();
			ImGui::Text("counter = %d", counter);

			ImGui::Text("Application average %.3f ms/frame (%.1f FPS)", 1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
			ImGui::End();
		}

		// Rendering
		ImGui::EndFrame();
		ImGui::Render();
		ImGui_ImplOpenGL2_RenderDrawData(ImGui::GetDrawData());
	}

	wglMakeCurrent(hdc, oContext);
}

void MySetCursor(void)
{
	if (showMenu && hasInitializedIMGUI)
		return;

	if (TY_SETCURSOR)
		TY_SETCURSOR();
}

void MyHandleInput(void)
{
	if (showMenu && hasInitializedIMGUI)
		return;

	if (TY_HANDLEINPUT)
		TY_HANDLEINPUT();
}

bool MySwapBuffers(void)
{
	ManagerDrawIMGUI(*TY_HDC);

    return TY_SWAPBUFFERS();
}

void Handler_Manager(void* hProc)
{
	uint32_t addr = BaseAddress + 0x19E035;
	uint32_t patch = (uint32_t)(((uint32_t)&MySwapBuffers - addr) - 5);

	// here we patch the swap buffer call with our own
	// this way we can render additional stuff on top of the game
	TY_SWAPBUFFERS = (SWAPBUFFERS)(*(uint32_t*)(addr + 1) + (addr)+5);
	TY_HDC = (HDC*)(*(uint32_t*)((uint32_t)TY_SWAPBUFFERS + 2));

	// write patch
	WriteProcessMemory(hProc, (LPVOID)(addr + 1), &patch, (DWORD)4, NULL);

	// patch game's input handler with our wrapper
	addr = BaseAddress + 0x19E7D1;
	patch = (uint32_t)(((uint32_t)&MyHandleInput - addr) - 5);
	TY_HANDLEINPUT = (HANDLEINPUT)(*(uint32_t*)(addr + 1) + (addr)+5);

	// write patch
	WriteProcessMemory(hProc, (LPVOID)(addr + 1), &patch, (DWORD)4, NULL);

	// patch game's input handler with our wrapper
	addr = BaseAddress + 0x19DE4A;
	patch = (uint32_t)(((uint32_t)&MySetCursor - addr) - 5);
	TY_SETCURSOR = (SETCURSOR)(*(uint32_t*)(addr + 1) + (addr)+5);

	// write patch
	WriteProcessMemory(hProc, (LPVOID)(addr + 1), &patch, (DWORD)4, NULL);
}
