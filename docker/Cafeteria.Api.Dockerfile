FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Cafeteria.Api/Cafeteria.Api.csproj", "Cafeteria.Api/"]
RUN dotnet restore "Cafeteria.Api/Cafeteria.Api.csproj"
COPY . .
WORKDIR "/src/Cafeteria.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cafeteria.Api.dll"]
