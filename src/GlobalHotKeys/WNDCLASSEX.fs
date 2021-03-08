module GlobalHotKeys.Native.WNDCLASSEX

open System
open System.Runtime.InteropServices
open GlobalHotKeys.Native.Functions
open GlobalHotKeys.Native.Types

let init hInstance className wndProc =
  let mutable i = WNDCLASSEX ()
  i.cbSize <- Marshal.SizeOf<WNDCLASSEX> ()
  i.hInstance <- hInstance
  i.lpfnWndProc <- wndProc
  i.lpszClassName <- className
  i

let fromWndProc =
  let hInstance = GetModuleHandle(null)
  let className = $"{nameof GlobalHotKeys}-{Guid.NewGuid():N}"
  init hInstance className
