using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Infrastructure.Serialization;
using MovieEvents.Infrastructure.Services;
using NSubstitute;

namespace MovieEvents.Core.Tests.Services;

public class AppStateServiceTests
{
    private readonly ILocalStorageService _storage = Substitute.For<ILocalStorageService>();
    private readonly ILogger<AppStateService> _logger = Substitute.For<ILogger<AppStateService>>();

    [Fact]
    public async Task GetStateAsync_ShouldReturnDefaultState_WhenStorageIsEmpty()
    {
        _storage.GetAsync<AppState>("movieevents_appstate", Arg.Any<CancellationToken>())
            .Returns((AppState?)null);

        var service = CreateService();

        var state = await service.GetStateAsync();

        state.Should().BeEquivalentTo(AppState.Create());
    }

    [Fact]
    public async Task SaveStateAsync_ShouldPersistState()
    {
        var state = CreateState();
        var service = CreateService();

        await service.SaveStateAsync(state);

        await _storage.Received(1).SetAsync("movieevents_appstate", state, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExportStateAsync_ShouldReturnValidJsonWithExportTimestamp()
    {
        var state = CreateState();
        _storage.GetAsync<AppState>("movieevents_appstate", Arg.Any<CancellationToken>())
            .Returns(state);
        var service = CreateService();

        var json = await service.ExportStateAsync();
        var exportedState = JsonSerializer.Deserialize(json, AppJsonContext.Default.AppState);

        exportedState.Should().NotBeNull();
        exportedState!.UserEmail.Should().Be(state.UserEmail);
        exportedState.UserName.Should().Be(state.UserName);
        exportedState.Club.Should().BeEquivalentTo(state.Club);
        exportedState.ExportedAt.Should().NotBeNull();
        exportedState.ExportedAt.Should().BeOnOrAfter(DateTimeOffset.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public async Task ImportStateAsync_ShouldPersistState_WhenJsonIsValid()
    {
        var state = CreateState();
        var json = JsonSerializer.Serialize(state, AppJsonContext.Default.AppState);
        var service = CreateService();

        var result = await service.ImportStateAsync(json);

        result.IsSuccess.Should().BeTrue();
        await _storage.Received(1).SetAsync(
            "movieevents_appstate",
            Arg.Is<AppState>(saved =>
                saved.UserEmail == state.UserEmail &&
                saved.UserName == state.UserName &&
                saved.Club.FavouriteMovies.Select(m => m.Id).SequenceEqual(state.Club.FavouriteMovies.Select(m => m.Id))),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ImportStateAsync_ShouldReturnFailure_WhenJsonIsMalformed()
    {
        var service = CreateService();

        var result = await service.ImportStateAsync("{ not valid json");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().StartWith("Invalid JSON format:");
        await _storage.DidNotReceiveWithAnyArgs().SetAsync(default!, default(AppState)!, default);
    }

    [Fact]
    public async Task ImportStateAsync_ShouldReturnFailure_WhenJsonRepresentsNullState()
    {
        var service = CreateService();

        var result = await service.ImportStateAsync("null");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid JSON: deserialization returned null.");
        await _storage.DidNotReceiveWithAnyArgs().SetAsync(default!, default(AppState)!, default);
    }

    private AppStateService CreateService() => new(_storage, _logger);

    private static AppState CreateState() => new()
    {
        UserEmail = "user@example.com",
        UserName = "User",
        Club = new MovieClub
        {
            FavouriteMovies =
            [
                new Movie
                {
                    Id = 7,
                    Title = "Se7en",
                    Overview = "A thriller"
                }
            ]
        }
    };
}
