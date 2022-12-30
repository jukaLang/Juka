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
WORKDIR /src/JukaApi

FROM build AS publish
RUN  curl -sS "https://webinstall.dev/jq" | bash && export PATH="/root/.local/bin:$PATH" && JUKTEMP=$(curl "https://api.github.com/repos/JukaLang/juka/releases/latest" | jq -r ".tag_name") && dotnet publish "JukaApi.csproj" /p:Version=$JUKTEMP -c Release --self-contained true -a "" -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./JukaApi"]