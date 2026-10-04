# GameAnalytics API

A backend service for looking up Valorant player stats and match history using the Riot Games API.

**Deployed on:** Google Cloud Run  
**Made with:** C# .NET 10, ASP.NET Core, SQLite

---

## What It Does

- Get a player's profile and statistics
- Retrieve match history for a player
- View detailed stats from a specific match (kills, deaths, assists, headshot %, etc.)
- Rate limit API requests to avoid hitting Riot's limits

It's pretty straightforward - the API sits between you and Riot's servers, fetches data, calculates some stats, and returns it.

---

## How It's Organized

The code is split into layers:

```
GameAnalytics.Api              → API endpoints and controllers
GameAnalytics.Application      → Business logic (calculating stats)
GameAnalytics.Domain           → Data models and interfaces
GameAnalytics.Infrastructure   → Database and API calls
```

Each layer has a specific job - keeps things organized and testable.

---

## Getting Started

### What You Need
- .NET 10 SDK ([download here](https://dotnet.microsoft.com/download))
- A Riot (unofficial) API key ([get one free here](https://docs.henrikdev.xyz/general/auth))
- Docker (optional, for running containerized)

### Running Locally

```bash
# Clone the repo
git clone <repo-url>
cd GameAnalytics

# Set up your API key
cp .env.example .env
nano .env  # Add your Riot API key

# Install and run
dotnet restore
dotnet run --project src/GameAnalytics.Api
```

Visit `http://localhost:5001/swagger` to see available endpoints.

### Running with Docker

```bash
cp .env.example .env
nano .env  # Add your API key

docker-compose -f compose.yaml up
```

Then visit `http://localhost:8080/swagger`

---

## API Endpoints

### Get Player PUUID
```
GET /api/users/puuid/{gameName}/{tagLine}
```
Returns a player's unique ID.

Example: `GET /api/users/puuid/Henrik/NA1`

---

### Get Match History
```
GET /api/users/match-history/{gameName}/{tagLine}?limit=10
```
Returns list of recent match IDs for a player.

---

### Get Match Details
```
GET /api/users/match/{matchId}
```
Returns stats from a specific match (kills, deaths, K/D ratio, headshots, etc.)


---

## Environment Variables

Create a `.env` file (copy from `.env.example`):

```env
HenrikApi__ApiKey=your-api-key-here
ConnectionStrings__DefaultConnection=Data Source=analytics.db
ASPNETCORE_ENVIRONMENT=Development
```

That's the minimum you need. See `.env.example` for all options.

---

## Tests

```bash
dotnet test
```

Tests include:
- Stat calculations (K/D ratio, headshot %)
- API response parsing
- Error handling

---

## Docker & Deployment

### Build and Run Locally
```bash
docker build -t gameanalytics:latest .
docker run -p 8080:8080 -e HenrikApi__ApiKey=your-key gameanalytics:latest
```

### Deploy to Google Cloud
```bash
gcloud run deploy gameanalytics \
  --source . \
  --region us-central1 \
  --set-env-vars HenrikApi__ApiKey=your-key
```

For detailed deployment instructions, see [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md)

---

## Project Structure

```
src/
├── GameAnalytics.Api/           Controllers & API setup
├── GameAnalytics.Application/   Business logic (stat calculations)
├── GameAnalytics.Domain/        Data models & interfaces
└── GameAnalytics.Infrastructure Database & external API calls

tests/                           Unit tests
docs/                           API & deployment documentation
```

---

## Tech Stack

- **.NET 10** & **ASP.NET Core** 
- **SQLite** for database
- **Entity Framework Core** for ORM
- **xUnit** & **Moq** for tests
- **Docker** for containerization
- **Google Cloud Run** for hosting
- **Henrik API** for player data

---

## What's Included

- **Rate limiting** - Limits requests to avoid hitting API quotas
- **Error handling** - Returns proper HTTP status codes and error messages
- **Stat calculations** - K/D ratio, K/D/A ratio, headshot percentages
- **Tests** - Unit tests for calculations and API integration
- **Database** - Stores user data with Entity Framework migrations
- **Docker** - Containerized for easy deployment

---

## Future Ideas

- User authentication (JWT) + OAuth 2.0
- Caching with Redis
- Real-time updates (WebSocket)
- PostgreSQL for better scaling
- Structured logging
- Background service for fetching and storing pro matches

---

## Security Notes

- API keys stored in environment variables (not in code)
- Rate limiting prevents abuse
- Error messages don't leak sensitive info
- Input validation on all endpoints

---

## Testing the API

Use Swagger UI at `/swagger` to test endpoints interactively.

Or with cURL:
```bash
curl -X GET "http://localhost:5001/api/users/puuid/kubykkamo/EUNE"
```

Or with Postman - import the Swagger endpoint.

---

## Limitations & Notes

- Uses SQLite which is fine for development but not ideal for production
- Data is lost if the container restarts (no persistence volume)
- No authentication - anyone can call the API
- Rate limited to 15 requests/minute to avoid hitting Riot's quotas

---


## Notes

This is a personal project built while learning .NET development. It's not production-grade but it works and demonstrates the basics of backend API development.
