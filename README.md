# TrayCommander

A Windows system tray utility that puts your cmd, PowerShell, and WSL commands one right-click away.

## Features

- Run commands via **cmd**, **PowerShell**, or **WSL** from the system tray context menu
- Execute **inline commands** or point to a **script file**
- Per-command **admin elevation** via UAC prompt, or run the entire app as administrator
- **ClickOnce deployment** with automatic updates
- Configuration stored as plain JSON — editable by hand or via the built-in editor

## Requirements

- Windows 10/11
- [.NET 10 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)

## Installation

**ClickOnce (recommended):** visit [dfsramos.github.io/tray-commander](https://dfsramos.github.io/tray-commander/) and click **Install**. Updates are applied automatically on next launch.

**Build from source:** see [Building](#building).

## Usage

TrayCommander lives in the notification area. **Right-click** the `>_` icon to open the command menu. **Left-click** also opens the menu.

### Editing commands

Right-click → **Edit Commands** to open the command editor. Each row defines one menu entry:

| Field | Description |
|---|---|
| Name | Label shown in the tray menu |
| Command | Inline command to execute (leave blank if using a script file) |
| Script Path | Path to a `.ps1`, `.sh`, `.bat`, or `.cmd` file |
| Runner | `cmd`, `powershell`, or `wsl` |
| Requires Admin | Triggers a UAC elevation prompt before running |

If both **Command** and **Script Path** are set, the script file takes precedence.

### Options

Right-click → **Options** to toggle **Always run as administrator**. When enabled, the app re-launches itself elevated and all commands run in that elevated context without individual UAC prompts.

## Building

Requires [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and Windows (or WSL with Windows targeting enabled).

```bash
dotnet restore
dotnet build
```

To publish a ClickOnce package locally:

```bash
msbuild TrayCommander.csproj /restore /t:Publish /p:PublishProfile=ClickOnce /p:Configuration=Release
```

Output lands in `bin/Release/net10.0-windows/publish/`.
