# Google API Setup

## Prerequisites

- A Google Cloud account
- A Google Cloud project

## Step 1: Create a Google Cloud Project

1. Go to [Google Cloud Console](https://console.cloud.google.com)
2. Click **New Project**
3. Name it (e.g., "MovieEvents")
4. Click **Create**

## Step 2: Enable APIs

1. Go to **APIs & Services** → **Library**
2. Search for and enable:
   - **Google Calendar API**
   - **Gmail API**

## Step 3: Configure OAuth Consent Screen

1. Go to **APIs & Services** → **OAuth consent screen**
2. Choose **External** user type
3. Fill in the required fields:
   - App name: "MovieEvents"
   - Support email: your email
   - Developer contact: your email
4. Add scopes:
   - `https://www.googleapis.com/auth/calendar.events` (Google Calendar events)
   - `https://www.googleapis.com/auth/gmail.send` (Send emails)
5. Add test users if in testing mode

## Step 4: Create OAuth 2.0 Credentials

1. Go to **APIs & Services** → **Credentials**
2. Click **Create Credentials** → **OAuth 2.0 Client ID**
3. Application type: **Web application**
4. Name: "MovieEvents"
5. Authorized JavaScript origins:
   - `https://yourusername.github.io` (production)
   - `https://localhost:5001` (development)
6. Authorized redirect URIs:
   - `https://yourusername.github.io/MovieEvents/authentication/login-callback`
   - `https://localhost:5001/authentication/login-callback`
7. Click **Create**
8. Note the **Client ID**

## Step 5: Configure the Application

Add the Client ID to your application configuration. The scopes requested are:

- `openid` - Basic OpenID Connect
- `email` - User email address
- `profile` - User profile information
- `https://www.googleapis.com/auth/calendar.events` - Calendar event management
- `https://www.googleapis.com/auth/gmail.send` - Send emails

## Scopes Explanation

| Scope | Purpose |
|-------|---------|
| `calendar.events` | Create, update, and delete calendar events for movie events |
| `gmail.send` | Send invitation, cancellation, and backup emails |

These are the minimum required scopes. The app does not request broader access.
