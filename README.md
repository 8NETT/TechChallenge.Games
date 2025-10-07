# Tech Challenge - Catálogo de Jogos

Este repositório contém o código-fonte do serviço de catálogo de jogos, desenvolvido como parte do Tech Challenge da pós-graduação. O serviço utiliza uma arquitetura baseada em CQRS e Event Sourcing para gerenciar um catálogo de jogos de forma assíncrona, escalável e resiliente.

## Estrutura do Projeto

A solução é baseada em uma arquitetura robusta que separa as responsabilidades de escrita (Command) e leitura (Query), organizada da seguinte forma:

-   `Src/TechChallenge.Games.Web/`: Projeto principal da API (ASP.NET Core). Expõe os endpoints para interação com o catálogo.
    -   `Endpoints/`: Define os endpoints da API, separando as rotas de Jogos, Biblioteca e Busca.
    -   `Configurations/`: Contém a configuração de injeção de dependência e serviços da aplicação.
    -   `Program.cs`: Ponto de entrada da aplicação web.

-   `Src/TechChallenge.Games.Application/`: Camada de aplicação. Contém a lógica de negócio, DTOs, e as interfaces (contratos) dos serviços.

-   `Src/TechChallenge.Games.Command.Domain/`: Domínio de escrita (Command).
    -   `Aggregates/`: Contém o agregado `Jogo`, que encapsula o estado e o comportamento do domínio.
    -   `Events/`: Define os eventos de domínio que representam as mudanças de estado (`JogoCriadoEvent`, `PrecoAlteradoEvent`, etc.).
    -   `Persistence/`: Define as abstrações para o repositório de escrita e o `IEventStore`.

-   `Src/TechChallenge.Games.Command.Infrastructure/`: Infraestrutura de escrita.
    -   `Persistence/`: Implementação do `IEventStore` utilizando o **Azure Cosmos DB**.
    -   `Producers/`: Publica os eventos de domínio no **Azure Event Hubs**.

-   `Src/TechChallenge.Games.Query.Domain/`: Domínio de leitura (Query).
    -   `Documents/`: Define os modelos de leitura (`JogoDocument`) que são otimizados para consulta.
    -   `Persistence/`: Define as abstrações para o repositório de leitura.

-   `Src/TechChallenge.Games.Query.Infrastructure/`: Infraestrutura de leitura.
    -   `Consumers/`: Consome os eventos do **Azure Event Hubs** para atualizar os modelos de leitura.
    -   `Persistence/`: Implementação do repositório de leitura, provavelmente utilizando um serviço de busca como **Elasticsearch**.

-   `TechChallenge.Games.Application.Testes/` e `TechChallenge.Games.Command.Domain.Tests/`: Projetos de testes unitários, utilizando xUnit.

-   `.github/`: Contém os workflows de CI/CD do GitHub Actions.

-   `Dockerfile`: Permite a containerização da aplicação para deploy e portabilidade.

## Tecnologias Utilizadas

-   **.NET 9**: A versão mais recente da plataforma de desenvolvimento da Microsoft, utilizada para construir uma aplicação performática e com recursos modernos.
-   **ASP.NET Core**: Framework para a construção da API web.
-   **CQRS e Event Sourcing**: Padrões de arquitetura que separam as operações de leitura e escrita, proporcionando escalabilidade, resiliência e um histórico completo de alterações através dos eventos.
-   **Azure Cosmos DB**: Utilizado como o Event Store, persistindo a sequência de eventos de domínio de forma imutável.
-   **Azure Event Hubs**: Serviço de streaming de Big Data, utilizado como o barramento de eventos para a comunicação assíncrona entre os lados de escrita e leitura.
-   **Elasticsearch**: (Inferido a partir das dependências) Utilizado como o banco de dados de leitura para armazenar e consultar os modelos de jogos de forma otimizada.
-   **OpenTelemetry**: Padrão de observabilidade utilizado para instrumentar a aplicação, coletando traces e métricas para monitoramento.
-   **xUnit**: Framework de testes unitários para a plataforma .NET.
-   **Docker**: Plataforma de containerização para empacotar a aplicação e suas dependências.
-   **GitHub Actions**: Automação de CI/CD para build, teste e deploy da aplicação.

## Como Executar a Aplicação

### Pré-requisitos

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   [Docker](https://www.docker.com/products/docker-desktop) (Opcional)
-   Acesso a serviços do Azure (Cosmos DB, Event Hubs) ou emuladores locais.

### Usando o .NET CLI

1.  Clone o repositório:
    ```bash
    git clone <URL-DO-REPOSITORIO>
    cd TechChallenge.Games
    ```

2.  Configure o arquivo `appsettings.Development.json` na pasta `Src/TechChallenge.Games.Web/` com as connection strings e configurações necessárias:
    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*",
      "CosmosDb": {
        "ConnectionString": "sua-connection-string-do-cosmos-db",
        "DatabaseName": "GamesEventStore"
      },
      "EventHub": {
        "ConnectionString": "sua-connection-string-do-event-hub",
        "HubName": "games"
      },
      "Elasticsearch": {
        "Uri": "sua-uri-do-elasticsearch"
      }
    }
    ```

3.  Execute a aplicação a partir da pasta do projeto web:
    ```bash
    cd Src/TechChallenge.Games.Web
    dotnet run
    ```

### Usando Docker

1.  Na raiz do projeto, construa a imagem Docker:
    ```bash
    docker build -t techchallenge-games .
    ```

2.  Execute o container, passando as variáveis de ambiente necessárias:
    ```bash
    docker run -p 8080:8080 \
      -e CosmosDb__ConnectionString="sua-connection-string" \
      -e EventHub__ConnectionString="sua-connection-string" \
      -e Elasticsearch__Uri="sua-uri" \
      techchallenge-games
    ```
    A API estará disponível em `http://localhost:8080`.

## Como Executar os Testes

Para executar todos os testes da solução, navegue até a raiz do projeto e execute o seguinte comando:

```bash
dotnet test
```
