FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8081
ENV ASPNETCORE_URLS http://*:3013

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["gatherme-suggestion-ms/gatherme-suggestion-ms.csproj", "gatherme-suggestion-ms/"]
RUN dotnet restore "gatherme-suggestion-ms/gatherme-suggestion-ms.csproj"
COPY . .
WORKDIR "/src/gatherme-suggestion-ms"
RUN dotnet build "gatherme-suggestion-ms.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gatherme-suggestion-ms.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gatherme-suggestion-ms.dll"]
