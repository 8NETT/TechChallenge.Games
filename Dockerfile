FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy solution file
COPY ["TechChallenge.Games.sln", "./"]

# Copy project files
COPY ["Src/TechChallenge.Games.Web/TechChallenge.Games.Web.csproj", "Src/TechChallenge.Games.Web/"]
COPY ["Src/TechChallenge.Games.Application/TechChallenge.Games.Application.csproj", "Src/TechChallenge.Games.Application/"]
COPY ["Src/TechChallenge.Games.Command.Domain/TechChallenge.Games.Command.Domain.csproj", "Src/TechChallenge.Games.Command.Domain/"]
COPY ["Src/TechChallenge.Games.Command.Infrastructure/TechChallenge.Games.Command.Infrastructure.csproj", "Src/TechChallenge.Games.Command.Infrastructure/"]
COPY ["Src/TechChallenge.Games.Query.Domain/TechChallenge.Games.Query.Domain.csproj", "Src/TechChallenge.Games.Query.Domain/"]
COPY ["Src/TechChallenge.Games.Query.Infrastructure/TechChallenge.Games.Query.Infrastructure.csproj", "Src/TechChallenge.Games.Query.Infrastructure/"]

# Copy test project files
COPY ["TechChallenge.Games.Command.Domain.Tests/TechChallenge.Games.Command.Domain.Tests.csproj", "TechChallenge.Games.Command.Domain.Tests/"]
COPY ["TechChallenge.Games.Application.Testes/TechChallenge.Games.Application.Testes.csproj", "TechChallenge.Games.Application.Testes/"]

# Restore packages
RUN dotnet restore "TechChallenge.Games.sln"

# Copy everything
COPY . ./

# Run tests
# RUN dotnet test "TechChallenge.Games.sln" --no-restore --verbosity normal

# Publish web project
RUN dotnet publish "Src/TechChallenge.Games.Web/TechChallenge.Games.Web.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install New Relic agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
 && echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
 && wget https://download.newrelic.com/548C16BF.gpg \
 && apt-key add 548C16BF.gpg \
 && apt-get update \
 && apt-get install -y 'newrelic-dotnet-agent' \
 && rm -rf /var/lib/apt/lists/*

# Enable New Relic agent
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so

# Copy published app
COPY --from=build-env /app/publish .

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8082
EXPOSE 8082

# Run the Web project DLL
ENTRYPOINT ["dotnet", "TechChallenge.Games.Web.dll"]
 