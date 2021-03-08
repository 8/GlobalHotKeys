module GlobalHotKeys.Test.NativeFunctionsTest

open System
open Xunit
open FsUnit.Xunit
open GlobalHotKeys.Native.Functions
open GlobalHotKeys.Native.Types

[<Fact>]
let ``RegisterClassEx`` () =

  let handler hWnd uMsg wParam lParam =
    nativeint 0
  
  let wndProc = WndProc(handler)
  ()

[<Fact>]
let ``PostMessage & GetMessage Test`` () =
  
  // arrange
  let uMsg = uint32 WindowMessage.WM_HOTKEY
  let wParam = nativeint 2
  let lParam = nativeint 3
  let postResult = PostMessage (IntPtr.Zero, uMsg, wParam, lParam)
  let mutable tagMSG = tagMSG ()
  
  // act
  let result = GetMessage(&tagMSG, IntPtr.Zero, 0u, 0u)

  // assert  
  result |> should be (greaterThan 0)
  tagMSG.message |> should equal uMsg
  tagMSG.wParam |> should equal wParam
  tagMSG.lParam |> should equal lParam
  ()
