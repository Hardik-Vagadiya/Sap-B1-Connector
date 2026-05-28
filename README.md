# SAP B1 Connector

A minimal, well-structured **SAP Business One add-on boilerplate** written in C#. Handles the full connection lifecycle for both the **UI API** (`SAPbouiCOM`) and **DI API** (`SAPbobsCOM`), so you can skip the boilerplate and focus on building business logic.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [How It Works](#how-it-works)
- [Getting Started](#getting-started)
- [Building the Project](#building-the-project)
- [Registering the Add-on in SAP B1](#registering-the-add-on-in-sap-b1)
- [Usage](#usage)
- [Known Limitations](#known-limitations)
- [License](#license)

---

## Overview

SAP Business One add-ons require two separate API connections to function:

| API | COM Library | Purpose |
|-----|-------------|---------|
| **UI API** | `SAPbouiCOM` | Interact with the SAP B1 client interface — menus, forms, events |
| **DI API** | `SAPbobsCOM` | Read and write business data — orders, invoices, partners, etc. |

This project provides a clean, minimal foundation that establishes both connections on startup, using the connection string SAP passes to the add-on at launch. It is intentionally thin — no business logic, no extra dependencies — making it easy to use as a starting point for any SAP B1 add-on project.

---

## Prerequisites

Before building or running this add-on, make sure you have the following installed:

- **Windows** (x64) — SAP B1 COM APIs are Windows-only
- **SAP Business One Client 10.0** — required for the COM references (`SAPbouiCOM`, `SAPbobsCOM`)
- **.NET Framework 4.8** — [Download here](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48)
- **Visual Studio 2019 or later** — with the `.NET desktop development` workload

> ⚠️ The project is compiled for **x64** only. Do not change the platform target to AnyCPU or x86, as the SAP B1 COM libraries require a 64-bit host process.

---

## Project Structure

```
SapB1Connector/
│
├── Core/
│   ├── SapApplication.cs     # Connects to the UI API (SAPbouiCOM)
│   ├── SapCompany.cs         # Connects to the DI API (SAPbobsCOM)
│   └── Startup.cs            # Orchestrates the connection sequence
│
├── ConnectionManager.cs      # Holds static references to the live
│                             # Application and Company objects
├── Program.cs                # Entry point — receives SAP connection args
├── App.config                # Application configuration
└── SapB1Connector.csproj     # Project file with COM references
```

### Why no `Application.Run(form)` in Program.cs?

SAP B1 add-ons do **not** open their own main window. The add-on attaches to SAP's existing client window and responds to events from there. `Application.Run()` is called without a form argument intentionally — this keeps the Windows message loop alive so the add-on can receive UI API events, while SAP's own window acts as the shell.

---

## How It Works

The connection sequence on startup is:

```
Program.Main(args)
    └── Startup.Initialize(args)
            ├── SapApplication.Connect(args)
            │       └── Reads args[0] (SAP connection string)
            │           Connects SboGuiApi and stores Application
            │           in ConnectionManager.oApp
            │
            └── (null guard) if oApp is null → exit early
                SapCompany.Connect()
                        └── Gets context cookie from oApp
                            Sets SBO login context on Company object
                            Connects DI API and stores Company
                            in ConnectionManager.oCom
                            Shows success message on SAP status bar
```

Once initialized, both `ConnectionManager.oApp` (UI API) and `ConnectionManager.oCom` (DI API) are available globally for use anywhere in the add-on.

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/your-username/Sap-B1-Connector.git
cd Sap-B1-Connector
```

### 2. Open in Visual Studio

Open `SapB1Connector.csproj` (or the `.slnx` solution file) in Visual Studio.

### 3. Verify COM references

In **Solution Explorer → References**, confirm that both COM references resolve correctly:

- `Interop.SAPbobsCOM` — DI API
- `Interop.SAPbouiCOM` — UI API

If they show a warning icon, it means the SAP B1 client is either not installed or installed at a different path. Reinstall or repair the SAP B1 client, then right-click References → **Add COM Reference** and re-add both libraries.

---

## Building the Project

1. Set the build configuration to **Debug | x64** (or **Release | x64**)
2. Build the solution: `Ctrl + Shift + B`
3. The output `.exe` will be at `bin\Debug\SapB1Connector.exe`

> Do not run the `.exe` directly — it will fail because SAP does not pass the connection string when launched manually. The add-on must be launched by SAP B1 via the Add-on Manager (see below).

---

## Registering the Add-on in SAP B1

1. Open **SAP Business One** and log in as a manager user
2. Go to **Administration → Add-Ons → Add-On Manager**
3. Click **Register** and browse to `SapB1Connector.exe`
4. Set the **Start-up Type** to `Automatic` or `Manual` as needed
5. Click **Update** and then **OK**

On the next SAP B1 login, the add-on will launch automatically. SAP passes the connection string as a command-line argument to the `.exe`, which is how `args[0]` in `Program.Main` receives it.

---

## Usage

Use this project as a **starting point** for your own SAP B1 add-on. After the connection is established, extend the project by:

- **Subscribing to UI API events** via `ConnectionManager.oApp` in an event handler class
- **Reading or writing business data** via `ConnectionManager.oCom` using DI API objects (e.g. `oCom.GetBusinessObject(...)`)
- **Adding custom menus or forms** through the UI API

Example — adding a menu item after connection:

```csharp
var oMenus = ConnectionManager.oApp.Menus;
var oCreationParams = (SAPbouiCOM.MenuCreationParams)
    ConnectionManager.oApp.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);

oCreationParams.Type    = SAPbouiCOM.BoMenuType.mt_String;
oCreationParams.UniqueID = "MY_MENU_ITEM";
oCreationParams.String  = "My Add-on";
oCreationParams.Position = 0;

oMenus.Item("43520").SubMenus.AddEx(oCreationParams);
```

---

## Known Limitations

- **Windows x64 only** — SAP B1 COM APIs do not support Linux, macOS, or 32-bit processes.
- **SAP B1 10.0** — COM reference GUIDs are version-specific. If you are on a different SAP B1 version, you will need to re-add the COM references from your installed client.
- **No graceful disconnect** — This boilerplate does not implement a disconnect handler for when SAP shuts down. For production add-ons, subscribe to `ConnectionManager.oApp.AppEvent` and handle `SAPbouiCOM.BoAppEventTypes.aet_ShutDown` to call `oCompany.Disconnect()` cleanly.

---

## License

This project is licensed under the [MIT License](LICENSE).