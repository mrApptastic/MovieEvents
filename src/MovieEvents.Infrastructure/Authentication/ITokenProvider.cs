namespace MovieEvents.Infrastructure.Authentication;

/// <summary>
/// Provides OAuth access tokens for Google API calls.
/// </summary>
public interface ITokenProvider
{
    /// <summary>Gets the current access token.</summary>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
