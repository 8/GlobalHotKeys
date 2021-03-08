# README
GlobalHotKeys is a tiny .NET Library registering global HotKeys on Windows, written by Martin Kramer (https://lostindetails.com)

The library allows an application to react to Key Press events even if the application does not currently have focus.

## Example Usage
Please take a look at the examples in the `src/Examples` folder.

Here is an example for a C# Console Application:

```cs
using System;
using GlobalHotKeys;
using GlobalHotKeys.Native.Types;

void HotKeyPressed(HotKey hotKey) =>
  Console.WriteLine($"HotKey Pressed: Id = {hotKey.Id}, Key = {hotKey.Key}, Modifiers = {hotKey.Modifiers}");

using var hotKeyManager = new HotKeyManager();
using var subscription = hotKeyManager.HotKeyPressed.Subscribe(HotKeyPressed);
using var shift1 = hotKeyManager.Register(VirtualKeyCode.KEY_1, Modifiers.Shift);
using var ctrl1 = hotKeyManager.Register(VirtualKeyCode.KEY_1, Modifiers.Control);

Console.WriteLine("Listening for HotKeys...");
Console.ReadLine();
```

## Source POI
- F#
- Examples for
  - WinForms
  - Wpf
  - Console
  - AvaloniaUI
- Implements a simple Message Loop

## COPYRIGHT
Copyright © 2021 Martin Kramer (https://lostindetails.com)
This work is free. You can redistribute it and/or modify it under the
terms of the Do What The Fuck You Want To Public License, Version 2,
as published by Sam Hocevar. See http://www.wtfpl.net/ for more details.