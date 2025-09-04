#!/bin/sh

dotnet restore ../..
dotnet build --no-incremental --no-restore -c Release --self-contained -r win-x64
dotnet publish -o dist --no-build --no-restore -c Release -p:PublishReadyToRun=True --self-contained -r win-x64
rm dist/appsettings.*.json

# Check if we're on Linux or Windows and use appropriate compression command
if [ "$(uname)" = "Linux" ]; then
    zip -r output.zip dist/*
else
    powershell -Command "Compress-Archive -Path ./dist/* -DestinationPath output.zip -Force"
fi
