using Microsoft.Extensions.DependencyInjection;
using MovieEvents.Core.Interfaces;
using MovieEvents.Infrastructure.ApiClients;
using MovieEvents.Infrastructure.Services;
using MovieEvents.Infrastructure.Storage;

namespace MovieEvents.Infrastructure.DependencyInjection;

/// <summary>
/// Extension methods for registering infrastructure services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all MovieEvents infrastructure services.
    /// </summary>
    public static IServiceCollection AddMovieEventsInfrastructure(
        this IServiceCollection services,
        Action<TmdbOptions>? configureTmdb = null)
    {
        if (configureTmdb is not null)
        {
            services.Configure(configureTmdb);
        }

        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddScoped<IAppStateService, AppStateService>();
        services.AddScoped<IThemeService, ThemeService>();
        services.AddScoped<IMovieEventService, MovieEventService>();

        services.AddHttpClient<ITmdbApiClient, TmdbApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.themoviedb.org/");
        });

        services.AddHttpClient<IGoogleCalendarService, GoogleCalendarService>();
        services.AddHttpClient<IGmailService, GmailService>();

        return services;
    }
}
