namespace MovieEvents.Core.Models;

/// <summary>
/// Represents the complete serializable application state.
/// </summary>
public sealed class AppState
{
    /// <summary>Gets or sets the signed-in user's email.</summary>
    public string? UserEmail { get; set; }

    /// <summary>Gets or sets the signed-in user's name.</summary>
    public string? UserName { get; set; }

    /// <summary>Gets or sets the user's movie club.</summary>
    public MovieClub Club { get; set; } = new();

    /// <summary>Gets or sets the export timestamp.</summary>
    public DateTimeOffset? ExportedAt { get; set; }

    /// <summary>Creates a new default application state.</summary>
    public static AppState Create() => new();
}
