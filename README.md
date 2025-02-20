
# JWT Authentication with ASP.NET Core

  

## 📜 Descrição

  

Este projeto implementa autenticação JWT em uma aplicação ASP.NET Core, com interface de usuário simples para login, logout e registro de usuários. A autenticação é realizada usando tokens JWT, e as senhas são protegidas com BCrypt.

A aplicação também é empacotada com **Docker**, usando **Docker Compose** para configurar os containers do PostgreSQL, Redis e a própria aplicação.

  

## 🛠️ Tecnologias Utilizadas

  

- **ASP.NET Core**

- **JWT**
- **BCrypt**
- **Entity Framework Core**
- **PostgreSQL**
- **Redis** (para armazenamento de sessões, por exemplo)
- **Docker** e **Docker Compose**
- **HTML** e **CSS**
  

## 🚀 Funcionalidades

- **Autenticação de usuários com JWT:** Permite login, registro e logout de usuários utilizando tokens JWT para autenticação.

- **Geração de tokens de acesso e refresh:** Gera tokens de acesso para login e tokens de refresh para reautenticação segura.

- **Armazenamento de tokens:**
  - **AccessToken:** O token de acesso é armazenado de forma segura em cookies no navegador do usuário.
  - **RefreshToken:** O token de refresh é armazenado no Redis, permitindo revalidação de sessão de maneira eficiente e segura.

- **Proteção de páginas:** A página Home (Index) e outras rotas são protegidas, sendo acessíveis apenas por usuários autenticados.

- **Hashing de senhas com BCrypt:** As senhas dos usuários são armazenadas de forma segura utilizando o algoritmo BCrypt.

- **Armazenamento de dados com Entity Framework Core:** Utiliza o Entity Framework Core para gerenciar dados do usuário e sessões, com suporte a PostgreSQL.

## 🔑 Instalação e Execução


### Como Executar com Docker

1.  **Clone o repositório**:

```bash
git clone https://github.com/phenriq-dev/JWTAuth.git
```

2. **Navegue até o diretório do projeto**:
```bash
cd JWTAuth
```
3. **Instale as dependências**:
```bash
dotnet restore
```
4. **Configure o appsettings.json** com suas credenciais de banco de dados e configurações de token.

5. A aplicação estará disponível em: `http://localhost:8080`.

## 🌟 O que eu aprendi

-   **Autenticação com JWT:** Aprendi a gerar e validar tokens JWT para autenticar usuários e proteger rotas da aplicação.
- **Injeção de Dependência (DI):** Utilizei DI para facilitar o gerenciamento e testes dos serviços na aplicação.
- **Boas práticas de segurança:** Apliquei técnicas de segurança como hashing de senhas (BCrypt) e armazenamento seguro de tokens (cookies e Redis).


## 🚀 Melhorias Futuras

- **Suporte a refresh tokens:** Melhorar a experiência de usuário com suporte à renovação de tokens, permitindo a manutenção da sessão sem necessidade de reautenticação.
- **Testes automatizados:** Adicionar testes unitários e de integração para garantir a estabilidade e qualidade do código.

## 🤝 Contribuições

Sinta-se à vontade para contribuir com este projeto! Sugestões de melhorias, criação de issues e pull requests são bem-vindas.

## 📧 Contato

Caso tenha interesse no projeto ou queira discutir mais sobre, entre em contato:

-   Email: hnriq.donha@gmail.com
-   LinkedIn: https://www.linkedin.com/in/pedro-donha/
