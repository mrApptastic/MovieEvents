# TMDb API Setup

## Overview

MovieEvents uses [The Movie Database (TMDb)](https://www.themoviedb.org/) API to search movies, browse details, and display posters.

## Step 1: Create an Account

1. Go to [themoviedb.org](https://www.themoviedb.org/)
2. Create a free account

## Step 2: Get an API Key

1. Go to [Settings → API](https://www.themoviedb.org/settings/api)
2. Request an API key
3. Choose **Developer** for personal use
4. Fill in the application details
5. Copy your **API Key (v3 auth)**

## Step 3: Configure the Application

Add your API key to `src/MovieEvents.App/wwwroot/appsettings.json`:

```json
{
  "Tmdb": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

**Warning**: Do not commit your API key to a public repository. For production, use environment variables or GitHub Secrets.

## API Endpoints Used

| Endpoint | Purpose |
|----------|---------|
| `GET /3/search/movie` | Search movies by title |
| `GET /3/movie/{id}` | Get movie details |
| `GET /3/configuration` | Get image base URLs and sizes |

## Rate Limits

TMDb allows approximately 40 requests per 10 seconds. The app performs minimal requests per user action.

## Image URLs

Movie posters and backdrops are served from TMDb's image CDN:
- Base URL: `https://image.tmdb.org/t/p/`
- Poster sizes: `w92`, `w154`, `w185`, `w300`, `w342`, `w500`, `w780`, `original`
- The app uses `w300` for poster thumbnails
