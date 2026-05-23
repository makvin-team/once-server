FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

RUN apt-get update && apt-get install -y --no-install-recommends \
    cabextract \
    curl \
    fontconfig \
    fonts-dejavu-core \
    fonts-dejavu-extra \
    fonts-liberation \
    fonts-noto-cjk \
    fonts-noto-core \
    icu-devtools \
    libc6 \
    libfontconfig1 \
    libfreetype6 \
    libgbm1 \
    libgdiplus \
    libglu1-mesa \
    libharfbuzz0b \
    libpng16-16 \
    libwebp7 \
    libx11-6 \
    tzdata \
    xfonts-utils \
    && fc-cache -fv \
    && rm -rf /var/lib/apt/lists/*

ENV TZ=Asia/Tashkent \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/Once.Domain/*.csproj          ./Once.Domain/
COPY src/Once.Application/*.csproj     ./Once.Application/
COPY src/Once.Infrastructure/*.csproj  ./Once.Infrastructure/
COPY src/Once.Api/*.csproj             ./Once.Api/

WORKDIR /src/Once.Api
RUN dotnet restore --verbosity minimal

FROM restore AS publish
ARG BUILD_CONFIGURATION=Release

COPY src/ /src/
WORKDIR /src/Once.Api

RUN dotnet publish "Once.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish --chown=appuser:appgroup /app/publish .

ARG ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Once.Api.dll"]
