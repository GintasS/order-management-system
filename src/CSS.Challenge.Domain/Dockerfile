# Use the .NET SDK image to build the library
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY ./*.csproj ./
RUN dotnet restore

# Copy the rest of the library files and build the library
COPY . ./
RUN dotnet build -c Release -o out

# This is a library, so no runtime image or entry point is needed