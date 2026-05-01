# Etapa base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /workspace

# Copia apenas os arquivos .csproj para restaurar dependências primeiro
COPY ["src/DevEval.WebApi/DevEval.WebApi.csproj", "src/DevEval.WebApi/"]
COPY ["src/DevEval.Application/DevEval.Application.csproj", "src/DevEval.Application/"]
COPY ["src/DevEval.Common/DevEval.Common.csproj", "src/DevEval.Common/"]
COPY ["src/DevEval.Domain/DevEval.Domain.csproj", "src/DevEval.Domain/"]
COPY ["src/DevEval.IoC/DevEval.IoC.csproj", "src/DevEval.IoC/"]
COPY ["src/DevEval.ORM/DevEval.ORM.csproj", "src/DevEval.ORM/"]

# Executa o restore de dependências
RUN dotnet restore "src/DevEval.WebApi/DevEval.WebApi.csproj"

# Copia todo o código-fonte agora que as dependências foram restauradas
COPY src/ src/

# Define o diretório correto para compilar
WORKDIR "/workspace/src/DevEval.WebApi"
RUN dotnet build "DevEval.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DevEval.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para rodar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DevEval.WebApi.dll"]
