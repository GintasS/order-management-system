# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first (enables layer caching)
COPY WebApp/WebApp.csproj WebApp/
COPY Core/Core.csproj Core/

# Restore dependencies (resolves project references)
RUN dotnet restore WebApp/WebApp.csproj

# Copy everything else
COPY . .

# Build
WORKDIR /src/WebApp
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "CSS.Challenge.Web.dll"]