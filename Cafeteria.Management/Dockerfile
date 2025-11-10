FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Cafeteria.Management/Cafeteria.Management.csproj", "Cafeteria.Management/"]
RUN dotnet restore "Cafeteria.Management/Cafeteria.Management.csproj"
COPY . .
WORKDIR "/src/Cafeteria.Management"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cafeteria.Management.dll"]
