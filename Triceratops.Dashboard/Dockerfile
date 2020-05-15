#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Triceratops.Dashboard/Triceratops.Dashboard.csproj", "Triceratops.Dashboard/"]
RUN dotnet restore "Triceratops.Dashboard/Triceratops.Dashboard.csproj"
COPY . .
WORKDIR "/src/Triceratops.Dashboard"
RUN dotnet build "Triceratops.Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Triceratops.Dashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triceratops.Dashboard.dll"]