FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/FlowCenter.API/FlowCenter.API.csproj", "src/FlowCenter.API/"]
COPY ["src/FlowCenter.Application/FlowCenter.Application.csproj", "src/FlowCenter.Application/"]
COPY ["src/FlowCenter.Domain/FlowCenter.Domain.csproj", "src/FlowCenter.Domain/"]
COPY ["src/FlowCenter.Infrastructure/FlowCenter.Infrastructure.csproj", "src/FlowCenter.Infrastructure/"]

RUN dotnet restore "src/FlowCenter.API/FlowCenter.API.csproj"

COPY . .
WORKDIR "/src/src/FlowCenter.API"
RUN dotnet publish "FlowCenter.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowCenter.API.dll"]