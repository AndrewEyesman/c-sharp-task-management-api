# 1. Use the .NET 10 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Copy the whole root so Docker sees the folder structure
COPY . . 
WORKDIR "/src/TaskApi"
RUN dotnet publish "TaskApi.csproj" -c Release -o /app/publish

# 2. Use the smaller "Runtime" image to actually run the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskApi.dll"]