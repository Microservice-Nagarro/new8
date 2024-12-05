#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY [".", "./"]
RUN dotnet build "BHF.MS.new8.sln" -c "$BUILD_CONFIGURATION"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "code/BHF.MS.new8/BHF.MS.new8.csproj" --no-build -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BHF.MS.new8.dll"]
