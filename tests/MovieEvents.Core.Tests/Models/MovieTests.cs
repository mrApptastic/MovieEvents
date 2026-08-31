using FluentAssertions;
using MovieEvents.Core.Models;

namespace MovieEvents.Core.Tests.Models;

public class MovieTests
{
    [Fact]
    public void GetPosterUrl_ShouldReturnComposedUrl_WhenPosterPathExists()
    {
        var movie = new Movie
        {
            PosterPath = "/poster.jpg"
        };

        var url = movie.GetPosterUrl("https://image.tmdb.org/t/p/", "w500");

        url.Should().Be("https://image.tmdb.org/t/p/w500/poster.jpg");
    }

    [Fact]
    public void GetPosterUrl_ShouldReturnEmptyString_WhenPosterPathIsNull()
    {
        var movie = new Movie
        {
            PosterPath = null
        };

        var url = movie.GetPosterUrl("https://image.tmdb.org/t/p/", "w500");

        url.Should().Be(string.Empty);
    }

    [Fact]
    public void GetPosterUrl_ShouldUseProvidedBaseAndSize_WhenPosterPathIsEmpty()
    {
        var movie = new Movie
        {
            PosterPath = string.Empty
        };

        var url = movie.GetPosterUrl("https://image.tmdb.org/t/p/", "w500");

        url.Should().Be("https://image.tmdb.org/t/p/w500");
    }

    [Fact]
    public void GetBackdropUrl_ShouldReturnComposedUrl_WhenBackdropPathExists()
    {
        var movie = new Movie
        {
            BackdropPath = "/backdrop.jpg"
        };

        var url = movie.GetBackdropUrl("https://image.tmdb.org/t/p/", "original");

        url.Should().Be("https://image.tmdb.org/t/p/original/backdrop.jpg");
    }

    [Fact]
    public void GetBackdropUrl_ShouldReturnEmptyString_WhenBackdropPathIsNull()
    {
        var movie = new Movie
        {
            BackdropPath = null
        };

        var url = movie.GetBackdropUrl("https://image.tmdb.org/t/p/", "original");

        url.Should().Be(string.Empty);
    }

    [Fact]
    public void GetBackdropUrl_ShouldUseProvidedBaseAndSize_WhenBackdropPathIsEmpty()
    {
        var movie = new Movie
        {
            BackdropPath = string.Empty
        };

        var url = movie.GetBackdropUrl("https://image.tmdb.org/t/p/", "original");

        url.Should().Be("https://image.tmdb.org/t/p/original");
    }
}
