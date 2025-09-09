FROM mcr.microsoft.com/dotnet/sdk:9.0 AS test
WORKDIR /src
COPY . .
RUN dotnet restore Cafeteria.sln
RUN dotnet build Cafeteria.sln -c Release --no-restore
RUN dotnet test Cafeteria.sln -c Release --no-build --verbosity normal
