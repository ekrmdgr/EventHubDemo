FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /EventHubDemo.Listener

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /EventHubDemo.Listener
COPY --from=build-env /EventHubDemo.Listener/out .
ENTRYPOINT ["dotnet", "EventHubDemo.Listener.dll"]