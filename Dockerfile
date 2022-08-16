FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Release
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RoyalERP/RoyalERP.csproj", "RoyalERP/"]
COPY ["Contracts/RoaylERP.Contracts.csproj", "Contracts/"]
RUN dotnet restore "RoyalERP/RoyalERP.csproj"

COPY . .

RUN dotnet build "RoyalERP/RoyalERP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoyalERP/RoyalERP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoyalERP.dll"]