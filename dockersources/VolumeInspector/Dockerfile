FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Triceratops.VolumeInspector.csproj"
RUN dotnet build "Triceratops.VolumeInspector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Triceratops.VolumeInspector.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Triceratops.VolumeInspector.dll"]