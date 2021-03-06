#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG BUILD_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
ARG BUILD_CONFIGURATION
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src
COPY ["Triceratops.Dashboard/Triceratops.Dashboard.csproj", "Triceratops.Dashboard/"]
RUN dotnet restore "Triceratops.Dashboard/Triceratops.Dashboard.csproj"
COPY . .
WORKDIR "/src/Triceratops.Dashboard"
RUN libman restore
RUN dotnet build "Triceratops.Dashboard.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION
RUN dotnet publish "Triceratops.Dashboard.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triceratops.Dashboard.dll"]
