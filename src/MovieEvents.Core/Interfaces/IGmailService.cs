using MovieEvents.Core.Models;
using MovieEvents.Core.Results;

namespace MovieEvents.Core.Interfaces;

/// <summary>
/// Sends emails via the Gmail API.
/// </summary>
public interface IGmailService
{
    /// <summary>Sends invitation emails to all invited friends.</summary>
    Task<Result> SendInvitationAsync(MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default);

    /// <summary>Sends cancellation emails to all invited friends.</summary>
    Task<Result> SendCancellationAsync(MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default);

    /// <summary>Sends a JSON backup to the user's own email.</summary>
    Task<Result> SendBackupAsync(string jsonData, string userEmail, CancellationToken cancellationToken = default);
}
