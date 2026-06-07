FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Streetcode.EmailService/Streetcode.EmailService.csproj", "Streetcode.EmailService/"]
RUN dotnet restore "Streetcode.EmailService/Streetcode.EmailService.csproj"

COPY ["Streetcode.EmailService/", "Streetcode.EmailService/"]

WORKDIR "/src/Streetcode.EmailService"
RUN dotnet publish "Streetcode.EmailService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

USER 1654

ENTRYPOINT ["dotnet", "Streetcode.EmailService.dll"]