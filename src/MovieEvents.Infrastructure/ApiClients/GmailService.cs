using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using MovieEvents.Core.Interfaces;
using MovieEvents.Core.Models;
using MovieEvents.Core.Results;
using MovieEvents.Infrastructure.Authentication;

namespace MovieEvents.Infrastructure.ApiClients;

/// <summary>
/// Gmail API v1 client for sending invitation and cancellation emails.
/// </summary>
public sealed class GmailService : IGmailService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenProvider _tokenProvider;
    private readonly ILogger<GmailService> _logger;

    /// <summary>Initializes a new instance of <see cref="GmailService"/>.</summary>
    public GmailService(HttpClient httpClient, ITokenProvider tokenProvider, ILogger<GmailService> logger)
    {
        _httpClient = httpClient;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result> SendInvitationAsync(MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthHeaderAsync(cancellationToken);
            var icsContent = GenerateIcs(movieEvent, "REQUEST");
            var htmlBody = GenerateInvitationHtml(movieEvent);

            foreach (var friend in friends)
            {
                var mime = BuildMimeMessage(friend.Email, $"Movie Event: {movieEvent.Movie.Title}", htmlBody, icsContent);
                await SendMessageAsync(mime, cancellationToken);
            }

            _logger.LogInformation("Sent invitation emails for event {EventId} to {Count} friends", movieEvent.Id, friends.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invitation emails");
            return Result.Failure($"Failed to send invitations: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> SendCancellationAsync(MovieEvent movieEvent, List<Friend> friends, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthHeaderAsync(cancellationToken);
            var icsContent = GenerateIcs(movieEvent, "CANCEL");
            var htmlBody = GenerateCancellationHtml(movieEvent);

            foreach (var friend in friends)
            {
                var mime = BuildMimeMessage(friend.Email, $"Cancelled: {movieEvent.Movie.Title}", htmlBody, icsContent);
                await SendMessageAsync(mime, cancellationToken);
            }

            _logger.LogInformation("Sent cancellation emails for event {EventId} to {Count} friends", movieEvent.Id, friends.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send cancellation emails");
            return Result.Failure($"Failed to send cancellations: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result> SendBackupAsync(string jsonData, string userEmail, CancellationToken cancellationToken = default)
    {
        try
        {
            await SetAuthHeaderAsync(cancellationToken);
            var htmlBody = "<h2>MovieEvents Backup</h2><p>Your MovieEvents data backup is attached.</p>";
            var mime = BuildMimeMessageWithAttachment(userEmail, "MovieEvents Backup",
                htmlBody, "movieevents-backup.json", "application/json", jsonData);
            await SendMessageAsync(mime, cancellationToken);

            _logger.LogInformation("Sent backup to {Email}", userEmail);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send backup email");
            return Result.Failure($"Failed to send backup: {ex.Message}");
        }
    }

    private async Task SetAuthHeaderAsync(CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task SendMessageAsync(string mimeMessage, CancellationToken cancellationToken)
    {
        var base64Url = Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeMessage))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');

        var response = await _httpClient.PostAsJsonAsync(
            "https://www.googleapis.com/gmail/v1/users/me/messages/send",
            new { raw = base64Url }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private static string GenerateInvitationHtml(MovieEvent evt)
    {
        var posterHtml = !string.IsNullOrEmpty(evt.Movie.PosterPath)
            ? $"<img src=\"{evt.Movie.GetPosterUrl("https://image.tmdb.org/t/p/", "w300")}\" alt=\"{evt.Movie.Title}\" style=\"max-width:300px;\" />"
            : string.Empty;

        return $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h1 style="color: #333;">🎬 Movie Event Invitation</h1>
                {posterHtml}
                <h2>{evt.Movie.Title}</h2>
                <table style="border-collapse: collapse; width: 100%;">
                    <tr><td style="padding: 8px; font-weight: bold;">📅 Date:</td><td style="padding: 8px;">{evt.EventDate:MMMM dd, yyyy}</td></tr>
                    <tr><td style="padding: 8px; font-weight: bold;">🕐 Time:</td><td style="padding: 8px;">{evt.EventDate:HH:mm}</td></tr>
                    <tr><td style="padding: 8px; font-weight: bold;">📍 Location:</td><td style="padding: 8px;">{evt.Location.Name} - {evt.Location.Address}</td></tr>
                    {(string.IsNullOrEmpty(evt.Notes) ? "" : $"<tr><td style=\"padding: 8px; font-weight: bold;\">📝 Notes:</td><td style=\"padding: 8px;\">{evt.Notes}</td></tr>")}
                </table>
                <p style="margin-top: 20px;">Please reply to let us know if you can attend!</p>
            </div>
            """;
    }

    private static string GenerateCancellationHtml(MovieEvent evt) => $"""
        <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
            <h1 style="color: #cc0000;">❌ Movie Event Cancelled</h1>
            <h2>{evt.Movie.Title}</h2>
            <p>The movie event originally scheduled for <strong>{evt.EventDate:MMMM dd, yyyy}</strong> at <strong>{evt.Location.Name}</strong> has been cancelled.</p>
            <p>We apologize for any inconvenience.</p>
        </div>
        """;

    private static string GenerateIcs(MovieEvent evt, string method)
    {
        var uid = evt.CalendarEventId ?? evt.Id.ToString();
        var dtStart = evt.EventDate.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'");
        var dtEnd = evt.EventDate.AddHours(3).UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'");
        var now = DateTimeOffset.UtcNow.UtcDateTime.ToString("yyyyMMdd'T'HHmmss'Z'");
        var status = method == "CANCEL" ? "CANCELLED" : "CONFIRMED";

        return $"""
            BEGIN:VCALENDAR
            VERSION:2.0
            PRODID:-//MovieEvents//EN
            METHOD:{method}
            BEGIN:VEVENT
            UID:{uid}
            DTSTART:{dtStart}
            DTEND:{dtEnd}
            DTSTAMP:{now}
            SUMMARY:Movie: {evt.Movie.Title}
            LOCATION:{evt.Location.Address}
            DESCRIPTION:{evt.Notes ?? ""}
            STATUS:{status}
            END:VEVENT
            END:VCALENDAR
            """;
    }

    private static string BuildMimeMessage(string to, string subject, string htmlBody, string icsContent)
    {
        var boundary = $"boundary_{Guid.NewGuid():N}";
        var sb = new StringBuilder();
        sb.AppendLine($"To: {to}");
        sb.AppendLine($"Subject: {subject}");
        sb.AppendLine("MIME-Version: 1.0");
        sb.AppendLine($"Content-Type: multipart/mixed; boundary=\"{boundary}\"");
        sb.AppendLine();
        sb.AppendLine($"--{boundary}");
        sb.AppendLine("Content-Type: text/html; charset=UTF-8");
        sb.AppendLine();
        sb.AppendLine(htmlBody);
        sb.AppendLine($"--{boundary}");
        sb.AppendLine("Content-Type: text/calendar; charset=UTF-8; method=REQUEST");
        sb.AppendLine("Content-Disposition: attachment; filename=\"event.ics\"");
        sb.AppendLine();
        sb.AppendLine(icsContent);
        sb.AppendLine($"--{boundary}--");
        return sb.ToString();
    }

    private static string BuildMimeMessageWithAttachment(string to, string subject, string htmlBody, string filename, string contentType, string content)
    {
        var boundary = $"boundary_{Guid.NewGuid():N}";
        var sb = new StringBuilder();
        sb.AppendLine($"To: {to}");
        sb.AppendLine($"Subject: {subject}");
        sb.AppendLine("MIME-Version: 1.0");
        sb.AppendLine($"Content-Type: multipart/mixed; boundary=\"{boundary}\"");
        sb.AppendLine();
        sb.AppendLine($"--{boundary}");
        sb.AppendLine("Content-Type: text/html; charset=UTF-8");
        sb.AppendLine();
        sb.AppendLine(htmlBody);
        sb.AppendLine($"--{boundary}");
        sb.AppendLine($"Content-Type: {contentType}");
        sb.AppendLine($"Content-Disposition: attachment; filename=\"{filename}\"");
        sb.AppendLine("Content-Transfer-Encoding: base64");
        sb.AppendLine();
        sb.AppendLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(content)));
        sb.AppendLine($"--{boundary}--");
        return sb.ToString();
    }
}
