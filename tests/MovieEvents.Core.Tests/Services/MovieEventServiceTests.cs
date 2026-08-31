using FluentAssertions;
using Microsoft.Extensions.Logging;
using MovieEvents.Core.Enums;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Core.Results;
using MovieEvents.Infrastructure.Services;
using NSubstitute;

namespace MovieEvents.Core.Tests.Services;

public class MovieEventServiceTests
{
    private readonly IAppStateService _appStateService = Substitute.For<IAppStateService>();
    private readonly IGoogleCalendarService _calendarService = Substitute.For<IGoogleCalendarService>();
    private readonly IGmailService _gmailService = Substitute.For<IGmailService>();
    private readonly ILogger<MovieEventService> _logger = Substitute.For<ILogger<MovieEventService>>();

    [Fact]
    public async Task CreateEventAsync_ShouldSaveEventAndCallCalendarAndGmailServices()
    {
        var friendOne = CreateFriend("Alex");
        var friendTwo = CreateFriend("Jamie");
        var group = new FriendGroup
        {
            Name = "Crew",
            FriendIds = [friendOne.Id, friendTwo.Id]
        };
        var state = new AppState
        {
            Club = new MovieClub
            {
                Friends = [friendOne, friendTwo],
                Groups = [group]
            }
        };
        var movieEvent = CreateMovieEvent(friendOne.Id, group.Id);

        _appStateService.GetStateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(state));
        _calendarService.CreateEventAsync(movieEvent, Arg.Any<List<Friend>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<string>.Success("calendar-123")));
        _gmailService.SendInvitationAsync(movieEvent, Arg.Any<List<Friend>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        var service = CreateService();

        var result = await service.CreateEventAsync(movieEvent);

        result.IsSuccess.Should().BeTrue();
        movieEvent.CalendarEventId.Should().Be("calendar-123");
        state.Club.Events.Should().ContainSingle().Which.Should().BeSameAs(movieEvent);
        await _appStateService.Received(1).SaveStateAsync(state, Arg.Any<CancellationToken>());
        await _calendarService.Received(1).CreateEventAsync(
            movieEvent,
            Arg.Is<List<Friend>>(friends => HaveSameFriendIds(friends, friendOne.Id, friendTwo.Id)),
            Arg.Any<CancellationToken>());
        await _gmailService.Received(1).SendInvitationAsync(
            movieEvent,
            Arg.Is<List<Friend>>(friends => HaveSameFriendIds(friends, friendOne.Id, friendTwo.Id)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelEventAsync_ShouldCancelEventAndCallCalendarAndGmailServices()
    {
        var friend = CreateFriend("Taylor");
        var group = new FriendGroup
        {
            Name = "Besties",
            FriendIds = [friend.Id]
        };
        var movieEvent = CreateMovieEvent(friend.Id, group.Id);
        movieEvent.CalendarEventId = "calendar-456";
        var originalUpdatedAt = movieEvent.UpdatedAt;
        var state = new AppState
        {
            Club = new MovieClub
            {
                Friends = [friend],
                Groups = [group],
                Events = [movieEvent]
            }
        };

        _appStateService.GetStateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(state));
        _calendarService.DeleteEventAsync("calendar-456", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));
        _gmailService.SendCancellationAsync(movieEvent, Arg.Any<List<Friend>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        var service = CreateService();

        var result = await service.CancelEventAsync(movieEvent.Id);

        result.IsSuccess.Should().BeTrue();
        movieEvent.Status.Should().Be(EventStatus.Cancelled);
        movieEvent.UpdatedAt.Should().BeOnOrAfter(originalUpdatedAt);
        await _calendarService.Received(1).DeleteEventAsync("calendar-456", Arg.Any<CancellationToken>());
        await _gmailService.Received(1).SendCancellationAsync(
            movieEvent,
            Arg.Is<List<Friend>>(friends => HaveSameFriendIds(friends, friend.Id)),
            Arg.Any<CancellationToken>());
        await _appStateService.Received(1).SaveStateAsync(state, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelEventAsync_ShouldReturnFailure_WhenEventDoesNotExist()
    {
        var state = AppState.Create();
        _appStateService.GetStateAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(state));
        var service = CreateService();

        var result = await service.CancelEventAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Event not found.");
        await _calendarService.DidNotReceiveWithAnyArgs().DeleteEventAsync(default!, default);
        await _gmailService.DidNotReceiveWithAnyArgs().SendCancellationAsync(default!, default!, default);
        await _appStateService.DidNotReceiveWithAnyArgs().SaveStateAsync(default!, default);
    }

    private MovieEventService CreateService() => new(_appStateService, _calendarService, _gmailService, _logger);

    private static bool HaveSameFriendIds(List<Friend> friends, params Guid[] expectedIds) =>
        friends.Select(friend => friend.Id).OrderBy(id => id).SequenceEqual(expectedIds.OrderBy(id => id));

    private static Friend CreateFriend(string name) => new()
    {
        Name = name,
        Email = $"{name.ToLowerInvariant()}@example.com"
    };

    private static MovieEvent CreateMovieEvent(Guid invitedFriendId, Guid invitedGroupId) => new()
    {
        Movie = new Movie
        {
            Id = 5,
            Title = "Interstellar",
            Overview = "Space epic"
        },
        EventDate = new DateTimeOffset(2026, 10, 10, 20, 0, 0, TimeSpan.Zero),
        Location = new Location
        {
            Name = "Grand Cinema",
            Address = "10 High Street"
        },
        InvitedFriendIds = [invitedFriendId],
        InvitedGroupIds = [invitedGroupId]
    };
}
