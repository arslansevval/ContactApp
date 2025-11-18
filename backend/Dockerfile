# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
COPY ["ContactApp.csproj", "./"]
RUN dotnet restore "ContactApp.csproj"

# Tüm dosyaları kopyala ve publish et
COPY . .
RUN dotnet build "ContactApp.csproj" -c Release -o /app/build
RUN dotnet publish "ContactApp.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview 
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
COPY --from=build /app/publish .
