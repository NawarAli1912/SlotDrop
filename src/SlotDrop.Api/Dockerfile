# Build context is the repo root: the API needs the solution-wide props, global.json and the other src projects.
FROM mcr.microsoft.com/dotnet/sdk:10.0.403-noble AS build
WORKDIR /src

# Restore first, from project files only, so this layer stays cached until a dependency changes.
COPY global.json Directory.Build.props Directory.Packages.props .editorconfig ./
COPY src/SlotDrop.Domain/SlotDrop.Domain.csproj src/SlotDrop.Domain/packages.lock.json src/SlotDrop.Domain/
COPY src/SlotDrop.Application/SlotDrop.Application.csproj src/SlotDrop.Application/packages.lock.json src/SlotDrop.Application/
COPY src/SlotDrop.Infrastructure/SlotDrop.Infrastructure.csproj src/SlotDrop.Infrastructure/packages.lock.json src/SlotDrop.Infrastructure/
COPY src/SlotDrop.Api/SlotDrop.Api.csproj src/SlotDrop.Api/packages.lock.json src/SlotDrop.Api/
RUN dotnet restore src/SlotDrop.Api/SlotDrop.Api.csproj --locked-mode

COPY src/ src/
RUN dotnet publish src/SlotDrop.Api/SlotDrop.Api.csproj --no-restore -c Release -o /app

# Chiseled: no shell or package manager, runs as the non-root "app" user, listens on 8080.
FROM mcr.microsoft.com/dotnet/aspnet:10.0.14-noble-chiseled AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SlotDrop.Api.dll"]
