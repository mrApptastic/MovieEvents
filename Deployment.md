# Deployment

## GitHub Pages

MovieEvents is automatically deployed to GitHub Pages via GitHub Actions.

### Workflow

The CI/CD pipeline (`.github/workflows/deploy.yml`) runs on every push to `main`:

1. **Build** - Compiles the solution in Release mode
2. **Test** - Runs all unit tests
3. **Publish** - Creates a production build of the Blazor WASM app
4. **Deploy** - Publishes to GitHub Pages

### Configuration

1. Go to repository **Settings** → **Pages**
2. Set Source to **GitHub Actions**
3. The workflow handles the rest automatically

### Base Path

The workflow automatically rewrites `<base href>` from `/` to `/MovieEvents/` for GitHub Pages subdirectory hosting.

A `404.html` copy of `index.html` is created to handle SPA routing.

### Environment Variables

For production, set the following in your repository secrets or the `appsettings.json`:

- `Tmdb:ApiKey` - Your TMDb API key
- Google OAuth client ID (configured in `index.html`)

### Manual Deployment

```bash
dotnet publish src/MovieEvents.App/MovieEvents.App.csproj -c Release -o release
# Deploy the contents of release/wwwroot/ to your web server
```

### Requirements

- .NET 10 SDK
- GitHub Pages enabled on the repository
- Repository Actions permissions for Pages deployment
