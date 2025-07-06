# Use .NET 9 SDK to build
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview
WORKDIR /app
COPY --from=build /app/out ./

# Expose port (change if needed)
EXPOSE 8080

ENTRYPOINT ["dotnet", "paymentBackend.dll"]
