# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

# Copy the csproj and restore as distinct layers
COPY EMLeaderboard/*.csproj ./
RUN dotnet restore

# Copy the entire project and build the release
COPY EMLeaderboard/. ./
RUN dotnet publish -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=base /app/out .

# Expose the port the app runs on
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "EMLeaderboard.dll"] 