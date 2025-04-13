FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrokenWebAPI/BrokenWebAPI.csproj", "BrokenWebAPI/"]
RUN dotnet restore "BrokenWebAPI/BrokenWebAPI.csproj"
COPY . .
WORKDIR "/src/BrokenWebAPI"
RUN dotnet build "BrokenWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrokenWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrokenWebAPI.dll", "--environment=Production"]