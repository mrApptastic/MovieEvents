using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MovieEvents.Core.Interfaces;
using MovieEvents.Infrastructure.Services;
using NSubstitute;

namespace MovieEvents.Core.Tests.Services;

public class ThemeServiceTests
{
    private readonly ILocalStorageService _storage = Substitute.For<ILocalStorageService>();
    private readonly RecordingJsRuntime _jsRuntime = new();
    private readonly ILogger<ThemeService> _logger = Substitute.For<ILogger<ThemeService>>();

    [Fact]
    public async Task GetThemeAsync_ShouldReturnLight_WhenStorageIsEmpty()
    {
        _storage.GetAsync<string>("movieevents_theme", Arg.Any<CancellationToken>())
            .Returns((string?)null);
        var service = CreateService();

        var theme = await service.GetThemeAsync();

        theme.Should().Be("light");
    }

    [Fact]
    public async Task SetThemeAsync_ShouldPersistThemeInvokeJsAndRaiseEvent()
    {
        var service = CreateService();
        var eventThemes = new List<string>();
        service.OnThemeChanged += theme => eventThemes.Add(theme);

        await service.SetThemeAsync("dark");

        await _storage.Received(1).SetAsync("movieevents_theme", "dark", Arg.Any<CancellationToken>());
        _jsRuntime.Identifier.Should().Be("setTheme");
        _jsRuntime.Arguments.Should().BeEquivalentTo(["dark"]);
        eventThemes.Should().Equal("dark");
    }

    private ThemeService CreateService() => new(_storage, _jsRuntime, _logger);

    private sealed class RecordingJsRuntime : IJSRuntime
    {
        public string? Identifier { get; private set; }

        public object?[]? Arguments { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Identifier = identifier;
            Arguments = args;
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Identifier = identifier;
            Arguments = args;
            return ValueTask.FromResult(default(TValue)!);
        }
    }
}
