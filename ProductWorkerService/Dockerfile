FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ProductWorkerService/ProductWorkerService.csproj", "ProductWorkerService/"]
RUN dotnet restore "ProductWorkerService/ProductWorkerService.csproj"
COPY . .
WORKDIR "/src/ProductWorkerService"
RUN dotnet build "ProductWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductWorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductWorkerService.dll"]
