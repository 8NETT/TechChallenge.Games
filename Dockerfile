FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["TechChallenge.Games.sln", "./"]
COPY ["Src/TechChallenge.Games.Web/TechChallenge.Games.Web.csproj", "Src/TechChallenge.Games.Web/"]
COPY ["Src/TechChallenge.Games.Core/TechChallenge.Games.Core.csproj", "Src/TechChallenge.Games.Core/"]
COPY ["Src/TechChallenge.Games.Infrastructure/TechChallenge.Games.Infrastructure.csproj", "Src/TechChallenge.Games.Infrastructure/"]
COPY ["Src/TechChallenge.Games.Application/TechChallenge.Games.Application.csproj", "Src/TechChallenge.Games.Application/"]
COPY ["TechChallenge.Games.Testes/TechChallenge.Games.Testes.csproj", "TechChallenge.Games.Testes/"]

RUN dotnet restore "TechChallenge.Games.sln"

# Copy everything else and build
COPY . ./

# Run tests
RUN dotnet test "TechChallenge.Games.Testes/TechChallenge.Games.Testes.csproj" --no-restore --verbosity normal

# Build the application
RUN dotnet publish "Src/TechChallenge.Games.Web/TechChallenge.Games.Web.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so

WORKDIR /app
COPY --from=build-env /app/publish .

# Force ASP.NET Core to listen on port 8081
ENV ASPNETCORE_URLS=http://+:8081

# Expose port 8081 so Azure Container Apps ingress can connect
EXPOSE 8081

ENTRYPOINT ["dotnet", "TechChallenge.Games.dll"]