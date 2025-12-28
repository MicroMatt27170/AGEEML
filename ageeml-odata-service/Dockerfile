FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ageeml-odata-service/service.csproj service/
RUN dotnet restore service/service.csproj
COPY ageeml-odata-service/ service/
WORKDIR /src/service
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
RUN mkdir -p /app/sqlite
COPY --from=build /app/publish .
COPY sqlite/ageeml.db /app/sqlite/ageeml.db
ENTRYPOINT ["dotnet", "service.dll"]
