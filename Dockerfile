#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src

COPY ["NuGet.Config", "."]
COPY ["EFSoft.Authentication.Api/EFSoft.Authentication.Api.csproj", "EFSoft.Authentication.Api/"]

ARG NUGET_PASSWORD
RUN apk add --update sed 
RUN sed -i "s|</configuration>|<packageSourceCredentials><emilfilip3><add key=\"Username\" value=\"emilfilip3\" /><add key=\"ClearTextPassword\" value=\"${NUGET_PASSWORD}\" /></emilfilip3></packageSourceCredentials></configuration>|" NuGet.Config

RUN dotnet restore "EFSoft.Authentication.Api/EFSoft.Authentication.Api.csproj" --configfile NuGet.Config

COPY . .
WORKDIR "/src/EFSoft.Authentication.Api"
RUN dotnet build "EFSoft.Authentication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EFSoft.Authentication.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EFSoft.Authentication.Api.dll"]