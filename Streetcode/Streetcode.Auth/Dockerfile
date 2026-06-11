FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Streetcode.Auth/Streetcode.Auth.csproj", "Streetcode.Auth/"]
COPY ["Streetcode.Common/Streetcode.Common.csproj", "Streetcode.Common/"]

RUN dotnet restore "Streetcode.Auth/Streetcode.Auth.csproj"

COPY . .

WORKDIR "/src/Streetcode.Auth"

RUN dotnet publish "Streetcode.Auth.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

USER 1654

ENTRYPOINT ["dotnet", "Streetcode.Auth.dll"]