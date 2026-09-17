FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Canopy/Canopy.csproj Canopy/
RUN dotnet restore Canopy/Canopy.csproj
COPY . .
RUN dotnet publish Canopy/Canopy.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN mkdir -p /app/keys
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV DATAPROTECTION_KEYS_PATH=/app/keys
EXPOSE 8080
ENTRYPOINT ["dotnet", "Canopy.dll"]
