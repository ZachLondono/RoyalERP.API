FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Release
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RoyalERP.API/RoyalERP.API.csproj", "RoyalERP.API/"]
COPY ["RoyalERP.API.Contracts/RoyalERP.API.Contracts.csproj", "RoyalERP.API.Contracts/"]
RUN dotnet restore "RoyalERP.API/RoyalERP.API.csproj"

COPY . .

RUN dotnet build "RoyalERP.API/RoyalERP.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoyalERP.API/RoyalERP.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoyalERP.API.dll"]