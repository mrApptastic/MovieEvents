using FluentAssertions;
using MovieEvents.Core.Models;

namespace MovieEvents.Core.Tests.Models;

public class AppStateTests
{
    [Fact]
    public void Create_ShouldReturnDefaultState()
    {
        var state = AppState.Create();

        state.Should().NotBeNull();
        state.UserEmail.Should().BeNull();
        state.UserName.Should().BeNull();
        state.ExportedAt.Should().BeNull();
        state.Club.Should().NotBeNull();
        state.Club.FavouriteMovies.Should().BeEmpty();
        state.Club.Friends.Should().BeEmpty();
        state.Club.Groups.Should().BeEmpty();
        state.Club.Locations.Should().BeEmpty();
        state.Club.Events.Should().BeEmpty();
    }
}
