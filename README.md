# MovieEvents

A .NET 10 Blazor WebAssembly Progressive Web App (PWA) for organizing movie events with friends.

## Features

- **Movie Search** - Browse and search movies using TMDb API
- **Favourites** - Save favourite movies for offline viewing
- **Friends & Groups** - Manage friends and organize them into groups
- **Locations** - Save event locations
- **Movie Events** - Schedule movie events with calendar integration
- **Google Calendar** - Automatic calendar event creation
- **Email Invitations** - Send HTML invitations via Gmail
- **Dark/Light Mode** - Toggle between themes
- **PWA** - Install as a native app, works offline
- **Data Portability** - Export/import all data as JSON
- **Gmail Backup** - Send data backup to your Gmail

## Architecture

- **MovieEvents.Core** - Domain models, interfaces, and result types
- **MovieEvents.Infrastructure** - Service implementations, API clients, storage
- **MovieEvents.App** - Blazor WASM UI components and pages

See [Architecture.md](Architecture.md) for details.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dot.net)
- A TMDb API key ([register here](https://www.themoviedb.org/settings/api))
- A Google Cloud project with Calendar and Gmail APIs enabled

### Configuration

1. Set your TMDb API key in `src/MovieEvents.App/wwwroot/appsettings.json`:
   ```json
   {
     "Tmdb": {
       "ApiKey": "YOUR_TMDB_API_KEY"
     }
   }
   ```

2. Configure Google OAuth - see [GoogleApiSetup.md](GoogleApiSetup.md)

### Run Locally

```bash
cd src/MovieEvents.App
dotnet run
```

### Run Tests

```bash
dotnet test
```

### Build for Production

```bash
dotnet publish src/MovieEvents.App/MovieEvents.App.csproj -c Release -o release
```

## Deployment

Deployed automatically to GitHub Pages via GitHub Actions on push to `main`.

See [Deployment.md](Deployment.md) for details.

## Documentation

- [Architecture.md](Architecture.md) - System architecture and design decisions
- [Deployment.md](Deployment.md) - Deployment configuration
- [GoogleApiSetup.md](GoogleApiSetup.md) - Google OAuth and API setup
- [TMDbSetup.md](TMDbSetup.md) - TMDb API configuration
- [Testing.md](Testing.md) - Testing strategy and guidelines

## Technology Stack

- C# / .NET 10
- Blazor WebAssembly (standalone)
- Bootstrap 5 + Bootstrap Icons
- System.Text.Json (source generators)
- xUnit + bUnit + FluentAssertions
- GitHub Actions + GitHub Pages

## License

MIT
