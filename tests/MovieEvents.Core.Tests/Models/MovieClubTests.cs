using FluentAssertions;
using MovieEvents.Core.Models;

namespace MovieEvents.Core.Tests.Models;

public class MovieClubTests
{
    [Fact]
    public void AddFavourite_ShouldAddMovie_WhenMovieIsNew()
    {
        var club = new MovieClub();
        var movie = CreateMovie(42);

        var added = club.AddFavourite(movie);

        added.Should().BeTrue();
        club.FavouriteMovies.Should().ContainSingle().Which.Should().BeSameAs(movie);
    }

    [Fact]
    public void AddFavourite_ShouldReturnFalse_WhenMovieAlreadyExists()
    {
        var movie = CreateMovie(42);
        var club = new MovieClub
        {
            FavouriteMovies = [movie]
        };

        var added = club.AddFavourite(CreateMovie(42, "Duplicate"));

        added.Should().BeFalse();
        club.FavouriteMovies.Should().ContainSingle().Which.Should().BeSameAs(movie);
    }

    [Fact]
    public void RemoveFavourite_ShouldRemoveAllMatchingMovies_AndReturnFalseWhenNotFound()
    {
        var club = new MovieClub
        {
            FavouriteMovies =
            [
                CreateMovie(42, "First Copy"),
                CreateMovie(42, "Second Copy"),
                CreateMovie(7, "Remaining Movie")
            ]
        };

        var removed = club.RemoveFavourite(42);
        var missing = club.RemoveFavourite(999);

        removed.Should().BeTrue();
        missing.Should().BeFalse();
        club.FavouriteMovies.Should().ContainSingle();
        club.FavouriteMovies.Single().Id.Should().Be(7);
    }

    [Fact]
    public void FriendMethods_ShouldAddRejectDuplicatesAndRemoveById()
    {
        var friend = CreateFriend("Taylor");
        var club = new MovieClub();

        var firstAdd = club.AddFriend(friend);
        var duplicateAdd = club.AddFriend(new Friend { Id = friend.Id, Name = "Duplicate", Email = "duplicate@example.com" });
        var removed = club.RemoveFriend(friend.Id);
        var missing = club.RemoveFriend(Guid.NewGuid());

        firstAdd.Should().BeTrue();
        duplicateAdd.Should().BeFalse();
        removed.Should().BeTrue();
        missing.Should().BeFalse();
        club.Friends.Should().BeEmpty();
    }

    [Fact]
    public void GroupMethods_ShouldAddRejectDuplicatesAndRemoveById()
    {
        var group = CreateGroup("Weekend Crew");
        var club = new MovieClub();

        var firstAdd = club.AddGroup(group);
        var duplicateAdd = club.AddGroup(new FriendGroup { Id = group.Id, Name = "Duplicate Group" });
        var removed = club.RemoveGroup(group.Id);
        var missing = club.RemoveGroup(Guid.NewGuid());

        firstAdd.Should().BeTrue();
        duplicateAdd.Should().BeFalse();
        removed.Should().BeTrue();
        missing.Should().BeFalse();
        club.Groups.Should().BeEmpty();
    }

    [Fact]
    public void LocationMethods_ShouldAddRejectDuplicatesAndRemoveById()
    {
        var location = CreateLocation("Cinema");
        var club = new MovieClub();

        var firstAdd = club.AddLocation(location);
        var duplicateAdd = club.AddLocation(new Location { Id = location.Id, Name = "Duplicate Location", Address = "Other Address" });
        var removed = club.RemoveLocation(location.Id);
        var missing = club.RemoveLocation(Guid.NewGuid());

        firstAdd.Should().BeTrue();
        duplicateAdd.Should().BeFalse();
        removed.Should().BeTrue();
        missing.Should().BeFalse();
        club.Locations.Should().BeEmpty();
    }

    [Fact]
    public void EventMethods_ShouldAddRejectDuplicatesAndRemoveById()
    {
        var movieEvent = CreateMovieEvent();
        var club = new MovieClub();

        var firstAdd = club.AddEvent(movieEvent);
        var duplicateAdd = club.AddEvent(new MovieEvent { Id = movieEvent.Id });
        var removed = club.RemoveEvent(movieEvent.Id);
        var missing = club.RemoveEvent(Guid.NewGuid());

        firstAdd.Should().BeTrue();
        duplicateAdd.Should().BeFalse();
        removed.Should().BeTrue();
        missing.Should().BeFalse();
        club.Events.Should().BeEmpty();
    }

    [Fact]
    public void GetEvent_ShouldReturnMatchingEvent_OrNullWhenMissing()
    {
        var movieEvent = CreateMovieEvent();
        var club = new MovieClub
        {
            Events = [movieEvent]
        };

        club.GetEvent(movieEvent.Id).Should().BeSameAs(movieEvent);
        club.GetEvent(Guid.NewGuid()).Should().BeNull();
    }

    [Fact]
    public void GetFriend_ShouldReturnMatchingFriend_OrNullWhenMissing()
    {
        var friend = CreateFriend("Jordan");
        var club = new MovieClub
        {
            Friends = [friend]
        };

        club.GetFriend(friend.Id).Should().BeSameAs(friend);
        club.GetFriend(Guid.NewGuid()).Should().BeNull();
    }

    [Fact]
    public void GetGroup_ShouldReturnMatchingGroup_OrNullWhenMissing()
    {
        var group = CreateGroup("Friends");
        var club = new MovieClub
        {
            Groups = [group]
        };

        club.GetGroup(group.Id).Should().BeSameAs(group);
        club.GetGroup(Guid.NewGuid()).Should().BeNull();
    }

    [Fact]
    public void GetLocation_ShouldReturnMatchingLocation_OrNullWhenMissing()
    {
        var location = CreateLocation("Home Theater");
        var club = new MovieClub
        {
            Locations = [location]
        };

        club.GetLocation(location.Id).Should().BeSameAs(location);
        club.GetLocation(Guid.NewGuid()).Should().BeNull();
    }

    private static Movie CreateMovie(int id, string? title = null) => new()
    {
        Id = id,
        Title = title ?? $"Movie {id}",
        Overview = "Overview"
    };

    private static Friend CreateFriend(string name) => new()
    {
        Name = name,
        Email = $"{name.ToLowerInvariant()}@example.com"
    };

    private static FriendGroup CreateGroup(string name) => new()
    {
        Name = name
    };

    private static Location CreateLocation(string name) => new()
    {
        Name = name,
        Address = "123 Main Street"
    };

    private static MovieEvent CreateMovieEvent() => new()
    {
        Movie = CreateMovie(100, "Event Movie"),
        EventDate = new DateTimeOffset(2026, 08, 31, 19, 0, 0, TimeSpan.Zero),
        Location = CreateLocation("Downtown Cinema")
    };
}
