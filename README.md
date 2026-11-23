# LifeDeck

A personal workspace built with **C#, .NET 10 and Blazor WebAssembly**, combining productivity tools, a diving logbook, personal finance tracking and everyday utilities.

LifeDeck stores personal records in the browser and includes **JSON backup and restore, TXT export, Excel export and Progressive Web App features**.

## Overview

LifeDeck brings several everyday tools into one browser-based application:

- Organize tasks, notes and appointments.
- Record scuba dives.
- Track personal income and expenses.
- Write locally stored personal posts.
- Check weather and convert currencies.
- Perform calculations.
- Export and restore personal records.

The application runs client-side without a backend database or user account system.

## Modules

| Module | Features |
|---|---|
| Overview | Record counts for the six data-management modules |
| Diving Logbook | Create, edit, delete and search dive records |
| Todo | Add and edit tasks, toggle completion and delete entries |
| Notebook | Create, edit and search notes by title or content |
| Calendar | Monthly calendar with appointment creation, editing and deletion |
| Personal Account | Income and expenses, categories, payment methods and separate currency balances |
| Blog | Personal posts with tags, search and expandable content |
| Weather | Location search and current weather conditions |
| Currency Converter | Currency conversion using external reference rates |
| Calculator | Arithmetic expressions with precedence, parentheses and decimals |
| Data Backup | JSON backup/restore and TXT/XLSX export |

## Diving Logbook

Dive entries include:

- Date, location and dive site
- Maximum depth and bottom time
- Dive type
- Tank and exposure suit details
- Weight
- Buddy
- Notes

The logbook supports editing, deletion and search by location, dive site or buddy.

## Personal Finance

The Personal Account module records income and expenses with:

- Date
- Amount and currency
- Category
- Payment method
- Description

Supported account currencies are **TRY, USD, EUR and GBP**.

Balances are calculated separately for each currency. Records can be searched and filtered by transaction type and currency.

These balances reflect manually entered transactions; the application does not connect to bank accounts.

## Weather and Currency Tools

### Weather

The application uses Open-Meteo services for location search and current conditions.

Displayed information includes temperature, feels-like temperature, humidity, wind, precipitation and atmospheric pressure.

### Currency Converter

The converter uses Frankfurter reference rates and supports:

- TRY
- USD
- EUR
- GBP
- CHF
- JPY
- CAD
- AUD

Weather lookups and conversions between different currencies require network access. Currency conversion is a separate utility and does not automatically convert financial records.

## Calculator

The calculator uses a C# expression parser with support for:

- Addition, subtraction, multiplication and division
- Operator precedence
- Parentheses
- Positive and negative numbers
- Decimal values
- Invalid-expression and division-by-zero handling

## Local Data Storage

The six record-based modules store JSON data in browser `localStorage` through a JavaScript interop service.

| Module | Storage Key |
|---|---|
| Diving Logbook | `lifedeck:dives:v1` |
| Todo | `lifedeck:todos:v1` |
| Notebook | `lifedeck:notes:v1` |
| Calendar | `lifedeck:appointments:v1` |
| Personal Account | `lifedeck:transactions:v1` |
| Blog | `lifedeck:blogposts:v1` |

Records belong to the browser profile and origin used to open the application. They are not automatically shared across browsers, devices or different hosting addresses.

The Blog module stores personal posts locally; it does not publish them to the internet.

## Backup, Export and Restore

| Format | Export | Restore | Contents |
|---|---|---|---|
| JSON | Yes | Yes | Versioned backup of all six record-based modules |
| TXT | Yes | No | Human-readable records grouped by module |
| Excel `.xlsx` | Yes | No | A workbook containing six worksheets |

Excel worksheets are:

- Logbook
- Todo
- Notebook
- Calendar
- Transactions
- Blog

The XLSX writer generates Office Open XML using .NET XML and ZIP APIs, without a third-party spreadsheet package.

### JSON Restore Workflow

