#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG BUILD_CONFIGURATION
WORKDIR /src
COPY ["Triceratops.Api/Triceratops.Api.csproj", "Triceratops.Api/"]
RUN dotnet restore "Triceratops.Api/Triceratops.Api.csproj"
COPY . .
WORKDIR "/src/Triceratops.Api"
RUN dotnet build "Triceratops.Api.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "Triceratops.Api.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triceratops.Api.dll"]