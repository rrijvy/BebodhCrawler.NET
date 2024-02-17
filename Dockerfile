FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./BebodhCrawler/BebodhCrawler.csproj" --disable-parallel
RUN dotnet publish "./BebodhCrawler/BebodhCrawler.csproj" -c release -o /app --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80

ENTRYPOINT ["dotnet", "BebodhCrawler.dll"]