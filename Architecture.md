# Architecture

## Overview

MovieEvents follows clean architecture principles with clear separation of concerns across three projects.

## Project Structure

```
MovieEvents/
├── src/
│   ├── MovieEvents.Core/          # Domain layer
│   │   ├── Models/                # Domain entities
│   │   ├── Interfaces/            # Service contracts
│   │   ├── Enums/                 # Domain enumerations
│   │   └── Results/               # Result pattern types
│   ├── MovieEvents.Infrastructure/ # Infrastructure layer
│   │   ├── ApiClients/            # External API clients (TMDb, Google)
│   │   ├── Authentication/        # Token provider abstraction
│   │   ├── DependencyInjection/   # Service registration
│   │   ├── Serialization/         # JSON source generators
│   │   ├── Services/              # Service implementations
│   │   └── Storage/               # Local storage implementation
│   └── MovieEvents.App/          # Presentation layer (Blazor WASM)
│       ├── Layout/                # Main layout and navigation
│       ├── Pages/                 # Routable page components
│       └── wwwroot/               # Static assets and PWA files
└── tests/
    ├── MovieEvents.Core.Tests/    # Unit tests for Core and Infrastructure
    └── MovieEvents.App.Tests/     # Component tests for Blazor UI
```

## Layers

### Core (MovieEvents.Core)

The domain layer contains:

- **Models** - Pure data classes representing domain entities (Movie, Friend, FriendGroup, Location, MovieEvent, MovieClub, AppState)
- **Interfaces** - Service contracts that define capabilities without implementation
- **Results** - Generic Result/Result\<T\> pattern for explicit error handling
- **Enums** - Domain enumerations (EventStatus)

No external dependencies. All other projects reference Core.

### Infrastructure (MovieEvents.Infrastructure)

Contains all service implementations:

- **LocalStorageService** - Browser localStorage via JS interop
- **AppStateService** - Application state persistence
- **ThemeService** - Dark/light mode management
- **MovieEventService** - Event creation/cancellation orchestration
- **TmdbApiClient** - TMDb movie search API
- **GoogleCalendarService** - Google Calendar REST API
- **GmailService** - Gmail REST API with MIME message construction
- **AppJsonContext** - System.Text.Json source generator for AOT-compatible serialization

### App (MovieEvents.App)

Blazor WebAssembly UI layer:

- **Dashboard** - Overview with statistics and upcoming events
- **Movies** - Search, browse, and favourite movies
- **Friends** - Manage friend contacts
- **Groups** - Organize friends into groups
- **Locations** - Manage event venues
- **Events** - Create and manage movie events
- **Settings** - Theme, data export/import, backup

## Design Decisions

### Result Pattern

All service operations return `Result` or `Result<T>` instead of throwing exceptions. This makes error handling explicit and prevents unhandled exceptions from crashing the WASM app.

### Local Storage

All data is stored in browser localStorage as serialized JSON. No backend database is required. The complete AppState is a single serializable object.

### Source Generators

System.Text.Json source generators (AppJsonContext) provide AOT-compatible, trim-safe serialization without reflection.

### Options Pattern

TMDb configuration uses the Options pattern (`IOptions<TmdbOptions>`) for strongly-typed configuration.

### Dependency Injection

All services are registered through `AddMovieEventsInfrastructure()` extension method, keeping Program.cs clean.

### PWA

Service worker enables offline caching of all static assets. Favourite movies store enough data locally for offline viewing.
