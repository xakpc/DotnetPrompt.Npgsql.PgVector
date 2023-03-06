#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/DotnetPrompt.Npgsql.PgVector.Experiment/DotnetPrompt.Npgsql.PgVector.Experiment.csproj", "src/DotnetPrompt.Npgsql.PgVector.Experiment/"]
RUN dotnet restore "src/DotnetPrompt.Npgsql.PgVector.Experiment/DotnetPrompt.Npgsql.PgVector.Experiment.csproj"
COPY . .
WORKDIR "/src/src/DotnetPrompt.Npgsql.PgVector.Experiment"
RUN dotnet build "DotnetPrompt.Npgsql.PgVector.Experiment.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotnetPrompt.Npgsql.PgVector.Experiment.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetPrompt.Npgsql.PgVector.Experiment.dll"]