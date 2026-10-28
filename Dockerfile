FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/EducaReuse.Api/EducaReuse.Api.csproj src/EducaReuse.Api/
RUN dotnet restore src/EducaReuse.Api/EducaReuse.Api.csproj
COPY . .
RUN dotnet publish src/EducaReuse.Api/EducaReuse.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "EducaReuse.Api.dll"]
