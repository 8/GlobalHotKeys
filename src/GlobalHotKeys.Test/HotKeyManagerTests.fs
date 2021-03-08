module GlobalHotKeys.Test.HotKeyManagerTests

open System.Threading.Tasks
open GlobalHotKeys.Native.Types
open Xunit
open FsUnit.Xunit
open GlobalHotKeys

[<Fact>]
let ``create and dispose HotKeyManager`` () =
  use manager = new HotKeyManager ()
  ()
  
  
[<Fact>]
let ``register 2 keys`` () =
  
  // arrange
  use manager = new HotKeyManager ()
  
  // act
  use s1 = manager.Register (VirtualKeyCode.KEY_0, Modifiers.Shift)
  use s2 = manager.Register (VirtualKeyCode.KEY_1, Modifiers.Shift)
  
  // assert
  s1.Id |> should equal 0
  s2.Id |> should equal 1
  ()

[<Fact>]
let ``register reuses ids`` () =
  
  // arrange
  use manager = new HotKeyManager ()
  
  // act
  use s1 = manager.Register (VirtualKeyCode.KEY_0, Modifiers.Shift)
  s1.Dispose ()
  use s2 = manager.Register (VirtualKeyCode.KEY_1, Modifiers.Shift)
  
  // assert
  s1.Id |> should equal 0
  s2.Id |> should equal 0
  ()
  
  
[<Fact
  (Skip = "manual test - you need to press the registered key on the keyboard")
  >]
let ``manual hotkey test`` () =
  
  // arrange
  use manager = new HotKeyManager ()
  let tcs = TaskCompletionSource<bool> ()
  let hotkeyHandler hotkey =
    printfn $"hotkey pressed: Id = {hotkey.Id}, Key = {hotkey.Key}, Modifers = {hotkey.Modifiers}"
    tcs.TrySetResult(true) |> ignore
    ()
  manager.HotKeyPressed.Subscribe hotkeyHandler
  |> ignore

  // act
  use registeredHotKey = manager.Register (VirtualKeyCode.KEY_1, Modifiers.Shift)
  printfn ($"hotkey registered, please press SHIFT-1")
  System.Threading.Thread.Sleep (System.TimeSpan.FromSeconds 10.)
  
  // assert
  tcs.Task.IsCompleted |> should be True
  
  ()
  
