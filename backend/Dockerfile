FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
WORKDIR /src
COPY backend/*.csproj .
RUN dotnet restore
COPY backend/ .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "backend.dll"]
