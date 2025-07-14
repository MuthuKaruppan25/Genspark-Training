#!/bin/bash

# echo "Waiting for PostgreSQL..."
# until pg_isready -h postgres -p 5432 -U muthu; do
#   echo "Postgres is unavailable - sleeping"
#   sleep 2
# done
 
# echo "Postgres is ready. Running EF Core migrations..."
 
 
# cd /app
# dotnet build
# dotnet ef database update
 
# echo "Starting application..."
# exec dotnet JobPortal.dll

# !/bin/bash

echo "Waiting for PostgreSQL..."
until pg_isready -h postgres -p 5432 -U muthu; do
  echo "Postgres is unavailable - sleeping"
  sleep 2
done

echo "Postgres is ready. Running EF Core migrations..."

cd /app
# Run from source to avoid build issues
dotnet build
dotnet ef migrations add New\
  --project JobPortal.csproj
dotnet ef database update \
  --project JobPortal.csproj 

echo "Starting application..."
exec dotnet JobPortal.dll
