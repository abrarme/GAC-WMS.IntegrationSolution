# Use the ASP.NET runtime image for base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage using SDK image with git installation
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install git for restoring from Git-based dependencies
RUN apt-get update && apt-get install -y git

# Copy and restore project
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GAC-WMS.IntegrationSolution.dll"]