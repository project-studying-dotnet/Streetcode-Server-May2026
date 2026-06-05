FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS base
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    curl \
    gpg \
    && rm -rf /var/lib/apt/lists/*

EXPOSE 5000
EXPOSE 5001
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
ARG Configuration=Debug

WORKDIR /src

# Restore
COPY ./Streetcode/*.sln ./
COPY ./Streetcode/Streetcode.WebApi/*.csproj ./Streetcode.WebApi/
COPY ./Streetcode/Streetcode.BLL/*.csproj ./Streetcode.BLL/
COPY ./Streetcode/Streetcode.DAL/*.csproj ./Streetcode.DAL/
COPY ./Streetcode/Streetcode.EmailService/*.csproj ./Streetcode.EmailService/
COPY ./Streetcode/DbUpdate/*.csproj ./DbUpdate/
COPY ./Streetcode/Streetcode.XUnitTest/*.csproj ./Streetcode.XUnitTest/
COPY ./Streetcode/Streetcode.XIntegrationTest/*.csproj ./Streetcode.XIntegrationTest/
COPY ./houses.zip ./

RUN dotnet restore Streetcode.WebApi/Streetcode.WebApi.csproj

# Build
COPY ./Streetcode/ ./

RUN dotnet build \
    Streetcode.WebApi/Streetcode.WebApi.csproj \
    -c "$Configuration" \
    --no-restore

# Publish
FROM build AS publish

RUN dotnet publish \
    Streetcode.WebApi/Streetcode.WebApi.csproj \
    -c "$Configuration" \
    -o /app/publish \
    --no-build

# Runtime
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

LABEL atom="Streetcode"

ENTRYPOINT ["dotnet", "Streetcode.WebApi.dll"]