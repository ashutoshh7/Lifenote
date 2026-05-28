FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Lifenote.API/Lifenote.API.csproj", "Lifenote.API/"]
COPY ["Lifenote.Application/Lifenote.Application.csproj", "Lifenote.Application/"]
COPY ["Lifenote.Domain/Lifenote.Domain.csproj", "Lifenote.Domain/"]
COPY ["Lifenote.Infrastructure/Lifenote.Infrastructure.csproj", "Lifenote.Infrastructure/"]
RUN dotnet restore "./Lifenote.API/Lifenote.API.csproj"
COPY . .
WORKDIR "/src/Lifenote.API"
RUN dotnet build "./Lifenote.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lifenote.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lifenote.API.dll"]
