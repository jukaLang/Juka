FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JukaApi/JukaApi.csproj", "JukaApi/"]
COPY ["JukaCompiler/JukaCompiler.csproj", "JukaCompiler/"]
RUN dotnet restore "JukaApi/JukaApi.csproj"
COPY . .
WORKDIR "/src/JukaApi"
RUN dotnet build "JukaApi.csproj" -c Release --self-contained true -a "" -o /app/build

FROM build AS publish
RUN dotnet publish "JukaApi.csproj" -c Release --self-contained true -a "" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./JukaApi"]