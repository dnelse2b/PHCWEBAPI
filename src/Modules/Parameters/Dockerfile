# Build da solução Parameters
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Parameters.API/Parameters.API.csproj", "Parameters.API/"]
COPY ["Parameters.Application/Parameters.Application.csproj", "Parameters.Application/"]
COPY ["Parameters.Domain/Parameters.Domain.csproj", "Parameters.Domain/"]
COPY ["Parameters.Infrastructure/Parameters.Infrastructure.csproj", "Parameters.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Parameters.API/Parameters.API.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/Parameters.API"
RUN dotnet build "Parameters.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Parameters.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parameters.API.dll"]
