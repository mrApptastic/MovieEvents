using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using FluentAssertions;
using MovieEvents.Core.Enums;
using MovieEvents.Core.Models;
using MovieEvents.Infrastructure.Serialization;

namespace MovieEvents.Core.Tests.Serialization;

public class SerializationTests
{
    [Fact]
    public void AppState_ShouldRoundTripThroughAppJsonContext()
    {
        var state = CreateAppState();

        AssertRoundTrip(state, AppJsonContext.Default.AppState);
    }

    [Fact]
    public void Movie_ShouldRoundTripThroughAppJsonContext()
    {
        var movie = CreateMovie();

        AssertRoundTrip(movie, AppJsonContext.Default.Movie);
    }

    [Fact]
    public void Friend_ShouldRoundTripThroughAppJsonContext()
    {
        var friend = CreateFriend("Alex", "alex@example.com");

        AssertRoundTrip(friend, AppJsonContext.Default.Friend);
    }

    [Fact]
    public void FriendGroup_ShouldRoundTripThroughAppJsonContext()
    {
        var friend = CreateFriend("Chris", "chris@example.com");
        var group = new FriendGroup
        {
            Name = "Movie Night",
            FriendIds = [friend.Id]
        };

        AssertRoundTrip(group, AppJsonContext.Default.FriendGroup);
    }

    [Fact]
    public void Location_ShouldRoundTripThroughAppJsonContext()
    {
        var location = CreateLocation();

        AssertRoundTrip(location, AppJsonContext.Default.Location);
    }

    [Fact]
    public void MovieEvent_ShouldRoundTripThroughAppJsonContext()
    {
        var friend = CreateFriend("Morgan", "morgan@example.com");
        var movieEvent = CreateMovieEvent(friend.Id);

        AssertRoundTrip(movieEvent, AppJsonContext.Default.MovieEvent);
    }

    private static void AssertRoundTrip<T>(T value, JsonTypeInfo<T> typeInfo)
    {
        var json = JsonSerializer.Serialize(value, typeInfo);
        var deserialized = JsonSerializer.Deserialize(json, typeInfo);

        deserialized.Should().NotBeNull();
        deserialized.Should().BeEquivalentTo(value);
    }

    private static AppState CreateAppState()
    {
        var friend = CreateFriend("Sam", "sam@example.com");
        var group = new FriendGroup
        {
            Name = "Inner Circle",
            FriendIds = [friend.Id]
        };
        var location = CreateLocation();
        var movieEvent = CreateMovieEvent(friend.Id, group.Id, location);
        var movie = CreateMovie();

        return new AppState
        {
            UserEmail = "owner@example.com",
            UserName = "Owner",
            ExportedAt = new DateTimeOffset(2026, 08, 31, 20, 0, 0, TimeSpan.Zero),
            Club = new MovieClub
            {
                FavouriteMovies = [movie],
                Friends = [friend],
                Groups = [group],
                Locations = [location],
                Events = [movieEvent]
            }
        };
    }

    private static Movie CreateMovie() => new()
    {
        Id = 99,
        Title = "Arrival",
        Overview = "First contact changes everything.",
        PosterPath = "/poster.jpg",
        BackdropPath = "/backdrop.jpg",
        ReleaseDate = "2016-11-11",
        VoteAverage = 8.1,
        Genres = ["Sci-Fi", "Drama"]
    };

    private static Friend CreateFriend(string name, string email) => new()
    {
        Name = name,
        Email = email
    };

    private static Location CreateLocation() => new()
    {
        Name = "Rooftop Cinema",
        Address = "500 Sunset Blvd",
        Notes = "Bring blankets"
    };

    private static MovieEvent CreateMovieEvent(Guid invitedFriendId, Guid? invitedGroupId = null, Location? location = null) => new()
    {
        Movie = CreateMovie(),
        EventDate = new DateTimeOffset(2026, 09, 15, 18, 30, 0, TimeSpan.Zero),
        Location = location ?? CreateLocation(),
        InvitedFriendIds = [invitedFriendId],
        InvitedGroupIds = invitedGroupId is null ? [] : [invitedGroupId.Value],
        Notes = "Meet early for snacks",
        CalendarEventId = "calendar-123",
        Status = EventStatus.Scheduled,
        CreatedAt = new DateTimeOffset(2026, 08, 01, 10, 0, 0, TimeSpan.Zero),
        UpdatedAt = new DateTimeOffset(2026, 08, 02, 12, 15, 0, TimeSpan.Zero)
    };
}
