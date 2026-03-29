FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DevInsight.Backend.sln ./
COPY src/DevInsight.Api/DevInsight.Api.csproj src/DevInsight.Api/
COPY src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Domain/DevInsight.EngineeringIntelligence.Domain.csproj src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Domain/
COPY src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Application/DevInsight.EngineeringIntelligence.Application.csproj src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Application/
COPY src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Infrastructure/DevInsight.EngineeringIntelligence.Infrastructure.csproj src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Infrastructure/
COPY src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Interfaces/DevInsight.EngineeringIntelligence.Interfaces.csproj src/Contexts/EngineeringIntelligence/DevInsight.EngineeringIntelligence.Interfaces/

RUN dotnet restore DevInsight.Backend.sln

COPY . .
RUN dotnet publish src/DevInsight.Api/DevInsight.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
	&& apt-get install -y --no-install-recommends git ca-certificates \
	&& rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "DevInsight.Api.dll"]
