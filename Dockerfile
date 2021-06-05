FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Play.Catalog.Service.csproj", ""]
RUN dotnet restore "Play.Catalog.Service.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Play.Catalog.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Catalog.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Play.Catalog.Service.dll"]
