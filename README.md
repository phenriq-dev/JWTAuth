
# JWT Authentication Example - ASP.NET Core

  

## 📜 Descrição

  

Este é um exemplo de implementação de autenticação baseada em JWT (JSON Web Token) usando **ASP.NET Core**. O projeto demonstra como proteger endpoints com tokens JWT, uma prática comum em projetos de APIs RESTful. Ele foi desenvolvido com o propósito de praticar e demonstrar habilidades com autenticação segura e controle de acesso em aplicações **backend**.

  

## 🛠️ Tecnologias Utilizadas

  

-  **C#**

-  **ASP.NET Core**

-  **JWT (JSON Web Token)**

-  **Dependency Injection (DI)**

-  **Token Validation**

-  **REST API**

  

## 🚀 Funcionalidades

  

- Autenticação de usuários por meio de login utilizando JWT.

- Geração de tokens de acesso e refresh.

- Controle de acesso para endpoints protegidos por JWT.

- Implementação modular usando **Injeção de Dependência (DI)** para serviços JWT.

## 🔑 Instalação e Execução

  

### Pré-requisitos

  

- .NET 7.0 SDK ou superior

- IDE (como Visual Studio ou VS Code)

  

### Passos para executar o projeto localmente

  

1.  **Clone o repositório**:

```bash
git clone https://github.com/seu-usuario/jwt-auth-example.git
```

2. **Navegue até o diretório do projeto**:
```bash
cd JWTAuth
```
3. **Instale as dependências**:
```bash
dotnet restore
```
4. **Configure o appsettings.json**: No arquivo `appsettings.json`, adicione suas configurações de `TokenConfigurations`:
```bash
{
  "TokenConfigurations": {
    "Audience": "SeuPublico",
    "Issuer": "SeuEmissor",
    "Seconds": 3600
  }
}
```
5. **Execute a aplicação**:
```bash
dotnet run
```
6. **Acesse a aplicação**: A API estará rodando em https://localhost:5001.

## 📂 Estrutura do Projeto

```bash
├── Controllers
│   └── UserController.cs      # Controlador de autenticação e geração de token JWT
├── Models
│   └── User.cs                # Modelo de dados do usuário
├── Repositories
│   └── UserRepository.cs      # Repositório para simular busca de usuários (pode ser modificado para usar banco de dados real)
├── Services
│   ├── Jwt
│   │   ├── Interfaces
│   │   │   ├── ITokenService.cs  # Interface para o serviço de token
│   │   ├── Manager
│   │   │   └── TokenService.cs   # Implementação da lógica de geração de tokens JWT
│   │   ├── Models
│   │   │   ├── TokenConfigurations.cs # Configurações de token JWT
│   │   └── SigningConfigurations.cs  # Configurações de assinatura e validação de tokens
├── Program.cs                 # Configuração da aplicação e registro de serviços
└── appsettings.json           # Configurações de Token
```

## 🔐 Endpoints da API
### `POST /account/login`

Realiza a autenticação do usuário e gera um token JWT.

- **Request**: 
```json 
{ 
  "username": "batman", 
  "password": "batman"
}
 ```
 - **Response**: 
```json 
{
  "user": {
    "id": 1,
    "username": "batman",
    "password": ""
  },
  "token": {
    "authenticated": true,
    "created": "2024-xx-xx xx:00:00",
    "expiration": "2024-xx-xx xx:00:00",
    "accessToken": "eyJhbGc...restodoToken",
    "refreshToken": "uniqueRefreshTokenValue"
  }
}
 ```

## 🌟 O que eu aprendi

-   **Implementação de JWT**: Aprendi como gerar e validar tokens JWT para autenticar usuários e proteger endpoints de APIs.

-   **Injeção de Dependência**: Utilizei DI para garantir que os serviços sejam facilmente testáveis e gerenciáveis.

-   **Segurança**: Apliquei boas práticas para assegurar que informações sensíveis, como senhas, não sejam expostas.


## 🚀 Melhorias Futuras

- Implementar **Redis** para gerenciamento de sessões de usuários, proporcionando armazenamento em memória, escalabilidade e expiração automática de sessões.

- Adicionar suporte a **refresh tokens** para melhorar a experiência do usuário e a segurança.

- Criar uma interface de usuário para facilitar a interação com a API.

- Implementar testes automatizados para garantir a estabilidade do código.

-  Implementar um **banco de dados real** para gerenciamento de usuários.

-  Implementar um sistema de **hashing de senhas** (como o **BCrypt**) para garantir que as senhas armazenadas sejam seguras.

- **Dockerizar** a aplicação para facilitar a implantação e garantir a consistência entre ambientes de desenvolvimento e produção.

## 🤝 Contribuições

Sinta-se à vontade para contribuir com este projeto! Sugestões de melhorias, criação de issues e pull requests são bem-vindas.

## 📧 Contato

Caso tenha interesse no projeto ou queira discutir mais sobre, entre em contato:

-   Email: hnriq.donha@gmail.com
-   LinkedIn: https://www.linkedin.com/in/pedro-donha/
