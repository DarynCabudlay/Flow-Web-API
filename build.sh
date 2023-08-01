#/bin/bash
wd=$(pwd)

# Restore packages
cd $wd/src/
dotnet restore

#Build
cd $wd/src/
dotnet build
