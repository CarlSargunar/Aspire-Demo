FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnalyticsApp/AnalyticsApp.csproj", "."]
RUN dotnet restore "./AnalyticsApp.csproj"
COPY "./AnalyticsApp" .
RUN dotnet build "./AnalyticsApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./AnalyticsApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY AnalyticsApp/nginx.conf /etc/nginx/nginx.conf