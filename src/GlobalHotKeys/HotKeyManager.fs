namespace GlobalHotKeys

open System
open System.Collections.Generic
open System.Reactive.Linq
open System.Reactive.Subjects
open System.Threading
open System.Threading.Tasks
open GlobalHotKeys.Native
open GlobalHotKeys.Native.Types
open Native.Functions

[<AutoOpen>]
module DictionaryExtension = 
  type Dictionary<'TKey, 'Value> with
    member this.Get(key : 'TKey) =
      this.TryGetValue (key)
      |> function
        | (true, value) -> Some value
        | _ -> None

type HotKey = {
  Id : int
  Modifiers : Modifiers
  Key : VirtualKeyCode
}

type IRegistration =
  interface
    inherit IDisposable
    abstract IsSuccessful : bool
    abstract Id : int
  end

type HotKeyManager() =
  class

    let hotkey = new Subject<HotKey> ()
    
    [<Literal>]
    let HotKeyMsg = 0x312u
      
    [<Literal>]
    let RegisterHotKeyMsg = 0x0400u // WM_USER
    
    [<Literal>]
    let UnregisterHotKeyMsg = 0x0401u // WM_USER + 1
    
    let messageLoopThreadHwnd =
      
      let tcsHwnd = TaskCompletionSource<IntPtr> ()
      
      let threadEntry () =
        let hInstance = GetModuleHandle(null)

        let registrations = Dictionary<int, HotKey> ()
        
        let nextId () =
          [0x0000..0xBFFF]
          |> List.tryFind (registrations.ContainsKey >> not)
        
        let register hWnd key modifiers id =
          match RegisterHotKey (hWnd, id, modifiers, key) with
          | true ->
              registrations.Add (id, { Id = id; Key = key; Modifiers = modifiers })
              true
          | false -> false
          
        let unregister hWnd id =
          registrations.Get(id)
          |> Option.map (fun registration -> UnregisterHotKey (hWnd, registration.Id))
          |> function
            | Some true ->
                registrations.Remove id |> ignore
                true
            | _ -> false

        let messageHandler hWnd (uMsg : uint) (wParam : nativeint) (lParam : nativeint) =
          match uMsg with
          | RegisterHotKeyMsg ->
            let key =  wParam |> int |> enum<VirtualKeyCode>
            let modifiers = lParam |> int |> enum<Modifiers>
            let id = nextId ()
            match id with
            | Some id -> register hWnd key modifiers id |> function true -> nativeint id | false -> nativeint -1
            | None -> IntPtr.Zero
              
          | UnregisterHotKeyMsg ->
            let id = wParam |> int
            unregister hWnd id |> function true -> nativeint id | false -> nativeint -1

          | HotKeyMsg ->
            registrations.Get (int wParam) |> Option.iter hotkey.OnNext
            nativeint 1
          | _ -> DefWindowProc(hWnd, uMsg, wParam, lParam)

        let mutable wndClassEx =
          WndProc(messageHandler) |> WNDCLASSEX.fromWndProc
        
        let registeredClass = RegisterClassEx(&wndClassEx)

        let messageLoop hWnd =
          let mutable msg = tagMSG ()
          let mutable ret = 0
          while (ret <- GetMessage (&msg, hWnd, 0u, 0u); ret <> -1 && ret <> 0) do
            TranslateMessage (&msg) |> ignore
            DispatchMessage (&msg) |> ignore

        let cleanup hWnd =
          registrations.Keys
          |> Seq.toArray
          |> Array.map (unregister hWnd)
          |> ignore
          
          DestroyWindow (hWnd)
          |> ignore
          
          UnregisterClass(wndClassEx.lpszClassName, hInstance)
          |> ignore
        
        // create the window
        let hWnd = CreateWindowEx(0, uint registeredClass, null, WindowStyle.WS_OVERLAPPED, 0, 0, 640, 480, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
        
        hWnd |> tcsHwnd.SetResult
      
        // enter message loop
        hWnd |> messageLoop
      
        // cleanup the resources afterwards
        hWnd |> cleanup
        
      let thread =
        let thread = Thread(ThreadStart(threadEntry))
        thread.Name <- "GlobalHotKeyManager Message Loop"
        thread.Start()
        thread
      
      (thread, tcsHwnd.Task.Result)

    member this.Register (key : VirtualKeyCode, modifiers : Modifiers) =
      let _, hWnd = messageLoopThreadHwnd
      
      // tell the message loop to register the hotkey
      let result = SendMessage (hWnd, RegisterHotKeyMsg, nativeint key, nativeint modifiers)
      
      // return a disposable that instructs the message loop to unregister the hotkey on disposal
      {
        new IRegistration with
          member this.IsSuccessful
            with get () =
              match int result with
              | -1 -> false
              | _ -> true
          member this.Id
            with get () = int result
        interface IDisposable with
          member this.Dispose () =
            match int result with
            | -1 -> ()
            | id -> SendMessage (hWnd, UnregisterHotKeyMsg, nativeint id, IntPtr.Zero) |> ignore
      }
      
    member this.HotKeyPressed
      with get () = hotkey.AsObservable()
   
    member this.Dispose () =
      let thread, hWnd = messageLoopThreadHwnd
        
      // shutdown the message loop
      PostMessage(hWnd, uint32 WindowMessage.WM_QUIT, IntPtr.Zero, IntPtr.Zero)
      |> ignore

      // wait for the shutdown
      thread.Join()

    interface IDisposable with
      member this.Dispose () = this.Dispose ()

  end

