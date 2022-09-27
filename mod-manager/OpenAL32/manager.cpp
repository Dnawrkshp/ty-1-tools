
#include "globals.h"
#include <cstdint>
#include <thread>
#include <string>

#include <Windows.h>
#include <TlHelp32.h>

#include "imgui.h"
#include "imgui_impl_win32.h"
#include "imgui_impl_opengl2.h"


// Data
static bool hasInitializedIMGUI = false;


typedef bool* (*SWAPBUFFERS) (HDC arg1);
typedef void(__stdcall* f_wglSwapBuffers)(HDC h);

static SWAPBUFFERS TY_SWAPBUFFERS = NULL;

void ManagerInitIMGUI()
{
    auto hwnd = GetActiveWindow();
    if (!hwnd)
    {

        return;
    }

    IMGUI_CHECKVERSION();
    ImGui::CreateContext();
    ImGuiIO& io = ImGui::GetIO();

    ImGui::StyleColorsDark();

    ImGui_ImplWin32_Init(hwnd);
    ImGui_ImplOpenGL2_Init();


    hasInitializedIMGUI = true;
}

void ManagerDrawIMGUI()
{
    if (!hasInitializedIMGUI)
    {
        ManagerInitIMGUI();
        return;
    }

    ImVec4 clear_color = ImVec4(0.0f, 0.0f, 0.0f, 0.0f);

    // Start the Dear ImGui frame
    ImGui_ImplOpenGL2_NewFrame();
    ImGui_ImplWin32_NewFrame();
    ImGui::NewFrame();

    // 2. Show a simple window that we create ourselves. We use a Begin/End pair to created a named window.
    {
        static float f = 0.0f;
        static int counter = 0;

        ImGui::Begin("Hello, world!");                          // Create a window called "Hello, world!" and append into it.

        ImGui::Text("This is some useful text.");               // Display some text (you can use a format strings too)

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
    ImGui::Render();
    ImGui_ImplOpenGL2_RenderDrawData(ImGui::GetDrawData());
}

bool MySwapBuffers(HDC arg1)
{
    bool result = TY_SWAPBUFFERS(arg1);
    ManagerDrawIMGUI();


    return result;
}

void Handler_Manager(void* hProc)
{
    HMODULE hOpenglModule = GetModuleHandle(L"opengl32.dll");

    if (hOpenglModule == NULL)
    {
        OutputDebugStringA("Failed to find opengl32.dll::wglSwapBuffers");
        return;
    }

    auto dWglSwapBuffers = GetProcAddress(hOpenglModule, "wglSwapBuffers");
    f_wglSwapBuffers wglSwapBuffers = reinterpret_cast<f_wglSwapBuffers>(dWglSwapBuffers);

    //if (d_wglSwapBuffers.Hook(wglSwapBuffers, &Hook::WglSwapBuffers::h_wglSwapBuffers, blackbone::HookType::HWBP))
    //{
    //    OutputDebugStringA("Hooked 'wglSwapBuffers'");
    //    return;
    //}
    //else
    //{
    //    OutputDebugStringA("Failed to Hook: wglSwapBuffers");
    //    return;
    //}


    //return;
	// Create new thread to run independently
	// When Ty exits, Windows should terminate the thread
	//std::thread task(ManagerLoop);
	//task.detach();

    uint32_t addr = BaseAddress + 0x19E035;
    uint32_t patch = (uint32_t)(((uint32_t)&MySwapBuffers - addr) - 5);

    // here we patch the swap buffer call with our own
    // this way we can render additional stuff on top of the game
    TY_SWAPBUFFERS = (SWAPBUFFERS)(*(uint32_t*)(addr + 1) + (addr) + 5);

    // write patch
    WriteProcessMemory(hProc, (LPVOID)(addr + 1), &patch, (DWORD)4, NULL);
}
