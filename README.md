# Softplan Task Manager API

API ASP.NET Core para gerenciamento de tarefas, com testes de integração.  
Banco de dados: **SQLite InMemory** (não precisa de container de banco).

---

## 🚀 Rodando a API com Docker

### Build da imagem
```bash
docker build -t softplan-api .
```

### Subir a API
```bash
docker run -p 5000:5000 softplan-api
```
A API estará disponível em: 👉 http://localhost:5000

### 🧪 Rodando os testes de integração
Você pode rodar os testes diretamente dentro do container:
```bash
docker run --rm softplan-api dotnet test ./tests/Softplan.TaskManager.IntegrationTests/Softplan.TaskManager.IntegrationTests.csproj
```

### 🐳 Usando Docker Compose
Se preferir, use o docker-compose.yml:

#### Subir a Api:
```bash
docker-compose up api
``` 

#### Rodar os testes integrados:
```bash
docker-compose up integration-tests
``` 

#### Rodar os testes unitários:
```bash
docker-compose up unit-tests
``` 

### ✅ Resumo
- **API**: ```docker run -p 5000:5000 softplan-api```
- **Testes**: ```docker run --rm softplan-api dotnet test ./tests/Softplan.TaskManager.IntegrationTests/Softplan.TaskManager.IntegrationTests.csproj```
- **Compose API**: ```docker-compose up api```
- **Compose Integration Tests**: ```docker-compose up integration-tests```
- **Compose Unit Tests**: ```docker-compose up unit-tests```

Como o banco é SQLite InMemory, não há dependências externas.

## 👤 Autor

- **Nome:** Élison Frankowski
- **GitHub:** [@elisonfrank](https://github.com/elisonfrank)
- **LinkedIn:** [elison-frankowski](https://www.linkedin.com/in/elison-frankowski-5b0543117)
- **Contato:** (42)99149-6711