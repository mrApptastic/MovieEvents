# Testing

## Overview

MovieEvents uses xUnit as the test framework with FluentAssertions for readable assertions and NSubstitute for mocking.

## Test Projects

### MovieEvents.Core.Tests

Tests for:
- **Models** - MovieClub operations, Movie URL generation, AppState creation
- **Results** - Result and Result\<T\> pattern
- **Serialization** - JSON round-trip with source generators
- **Services** - AppStateService, MovieEventService, ThemeService (with mocked dependencies)

### MovieEvents.App.Tests

Tests for:
- **Components** - Blazor component rendering and interaction using bUnit

## Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run specific project
dotnet test tests/MovieEvents.Core.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Test Patterns

### Service Tests

Services are tested with mocked dependencies using NSubstitute:

```csharp
var storage = Substitute.For<ILocalStorageService>();
var logger = Substitute.For<ILogger<AppStateService>>();
var service = new AppStateService(storage, logger);
```

### Serialization Tests

Verify round-trip fidelity:

```csharp
var json = JsonSerializer.Serialize(state, AppJsonContext.Default.Options);
var deserialized = JsonSerializer.Deserialize<AppState>(json, AppJsonContext.Default.Options);
deserialized.Should().BeEquivalentTo(state);
```

### Component Tests (bUnit)

```csharp
using var ctx = new TestContext();
ctx.Services.AddSingleton(Substitute.For<IAppStateService>());
var cut = ctx.RenderComponent<Home>();
cut.Find("h1").TextContent.Should().Contain("Dashboard");
```

## Libraries

| Library | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.9+ | Test framework |
| FluentAssertions | 8.x | Readable assertions |
| NSubstitute | 5.x | Mocking |
| bUnit | 2.x | Blazor component testing |
