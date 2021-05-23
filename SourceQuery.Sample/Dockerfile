FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SourceQuery.Sample/SourceQuery.Sample.csproj", "SourceQuery.Sample/"]
RUN dotnet restore "SourceQuery.Sample/SourceQuery.Sample.csproj"
COPY . .
WORKDIR "/src/SourceQuery.Sample"
RUN dotnet build "SourceQuery.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SourceQuery.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SourceQuery.Sample.dll"]
