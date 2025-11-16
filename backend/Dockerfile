# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
COPY ["ContactApp.csproj", "./"]
RUN dotnet restore "ContactApp.csproj"

# Tüm dosyaları kopyala ve publish et
COPY . .
RUN dotnet publish "ContactApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContactApp.dll"]
