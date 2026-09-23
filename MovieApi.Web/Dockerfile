FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Packages.props", "./"]
COPY ["MovieApi.Web/MovieApi.Web.csproj", "MovieApi.Web/"]
COPY ["MovieApi.Contracts/MovieApi.Contracts.csproj", "MovieApi.Contracts/"]
COPY ["MovieApi.Core/MovieApi.Core.csproj", "MovieApi.Core/"]
COPY ["MovieApi.Domain/MovieApi.Domain.csproj", "MovieApi.Domain/"]
COPY ["MovieApi.Infrastructure.Postgres/MovieApi.Infrastructure.Postgres.csproj", "MovieApi.Infrastructure.Postgres/"]

RUN dotnet restore "MovieApi.Web/MovieApi.Web.csproj"

COPY . .
WORKDIR "/src/MovieApi.Web"
RUN dotnet publish "MovieApi.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
USER app

ENTRYPOINT ["dotnet", "MovieApi.Web.dll"]