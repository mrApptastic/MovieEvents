namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a user's movie club containing all their data.
/// </summary>
public sealed class MovieClub
{
    /// <summary>Gets or sets the favourite movies.</summary>
    public List<Movie> FavouriteMovies { get; set; } = [];

    /// <summary>Gets or sets the friends.</summary>
    public List<Friend> Friends { get; set; } = [];

    /// <summary>Gets or sets the friend groups.</summary>
    public List<FriendGroup> Groups { get; set; } = [];

    /// <summary>Gets or sets the locations.</summary>
    public List<Location> Locations { get; set; } = [];

    /// <summary>Gets or sets the events.</summary>
    public List<MovieEvent> Events { get; set; } = [];

    /// <summary>Adds a movie to favourites if not already present.</summary>
    public bool AddFavourite(Movie movie)
    {
        if (FavouriteMovies.Any(m => m.Id == movie.Id)) return false;
        FavouriteMovies.Add(movie);
        return true;
    }

    /// <summary>Removes a movie from favourites by TMDb ID.</summary>
    public bool RemoveFavourite(int movieId) =>
        FavouriteMovies.RemoveAll(m => m.Id == movieId) > 0;

    /// <summary>Adds a friend.</summary>
    public bool AddFriend(Friend friend)
    {
        if (Friends.Any(f => f.Id == friend.Id)) return false;
        Friends.Add(friend);
        return true;
    }

    /// <summary>Removes a friend by ID.</summary>
    public bool RemoveFriend(Guid friendId) =>
        Friends.RemoveAll(f => f.Id == friendId) > 0;

    /// <summary>Adds a group.</summary>
    public bool AddGroup(FriendGroup group)
    {
        if (Groups.Any(g => g.Id == group.Id)) return false;
        Groups.Add(group);
        return true;
    }

    /// <summary>Removes a group by ID.</summary>
    public bool RemoveGroup(Guid groupId) =>
        Groups.RemoveAll(g => g.Id == groupId) > 0;

    /// <summary>Adds a location.</summary>
    public bool AddLocation(Location location)
    {
        if (Locations.Any(l => l.Id == location.Id)) return false;
        Locations.Add(location);
        return true;
    }

    /// <summary>Removes a location by ID.</summary>
    public bool RemoveLocation(Guid locationId) =>
        Locations.RemoveAll(l => l.Id == locationId) > 0;

    /// <summary>Adds an event.</summary>
    public bool AddEvent(MovieEvent movieEvent)
    {
        if (Events.Any(e => e.Id == movieEvent.Id)) return false;
        Events.Add(movieEvent);
        return true;
    }

    /// <summary>Removes an event by ID.</summary>
    public bool RemoveEvent(Guid eventId) =>
        Events.RemoveAll(e => e.Id == eventId) > 0;

    /// <summary>Gets an event by ID.</summary>
    public MovieEvent? GetEvent(Guid eventId) =>
        Events.FirstOrDefault(e => e.Id == eventId);

    /// <summary>Gets a friend by ID.</summary>
    public Friend? GetFriend(Guid friendId) =>
        Friends.FirstOrDefault(f => f.Id == friendId);

    /// <summary>Gets a group by ID.</summary>
    public FriendGroup? GetGroup(Guid groupId) =>
        Groups.FirstOrDefault(g => g.Id == groupId);

    /// <summary>Gets a location by ID.</summary>
    public Location? GetLocation(Guid locationId) =>
        Locations.FirstOrDefault(l => l.Id == locationId);
}
