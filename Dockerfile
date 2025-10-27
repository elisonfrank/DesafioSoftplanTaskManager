# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./TaskManager.sln
RUN dotnet publish ./src/Softplan.TaskManager.Api/Softplan.TaskManager.Api.csproj -c Release -o /app
RUN dotnet build ./tests/Softplan.TaskManager.IntegrationTests/Softplan.TaskManager.IntegrationTests.csproj -c Release

# Etapa de testes (só usada se você buildar com --target testrunner)
FROM build AS testrunner
WORKDIR /src
CMD ["dotnet", "test", "TaskManager.sln", "--logger:trx"]

# Etapa de runtime da API (imagem final)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=build /app .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Softplan.TaskManager.Api.dll"]