# ClimateGSR

ClimateGSR is an ASP.NET Core web application for exploring climate and population statistics by country and playing a climate-themed geography quiz.

## Features

- Interactive world map with country-level climate and population information.
- Quiz game with five rounds, emissions prompts, hints, and score tracking.
- Guest play; guest scores are not saved.
- Account registration, login, and account management with ASP.NET Core Identity.
- Leaderboard ranked by signed-in users' highest single-game score.

## Requirements

- .NET 10 SDK

## Run locally

From the repository root:

```bash
cd "ClimateGSR in ASP.NET Core"
dotnet restore
dotnet run --launch-profile https
```

The HTTPS profile serves the app at `https://localhost:7121` and `http://localhost:5205`. To use the HTTP profile instead:

```bash
dotnet run --launch-profile http
```

On startup, the application applies Entity Framework Core migrations and seeds the SQLite database from files in `App_Data/` when country data has not already been seeded. The default connection string in `appsettings.json` creates/uses `ClimateGSR.db` in the application content root.

## Build

From the repository root:

```bash
dotnet build "ClimateGSR in ASP.NET Core.sln"
```

## Project layout

- `ClimateGSR in ASP.NET Core/Controllers/` — MVC controllers for accounts, the game, and home-page data.
- `ClimateGSR in ASP.NET Core/Models/` — Entity Framework entities, view models, game logic, and leaderboard service.
- `ClimateGSR in ASP.NET Core/Views/` — Razor views.
- `ClimateGSR in ASP.NET Core/App_Data/` — climate, population, temperature, and country source data used for seeding.
- `ClimateGSR in ASP.NET Core/wwwroot/` and `Content/` — static assets and styles.
- `ClimateGSR in ASP.NET Core/Migrations/` — Entity Framework Core database migrations.

The leaderboard is available at `/Game/Leaderboard`. Only scores from games started while signed in are saved; signing in partway through a guest game does not make that game's score eligible for saving.
