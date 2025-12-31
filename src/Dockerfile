# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files first (enables layer caching)
COPY CSS.Challenge.Web/CSS.Challenge.Web.csproj CSS.Challenge.Web/
COPY CSS.Challenge.Domain/CSS.Challenge.Domain.csproj CSS.Challenge.Domain/

# Restore dependencies (resolves project references)
RUN dotnet restore CSS.Challenge.Web/CSS.Challenge.Web.csproj

# Copy everything else
COPY . .

# Build
WORKDIR /src/CSS.Challenge.Web
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0

ARG ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80

# Set the entry point for the application
ENTRYPOINT ["dotnet", "CSS.Challenge.Web.dll"]