# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj file and restore
COPY ./GAC-WMS.IntegrationSolution.csproj ./
RUN dotnet restore ./GAC-WMS.IntegrationSolution.csproj

# Copy the rest of the files and publish
COPY . .
RUN dotnet publish ./GAC-WMS.IntegrationSolution.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Remove this line: USER app
# → It restricts permissions and may cause Kestrel to fail to bind to port 80

COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:80
# Use port 80 (which is expected by default for ASP.NET Core)
EXPOSE 8085
# Start the app
ENTRYPOINT ["dotnet", "GAC-WMS.IntegrationSolution.dll"]
