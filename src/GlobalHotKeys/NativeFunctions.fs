module GlobalHotKeys.Native.Functions

open System
open System.Runtime.InteropServices
open GlobalHotKeys.Native.Types

[<Literal>]
let private Kernel32 = "Kernel32"

[<Literal>]
let private User32 = "User32"

[<DllImport(Kernel32, SetLastError = true, CharSet = CharSet.Unicode)>]
extern IntPtr GetModuleHandle(string lpModuleName);

[<DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")>]
extern uint16 RegisterClassEx(WNDCLASSEX& lpwcx)

[<DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint ="UnregisterClassW")>]
extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

[<DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW")>]
extern IntPtr CreateWindowEx(int dwExStyle, uint lpClassName, string lpWindowName, WindowStyle dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam)

[<DllImport(User32, SetLastError = true)>]
extern bool DestroyWindow(IntPtr hwnd)

[<DllImport(User32)>]
extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)

[<DllImport(User32, SetLastError = true)>]
extern bool RegisterHotKey(IntPtr hWnd, int id, Modifiers fsModifiers, VirtualKeyCode vk)

[<DllImport(User32, SetLastError = true)>]
extern bool UnregisterHotKey(IntPtr hWnd, int id)

[<DllImport(User32, SetLastError = true)>]
extern int GetMessage(tagMSG& lpMsg, IntPtr hwnd, uint32 wMsgFilterMin, uint32 wMsgFilterMax)

[<DllImport(User32, SetLastError = true)>]
extern bool PostMessage(IntPtr hWnd, uint32 Msg, IntPtr wParam, IntPtr lParam)

[<DllImport(User32, SetLastError = true)>]
extern bool TranslateMessage(tagMSG& lpMsg)

[<DllImport(User32, SetLastError = true)>]
extern IntPtr DispatchMessage(tagMSG& lpMsg)

[<DllImport(User32, SetLastError = true)>]
extern IntPtr SendMessage(IntPtr hWnd, uint32 Msg, IntPtr wParam, IntPtr lParam)