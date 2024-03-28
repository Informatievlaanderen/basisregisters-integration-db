#!/bin/sh

dotnet paket restore
dotnet restore ../..
dotnet build --no-incremental --no-restore -c Release --self-contained
dotnet publish -o dist --no-build --no-restore -c Release -p:PublishReadyToRun=True --self-contained
rm dist/appsettings.*.json 
powershell -Command "Compress-Archive -Path ./dist/* -DestinationPath output.zip"
