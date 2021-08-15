FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

#Copy
COPY . ./

#Restore
RUN dotnet restore

#Test
RUN dotnet test

#Publish
WORKDIR /app/Pokedex.Api
RUN dotnet publish -c Release -o out

#Published Clean Image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/Pokedex.Api/out ./

ENTRYPOINT ["dotnet", "Pokedex.Api.dll"]