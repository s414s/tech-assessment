# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BookingSystem/BookingSystem.csproj BookingSystem/
RUN dotnet restore BookingSystem/BookingSystem.csproj

COPY BookingSystem/ BookingSystem/
RUN dotnet publish BookingSystem/BookingSystem.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BookingSystem.dll"]
