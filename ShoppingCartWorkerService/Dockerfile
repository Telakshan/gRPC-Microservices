FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ShoppingCartWorkerService/ShoppingCartWorkerService.csproj", "ShoppingCartWorkerService/"]
RUN dotnet restore "ShoppingCartWorkerService/ShoppingCartWorkerService.csproj"
COPY . .
WORKDIR "/src/ShoppingCartWorkerService"
RUN dotnet build "ShoppingCartWorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoppingCartWorkerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingCartWorkerService.dll"]
