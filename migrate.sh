#!/bin/bash

DATABASE_PROVIDER=${DATABASE_PROVIDER:-"mysql"}

export ConnectionStrings__DefaultConnection="Server=${DATABASE_HOST};Port=3306;Database=${DATABASE_NAME};User=root;Password=${DATABASE_PASSWORD};"

echo "Using connection string: $ConnectionStrings__DefaultConnection"

dotnet tool install --global dotnet-ef --version 9.0.0

if [ "$DATABASE_PROVIDER" = "mysql" ]; then
    cd src/ReadOrNot.Migrations.MySql
elif [ "$DATABASE_PROVIDER" = "sqlserver" ]; then
    cd src/ReadOrNot.Migrations.SqlServer
fi

~/.dotnet/tools/dotnet-ef database update -v -- connection="$ConnectionStrings__DefaultConnection"