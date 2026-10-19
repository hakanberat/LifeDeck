# LifeDeck

**A modular, local-first personal productivity Progressive Web App built with .NET 10 and Blazor WebAssembly.**

LifeDeck brings everyday tools into a single workspace: a diving logbook, task manager, notebook, appointments calendar, personal finance tracker, blog, weather information, currency converter, and calculator.

The project demonstrates client-side application development with **C#, Razor, dependency injection, browser `localStorage`, JavaScript interop, REST APIs, and Progressive Web App (PWA) features**. It also includes a data portability system with **JSON backup and restore, TXT export, and native XLSX generation**.

> **Project status:** Active development · 2026  
> **Deployment:** Source-code repository; not hosted as a public web application.

## Features

| Module | Capabilities |
| --- | --- |
| **Dashboard** | Displays live record counts for the six data-management modules. |
| **Diving Logbook** | Create, edit, delete, and search dive entries; record dive sites, depth, duration, equipment, buddy, and notes. |
| **Todo** | Manage tasks and completion status with persistent local storage. |
| **Notebook** | Create, edit, delete, and search notes by title or content. |
| **Calendar** | Browse a monthly calendar and manage past and upcoming appointments. |
| **Weather** | Search locations and retrieve current conditions using the Open-Meteo API. |
| **Currency Converter** | Convert between TRY, USD, EUR, GBP, CHF, JPY, CAD, and AUD using Frankfurter reference rates. |
| **Calculator** | Evaluate expressions with operator precedence, parentheses, decimals, and error handling using a C# parser. |
| **Personal Account** | Track income and expenses, payment methods, categories, and separate balances for TRY, USD, EUR, and GBP. |
| **Blog** | Write, edit, delete, tag, search, and expand personal posts. |
| **Data Backup** | Export records to JSON, TXT, or Excel; validate and restore LifeDeck JSON backups. |

## Technology Stack

| Area | Technology |
| --- | --- |
| Framework | **.NET 10**, **Blazor WebAssembly** |
| Languages | **C#**, **Razor**, HTML, CSS, JavaScript |
| UI | Component-based Blazor pages, CSS isolation, Bootstrap |
| Data persistence | Browser **`localStorage`** via **JavaScript interop** |
| Application structure | Models, services, dependency injection, client-side UI |
| External APIs | **Open-Meteo** (weather and geocoding), **Frankfurter** (exchange rates) |
| PWA | Web App Manifest, Service Worker, Cache API |
| Export formats | **JSON**, **TXT**, **Excel `.xlsx`** |
| Excel generation | Office Open XML via .NET XML and ZIP APIs; **no third-party Excel package** |

## Data Backup and Restore

LifeDeck keeps user-created records in the browser and supports manual file exports.

| Format | Export | Import / Restore | Details |
| --- | :---: | :---: | --- |
| **JSON** | Yes | Yes | Versioned LifeDeck backup containing all six data modules. |
| **TXT** | Yes | No | Human-readable records organized by module. |
| **Excel (`.xlsx`)** | Yes | No | Six worksheets: Logbook, Todo, Notebook, Calendar, Transactions, and Blog. |

The JSON restore workflow checks the file structure and record formats, previews record counts, and requires confirmation before replacing stored data. **Restoration replaces existing records; it does not merge them.** The storage writer attempts to roll back changes if an error occurs, but this is not a database transaction. Keep an additional copy of important backups.

File saving uses the browser's native Save As API when available, with a download fallback.

## Progressive Web App

LifeDeck includes:

- A web app manifest and installable application icons.
- A Service Worker registration.
- Asset pre-caching in **published Release builds** for offline application access.
- A standalone application window when installed through a supported browser.

**Offline limitations:** Weather and currency conversions require network access for fresh API responses. Offline availability of the application itself depends on successful Service Worker installation and caching. The development build does not enable the production offline-cache strategy.

## Getting Started

### Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A modern browser that supports Blazor WebAssembly

### Run locally

```bash
git clone https://github.com/hakanberat/LifeDeck.git
cd LifeDeck
dotnet restore
dotnet watch run
```

Open the local URL shown in the terminal. Alternatively, use `dotnet run`.

### Create a release build

```bash
dotnet publish -c Release -o ../LifeDeck-Publish
```

The publishable static application is placed in `../LifeDeck-Publish/wwwroot`. Serve that directory through a local/static web server to test the published PWA. For example, with the optional `dotnet-serve` tool:

```bash
dotnet tool install --global dotnet-serve
dotnet serve -d ../LifeDeck-Publish/wwwroot -p 8081
```

Then visit `http://localhost:8081`. **PWA installation and offline behavior should be tested against the published build**, not assumed from `dotnet watch run`.

## Project Structure

```text
LifeDeck/
├── Layout/                  # Application shell and navigation
├── Models/                  # Records and API/backup models
├── Pages/                   # Blazor pages and isolated CSS
├── Services/
│   ├── Api/                 # Weather and currency services
│   ├── Backup/              # JSON/TXT/XLSX export and JSON restore
│   ├── Calculator/          # C# expression parser
│   └── Storage/             # localStorage abstraction
├── wwwroot/
│   ├── css/                 # Shared application styles
│   ├── icons/               # PWA icons
│   ├── js/                  # Browser file and storage interop
│   ├── manifest.webmanifest
│   ├── service-worker.js
│   └── service-worker.published.js
├── App.razor
├── Program.cs
└── LifeDeck.csproj
```

## Data Privacy and Current Limitations

- **No backend database, sign-in system, or cloud sync.** Data is stored per browser origin and browser profile.
- Records are **not automatically shared or synchronized** across devices or browsers.
- Clearing site data, changing browser profiles, or uninstalling applications with associated data removal can erase locally stored records. Export JSON backups regularly.
- JSON backup files can contain personal and financial information. Store them securely and avoid committing them to a public repository.
- Personal Account balances are calculated from entered transactions, not connected bank balances. Different currencies are **not combined into one balance**.
- Currency rates are reference rates, not necessarily a bank's buy/sell rates.
- Blog entries are personal, locally stored records; **they are not published to the internet**.

## Project Purpose

LifeDeck is a hands-on software portfolio project exploring practical **C#/.NET frontend engineering**, modular UI development, browser persistence, API consumption, PWA architecture, and data export/restore workflows.

**Author:** [hakanberat](https://github.com/hakanberat)  
**Started:** 2026
