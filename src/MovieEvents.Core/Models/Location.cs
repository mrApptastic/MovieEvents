namespace MovieEvents.Core.Models;

/// <summary>
/// Represents a location where a movie event can take place.
/// </summary>
public sealed class Location
{
    /// <summary>
    /// Gets or sets the unique identifier for the location.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the display name for the location.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the address for the location.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets any additional notes about the location.
    /// </summary>
    public string? Notes { get; set; }
}
