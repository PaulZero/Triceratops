FROM mcr.microsoft.com/dotnet/core/sdk:3.1.202-buster AS build
WORKDIR /src
COPY "Triceratops.Libraries" "Triceratops.Libraries/"
COPY "Triceratops.Blazor" "Triceratops.Blazor/"

RUN dotnet restore "Triceratops.Blazor/Triceratops.Blazor.csproj"
RUN dotnet build "Triceratops.Blazor/Triceratops.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Triceratops.Blazor/Triceratops.Blazor.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish .
COPY blazor.nginx.conf /etc/nginx/nginx.conf