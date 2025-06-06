# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TrainingAgent/TrainingAgent.csproj", "TrainingAgent/"]
RUN dotnet restore "TrainingAgent/TrainingAgent.csproj"
COPY . .
WORKDIR "/src/TrainingAgent"
RUN dotnet build "TrainingAgent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrainingAgent.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrainingAgent.dll"]
