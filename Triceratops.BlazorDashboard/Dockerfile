#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Triceratops.BlazorDashboard/Triceratops.BlazorDashboard.csproj", "Triceratops.BlazorDashboard/"]
RUN dotnet restore "Triceratops.BlazorDashboard/Triceratops.BlazorDashboard.csproj"
COPY . .
WORKDIR "/src/Triceratops.BlazorDashboard"
RUN dotnet build "Triceratops.BlazorDashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Triceratops.BlazorDashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triceratops.BlazorDashboard.dll"]