1. Select a LifeDeck backup file.
2. Validate its format, version, modules and record structure.
3. Preview the record counts.
4. Confirm replacement.
5. Write the restored records to browser storage.

**Restoring replaces existing module records; it does not merge them.**

If a storage write fails, the JavaScript restore routine attempts to recover the previous values. Browser storage does not provide a transaction across these writes.

File exports use a native Save As dialog where supported, with a download fallback.

## Progressive Web App

LifeDeck includes:

- A web app manifest and application icons
- Service worker registration
- Versioned asset caching for published builds
- Offline navigation through the cached application entry point
- Standalone display when installed through a supported browser

Offline application access requires an initial successful load and service worker installation.

The published service worker caches application assets. External weather and currency requests remain network-dependent.

The development service worker does not use the production offline caching strategy.

## Interface

The shared application layout includes:

- Responsive sidebar navigation
- Collapsible desktop navigation
- Mobile navigation controls
- Theme switching
- Reusable SVG icons
- Shared styles and page-specific CSS isolation

## Technology Stack

| Area | Technology |
|---|---|
| Language | C# |
| Framework | .NET 10 / Blazor WebAssembly |
| Components | Razor |
| Styling | CSS, CSS isolation and bundled Bootstrap |
| Browser Integration | JavaScript interop |
| Persistence | Browser localStorage and System.Text.Json |
| External Data | HttpClient, Open-Meteo and Frankfurter |
| PWA | Web App Manifest, Service Worker and Cache API |
| Excel Export | Office Open XML through .NET XML and ZIP APIs |

## Architecture

LifeDeck is a client-side application organized into pages, models and focused services.

| Path | Responsibility |
|---|---|
| `Pages/` | Interactive modules and their scoped styles |
| `Models/` | Personal records, API responses and backup format |
| `Layout/` | Application shell and navigation |
| `Components/` | Reusable interface components |
| `Services/Storage/` | Browser storage abstraction |
| `Services/Api/` | Weather and currency API access |
| `Services/Backup/` | Backup validation, restore and export generation |
| `Services/Calculator/` | Arithmetic expression parser |
| `wwwroot/js/` | File access, restore and theme integration |
| `wwwroot/css/` | Shared application styling |
| `Program.cs` | Client startup and service registration |

Pages manage their interactive state. Storage, API and backup services are registered through dependency injection.

No server-side application or relational database is required.

## Getting Started

### Requirements

- .NET 10 SDK
- A browser capable of running Blazor WebAssembly
- Internet access for package restoration and external API features

### Clone and Run

```bash
git clone https://github.com/hakanberat/LifeDeck.git
cd LifeDeck

dotnet restore
dotnet run
```

Open the local address printed in the terminal.

For development with automatic reload:

```bash
dotnet watch run
```

### Publish

```bash
dotnet publish LifeDeck.csproj -c Release -o ../LifeDeck-Publish
```

The published static application is located in:

```text
../LifeDeck-Publish/wwwroot
```

Serve this directory through a static web server.

For example, using the optional `dotnet-serve` tool:

```bash
dotnet tool install --global dotnet-serve
dotnet serve -d ../LifeDeck-Publish/wwwroot -p 8081
```

Then open:

```text
http://localhost:8081
```

Use the published build to verify installation and offline behavior. Service worker functionality requires HTTPS or a supported localhost environment.

The current HTML base path is `/`; deployment beneath a subdirectory requires appropriate base-path configuration.

## Current Scope

- Personal records are stored locally without cloud synchronization.
- Clearing browser site data removes locally stored records.
- JSON export provides manual backup and transfer between workspaces.
- TXT and Excel files are exports, not supported restore formats.
- Weather and currency information depend on external services.
- No automated test project is included in the repository.

## Author

**Hakan Berat Demircan**

LifeDeck demonstrates client-side C# development, reusable Blazor interfaces, browser persistence, API integration, PWA asset caching and data export/restore workflows.

[GitHub Profile](https://github.com/hakanberat)
