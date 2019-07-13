FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Juka/Juka.csproj", "Juka/"]
RUN dotnet restore "Juka/Juka.csproj"
COPY . .
WORKDIR "/src/Juka"
RUN dotnet build "Juka.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Juka.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Juka.dll"]