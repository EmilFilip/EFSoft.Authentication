#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["EFSoft.Authentication.Api/EFSoft.Authentication.Api.csproj", "EFSoft.Authentication.Api/"]
RUN dotnet restore "EFSoft.Authentication.Api/EFSoft.Authentication.Api.csproj"
COPY . .
WORKDIR "/src/EFSoft.Authentication.Api"
RUN dotnet build "EFSoft.Authentication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EFSoft.Authentication.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EFSoft.Authentication.Api.dll"]