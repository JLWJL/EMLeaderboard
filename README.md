# EMLeaderboard API

A RESTful API service that manages a real-time customer leaderboard system. The service tracks customer scores and provides ranking functionality with support for efficient score updates and flexible leaderboard queries.

## Features

- Update customer scores with positive or negative values
- Retrieve customers by rank range
- Get customer rankings with configurable number of higher and lower ranked neighbors
- Thread-safe score updates with concurrent access support
- Exception handling with detailed error responses


## Development

### Prerequisites
- .NET 8.0 SDK
- Docker (optional)

### Local Setup
1. Clone the repository
2. Navigate to project directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```
5. Run the project:
   ```bash
   dotnet run
   ```

### Running Tests
```bash
dotnet test
```

#### Using Docker
1. Build the Docker image:
```bash 
docker build -t emleaderboard .
```

2. Run the container:
```bash
docker run -p 8080:8080 emleaderboard
```