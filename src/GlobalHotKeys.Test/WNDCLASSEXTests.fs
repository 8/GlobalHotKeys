module GlobalHotKeys.Test.WNDCLASSEXTests

open System
open GlobalHotKeys.Native.Functions
open GlobalHotKeys.Native.Types
open GlobalHotKeys.Native.WNDCLASSEX
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``init test`` () =
  
  let messageHandler hWnd uMsg wParam lParam = nativeint 0
  
  let hInstance = nativeint 3
  let className = "class"
  let wndProc = WndProc(messageHandler)
  let result = init hInstance className wndProc

  result.hInstance |> should equal hInstance
  result.cbSize |> should not' (equal 0)
  result.lpfnWndProc |> should not' (be Null)
  result.lpszClassName |> should equal className
  ()