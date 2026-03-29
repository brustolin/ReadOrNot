# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY global.json ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY src/ReadOrNot.Web/ReadOrNot.Web.csproj ./src/ReadOrNot.Web/

RUN dotnet restore "./src/ReadOrNot.Web/ReadOrNot.Web.csproj"

COPY src/ReadOrNot.Web/ ./src/ReadOrNot.Web/
WORKDIR /src/src/ReadOrNot.Web

FROM build AS publish
RUN dotnet publish "ReadOrNot.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ReadOrNot.Web.dll"]
