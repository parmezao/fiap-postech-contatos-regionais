# Projeto Tech Challenge - .NET 8 + SQLServer com Docker 

Este é um projeto .NET que utiliza Docker para facilitar o ambiente de desenvolvimento e execução. Utiliza Docker Compose para orquestrar a aplicação juntamente com um banco de dados SQLServer.

## Pré-requisitos

- Docker Engine: [Instalação do Docker](https://docs.docker.com/get-docker/)
- Docker Compose: [Instalação do Docker Compose](https://docs.docker.com/compose/install/)

## Como executar

1. Clone este repositório:

 ```bash
   git clone https://github.com/parmezao/fiap-postech-contatos-regionais.git
   cd fiap-postech-contatos-regionais
  ```

2. Execute o seguinte comando para iniciar o projeto junto com o SQLServer:

```bash
  docker-compose up -d
```
Isso iniciará os contêineres Docker em segundo plano (-d para detached mode), incluindo a aplicação .NET e o banco de dados SQLServer.

Acesse a aplicação em http://localhost:8080

Usuário e senha para a geração de Token. Usuário:admin, senha: admin@123

3. Parando a execução do projeto e removendo os containers

```bash
  docker-compose down
```
