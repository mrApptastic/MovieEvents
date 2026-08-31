using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MovieEvents.App;
using MovieEvents.Infrastructure.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMovieEventsInfrastructure(options =>
{
    options.ApiKey = builder.Configuration["Tmdb:ApiKey"] ?? string.Empty;
});

await builder.Build().RunAsync();
