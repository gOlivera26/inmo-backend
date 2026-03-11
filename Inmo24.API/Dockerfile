FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Inmo24.API/Inmo24.API.csproj", "Inmo24.API/"]
COPY ["Inmo24.Application/Inmo24.Application.csproj", "Inmo24.Application/"]
COPY ["Inmo24.Domain/Inmo24.Domain.csproj", "Inmo24.Domain/"]

RUN dotnet restore "Inmo24.API/Inmo24.API.csproj"

COPY . .

WORKDIR "/src/Inmo24.API"
RUN dotnet build "Inmo24.API.csproj" -c Release -o /app/build
RUN dotnet publish "Inmo24.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN groupadd -r appgroup && useradd -r -g appgroup appuser

COPY --from=build /app/publish .
RUN chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Inmo24.API.dll"]