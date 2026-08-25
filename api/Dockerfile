FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["api/Orbital.Api.csproj", "api/"]
RUN dotnet restore "api/Orbital.Api.csproj"

COPY api/ api/
WORKDIR /src/api
RUN dotnet build "Orbital.Api.csproj" --configuration Release --no-restore
RUN dotnet publish "Orbital.Api.csproj" --configuration Release --no-restore --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Orbital.Api.dll"]
