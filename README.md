# lil timer

A minimal Pomodoro timer for Windows. Runs as a hidden background task — no
taskbar entry, no main window. You control it entirely from the keyboard.

## Download

Grab the latest `lil-timer.exe` from the
[Releases](https://github.com/joshuatan0821/lil-timer/releases) page. It is a
self-contained build — no .NET runtime needed.

## Usage

| Key | Action |
| --- | --- |
| `Ctrl+Alt+T` | start / pause |
| `Ctrl+Alt+R` | reset |

The app sits in the system tray. Hover the icon to see the live countdown,
double-click it to open a status window, or right-click for start / pause /
reset / quit.

- 25 min focus → 5 min break, repeating automatically.
- When a session ends you get a balloon notification and a retro arcade chime.
- The tray icon is red while focusing, green on a break, gray when paused.

## Run at startup

1. Press `Win+R`, type `shell:startup`, and press Enter.
2. Drop a shortcut to `lil-timer.exe` into the folder that opens.

## Build from source

Requires the .NET 8 SDK.

```
dotnet publish -c Release -r win-x64 --self-contained -o dist
```

The exe is written to `dist\lil-timer.exe`.

## Design

The UI follows the design system in `DESIGN.md` — monospace type throughout,
cream canvas, 4px-radius controls, and ASCII bracket markers instead of icons.