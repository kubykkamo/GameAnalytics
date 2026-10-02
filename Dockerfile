FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GameAnalytics.slnx", "./"]
COPY ["src/GameAnalytics.Api/GameAnalytics.Api.csproj", "src/GameAnalytics.Api/"]
COPY ["src/GameAnalytics.Application/GameAnalytics.Application.csproj", "src/GameAnalytics.Application/"]
COPY ["src/GameAnalytics.Domain/GameAnalytics.Domain.csproj", "src/GameAnalytics.Domain/"]
COPY ["src/GameAnalytics.Infrastructure/GameAnalytics.Infrastructure.csproj", "src/GameAnalytics.Infrastructure/"]
RUN dotnet restore "src/GameAnalytics.Api/GameAnalytics.Api.csproj"


COPY . .
WORKDIR "/src/src/GameAnalytics.Api"
RUN dotnet publish "GameAnalytics.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GameAnalytics.Api.dll"]