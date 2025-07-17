
# # FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
# # WORKDIR /src
 
# # # Copy and build the app
# # COPY . ./
# # RUN dotnet restore
# # RUN dotnet publish -c Release -o /app/publish
 
# # # Use runtime image
# # FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
# # WORKDIR /app
# # COPY --from=build /app/publish .
 
# # # Expose the default port
# # EXPOSE 8080
 
# # ENTRYPOINT ["dotnet", "JobPortal.dll"]

# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
 
# WORKDIR /src
 
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="${PATH}:/root/.dotnet/tools"
 
# COPY *.csproj ./
# RUN dotnet restore
 
# COPY . ./
# RUN dotnet publish -c Release -o /app/publish
 
 
# FROM mcr.microsoft.com/dotnet/sdk:9.0
 
# WORKDIR /app
 
# RUN apt-get update && apt-get install -y postgresql-client
 
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="${PATH}:/root/.dotnet/tools"
 
# COPY --from=build /app/publish .
 
# COPY . .
 
# COPY entrypoint.sh .
# COPY https-dev-cert.pfx .
# RUN chmod +x ./entrypoint.sh
 
# RUN mkdir -p /app/UploadedThumbnails /app/Uploads
 
# ENV ASPNETCORE_ENVIRONMENT=Development
 
# EXPOSE 5001
 
# ENTRYPOINT ["./entrypoint.sh"]
 
# ---------- base image for runtime ----------
# FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# WORKDIR /app

# # generate a self-signed certificate
# RUN apt-get update && apt-get install -y openssl && \
#     mkdir /https && \
#     openssl req -x509 -nodes -days 365 \
#       -newkey rsa:2048 \
#       -keyout /https/aspnetcore.key \
#       -out /https/aspnetcore.crt \
#       -subj "/CN=localhost" && \
#     openssl pkcs12 -export \
#       -out /https/aspnetcore.pfx \
#       -inkey /https/aspnetcore.key \
#       -in /https/aspnetcore.crt \
#       -passout pass:password

# # expose http and https
# EXPOSE 8080
# EXPOSE 8081

# # ---------- build image ----------
# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# WORKDIR /src

# # install dotnet ef tool
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="${PATH}:/root/.dotnet/tools"

# # copy csproj and restore
# COPY *.csproj ./
# RUN dotnet restore

# # copy everything else and publish
# COPY . ./
# RUN dotnet publish -c Release -o /app/publish

# # ---------- final image ----------
# FROM base AS final
# WORKDIR /app

# # copy published build
# COPY --from=build /app/publish .

# # copy generated certificate from base
# COPY --from=base /https/aspnetcore.pfx /https/aspnetcore.pfx

# # environment
# ENV ASPNETCORE_ENVIRONMENT=Development
# ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"

# # tell kestrel where to find the pfx
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetcore.pfx"
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"

# ENTRYPOINT ["./entrypoint.sh"]


# ----------- BASE IMAGE WITH SELF-SIGNED CERT --------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /app

# generate self-signed cert
RUN apt-get update && apt-get install -y openssl && \
    mkdir /https && \
    openssl req -x509 -nodes -days 365 \
      -newkey rsa:2048 \
      -keyout /https/aspnetcore.key \
      -out /https/aspnetcore.crt \
      -subj "/CN=localhost" && \
    openssl pkcs12 -export \
      -out /https/aspnetcore.pfx \
      -inkey /https/aspnetcore.key \
      -in /https/aspnetcore.crt \
      -passout pass:password

EXPOSE 8080
EXPOSE 8081

# ----------- BUILD IMAGE --------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# ----------- FINAL IMAGE --------------
FROM base AS final
WORKDIR /app

# install postgres-client so you can use pg_isready
RUN apt-get update && apt-get install -y postgresql-client
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# copy published app
COPY --from=build /app/publish .

# copy the certificate
COPY --from=base /https/aspnetcore.pfx /https/aspnetcore.pfx

# copy your entrypoint
COPY entrypoint.sh .
COPY JobPortal.csproj .
RUN chmod +x ./entrypoint.sh

# optional folders
RUN mkdir -p /app/UploadedThumbnails /app/Uploads

# environment
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/https/aspnetcore.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"

ENTRYPOINT ["./entrypoint.sh"]
