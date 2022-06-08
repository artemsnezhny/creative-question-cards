FROM mcr.microsoft.com/dotnet/aspnet:6.0.1-focal-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal-arm64v8 AS build
WORKDIR /src
COPY ["src/CreativeQuestionCards.Bots.BackgroundServices/CreativeQuestionCards.Bots.BackgroundServices.csproj", "src/CreativeQuestionCards.Bots.BackgroundServices/"]
RUN dotnet restore "src\CreativeQuestionCards.Bots.BackgroundServices\CreativeQuestionCards.Bots.BackgroundServices.csproj"
COPY . .
WORKDIR "/src/src/CreativeQuestionCards.Bots.BackgroundServices"
RUN dotnet build "CreativeQuestionCards.Bots.BackgroundServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreativeQuestionCards.Bots.BackgroundServices.csproj" -c Release -o /app/publish
#COPY ["/src/CreativeQuestionCards.Bots.BackgroundServices/database.db", "/app/publish"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreativeQuestionCards.Bots.BackgroundServices.dll"]
