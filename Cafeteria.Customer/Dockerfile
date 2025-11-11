FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Cafeteria.Customer/Cafeteria.Customer.csproj", "Cafeteria.Customer/"]
RUN dotnet restore "Cafeteria.Customer/Cafeteria.Customer.csproj"
COPY . .
WORKDIR "/src/Cafeteria.Customer"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cafeteria.Customer.dll"]
