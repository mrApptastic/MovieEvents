using System.Text.Json;
using System.Text.Json.Serialization;
using MovieEvents.Core.Models;

namespace MovieEvents.Infrastructure.Serialization;

/// <summary>
/// Source-generated JSON serialization context for the application.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AppState))]
[JsonSerializable(typeof(MovieClub))]
[JsonSerializable(typeof(Movie))]
[JsonSerializable(typeof(Friend))]
[JsonSerializable(typeof(FriendGroup))]
[JsonSerializable(typeof(Location))]
[JsonSerializable(typeof(MovieEvent))]
[JsonSerializable(typeof(TmdbSearchResult))]
[JsonSerializable(typeof(TmdbConfiguration))]
[JsonSerializable(typeof(List<Movie>))]
[JsonSerializable(typeof(List<Friend>))]
[JsonSerializable(typeof(List<FriendGroup>))]
[JsonSerializable(typeof(List<Location>))]
[JsonSerializable(typeof(List<MovieEvent>))]
[JsonSerializable(typeof(string))]
public sealed partial class AppJsonContext : JsonSerializerContext
{
}
