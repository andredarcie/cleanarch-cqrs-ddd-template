# DevEval.IntegrationTests

Suíte de testes integrados da solução `DevEval`.

## Objetivo

Este projeto valida o comportamento real da API, cobrindo a integração entre:

- pipeline HTTP do ASP.NET Core;
- autenticação JWT;
- handlers da aplicação;
- persistência com `EF Core`;
- banco PostgreSQL descartável.

Diferente dos testes unitários de `test/DevEval.Test`, aqui o foco é exercitar fluxos completos da aplicação.

## Como Funciona

Os testes:

- sobem um container PostgreSQL temporário via `docker`;
- injetam a connection string e as configurações JWT no host da API;
- iniciam a aplicação com `WebApplicationFactory<Program>`;
- resetam o banco entre cenários com `Respawn`;
- recriam o usuário base de autenticação antes dos testes que precisam de login.

Essa abordagem evita falsos positivos comuns de banco em memória e mantém a suíte próxima do comportamento real de produção.

## Responsabilidades da Infraestrutura

A infraestrutura foi separada por responsabilidade:

- `CustomWebApplicationFactory`: sobe o host HTTP da API para testes.
- `IntegrationTestFixture`: orquestra o ciclo de vida da suíte.
- `PostgresTestDatabase`: gerencia o PostgreSQL temporário via `docker`.
- `DatabaseResetter`: encapsula o reset do banco com `Respawn`.
- `TestUserSeeder`: garante o usuário base de autenticação.
- `TestEnvironmentVariables`: injeta e limpa as variáveis de ambiente usadas pela API.

## Estrutura

```text
DevEval.IntegrationTests/
  Fixtures/
    IntegrationTestCollection.cs
  Infrastructure/
    CustomWebApplicationFactory.cs
    DatabaseResetter.cs
    IntegrationTestFixture.cs
    PostgresTestDatabase.cs
    TestAuthentication.cs
    TestEnvironmentVariables.cs
    TestUserSeeder.cs
  Scenarios/
    Auth/
      LoginTests.cs
  DevEval.IntegrationTests.csproj
  README.md
```

## Requisitos

Para executar esta suíte localmente, o ambiente precisa ter:

- `.NET 10 SDK`;
- `Docker` disponível no `PATH`;
- engine do Docker em execução.

## Como Executar

Na raiz do repositório:

```console
dotnet test test/DevEval.IntegrationTests/DevEval.IntegrationTests.csproj
```

## Primeiro Cenário Implementado

Atualmente a suíte já cobre:

- login com credenciais válidas em `/api/auth/login`;
- geração real de token JWT;
- bootstrap da API com PostgreSQL temporário;
- reset e reseed do banco entre execuções;
- separação explícita entre host HTTP, banco temporário, reset e seed.

## Convenções do Projeto

- Novos testes devem ser organizados por recurso em `Scenarios/`.
- Infraestrutura compartilhada deve ficar em `Infrastructure/`.
- Fixtures de coleção devem ficar em `Fixtures/`.
- Cada teste deve validar status HTTP, payload relevante e, quando aplicável, efeito persistido no banco.

## Aderência às Boas Práticas

Este projeto foi estruturado para seguir os 10 pontos discutidos para testes de integração:

- `[x]` Testa fluxos reais entre API, autenticação e banco.
- `[x]` Usa PostgreSQL real em ambiente temporário.
- `[x]` Garante isolamento com reset automático do banco por teste.
- `[x]` Evita mocks internos no fluxo de integração.
- `[x]` Usa dados de teste controlados e previsíveis.
- `[x]` Mantém testes determinísticos.
- `[x]` Executa em ambiente próximo de produção, com a mesma engine de banco.
- `[x]` Valida contratos HTTP, incluindo status code, payload e `Location` quando aplicável.
- `[x]` Mantém a suíte rápida o suficiente para uso em CI.
- `[x]` Faz limpeza automática de estado e destruição do banco temporário ao final.

Observação:

- A base já segue esses pontos na infraestrutura atual.
- A profundidade da cobertura ainda cresce conforme novos cenários forem adicionados.

## Próximos Passos Sugeridos

- adicionar cenários de `Products`;
- adicionar cenários de `Carts`;
- validar fluxo `checkout`;
- cobrir cenários de erro como `401`, `404` e `400`.
