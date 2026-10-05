# Arvum API - Sistema de Gestão Financeira Pessoal

Backend em **.NET 9** com arquitetura em camadas voltado ao controle e planejamento financeiro pessoal, integrando cadastros gerais (orçamentos, metas, investimentos, dívidas, rendas), extrato com conciliação cronológica de saldos e inteligência de indicadores financeiros.

---

## 1. Arquitetura da Solução

A aplicação é dividida em 4 camadas principais localizadas na pasta `erp_pessoal`:

- **Presentation (`Presentation.csproj`):** Controladores HTTP (REST), modelos de entrada/saída web (`WebModels`), mappers de entrada e inicialização do ASP.NET Core (`Program.cs`) com JWT e Swagger.
- **Application (`Application.csproj`):** Orquestração de regras de aplicação, serviços (`Services`), interfaces contratuais de repositório e DTOs desacoplados.
- **Domain (`Domain.csproj`):** Regras de negócio puras, entidades ricas de domínio (`Entities`) e helpers (ex: normalização de sinais e autenticação).
- **Infrastructure (`Infrastructure.csproj`):** Camada de dados com conexão PostgreSQL via `NpgsqlConnection` e mapeamento SQL de alta performance via **Dapper**.

> **Nota:** A pasta `erp_old` é legada e está descontinuada. Não deve ser utilizada para compilação ou execução.

---

## 2. Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.
- Banco de dados PostgreSQL (v14+) acessível.
- CLI do .NET configurada no PATH do sistema.

---

## 3. Guia de Build para Testes e Desenvolvimento

Para compilar, validar tipos e executar testes ou análise de integridade da solução:

### 3.1. Restauração de Pacotes
```bash
dotnet restore erp_pessoal/erp_pessoal.sln
```

### 3.2. Build de Verificação e Desenvolvimento (Debug)
Compila todos os projetos da solução em modo `Debug`, gerando os binários de desenvolvimento e validando sintaxe e referências:
```bash
dotnet build erp_pessoal/erp_pessoal.sln -c Debug
```
*Parâmetros úteis:*
- `--no-restore`: Pula a etapa de restauração se já tiver sido executada previamente.
- `-v minimal`: Reduz a verbosidade dos logs no terminal.

### 3.3. Execução de Testes
Caso projetos de teste unitário/integração sejam adicionados:
```bash
dotnet test erp_pessoal/erp_pessoal.sln -c Debug --logger "console;verbosity=normal"
```

### 3.4. Execução Local para Desenvolvimento
Para rodar a API localmente com hot-reload e Swagger:
```bash
dotnet run --project erp_pessoal/Presentation/Presentation.csproj
```
Por padrão, a aplicação responderá em:
- `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

---

## 4. Guia de Build para Produção (Preparar Aplicação para Rodar com Runtime)

Para preparar a aplicação para rodar em servidores (ex.: Ubuntu VPS, Docker, Render, Oracle Cloud):

### 4.1. Publicação Dependente de Framework (FDD - Framework-Dependent Deployment)
Recomendado quando o host/servidor já possui o .NET 9 Runtime instalado. Gera um pacote leve:

```bash
dotnet publish erp_pessoal/Presentation/Presentation.csproj -c Release -o ./publish/fdd
```

**Para executar o pacote gerado no servidor:**
```bash
cd ./publish/fdd
dotnet Presentation.dll
```

### 4.2. Publicação Auto-Contida (Self-Contained Deployment)
Empacota o runtime do .NET 9 junto com a aplicação, não necessitando que o servidor de destino tenha o .NET instalado:

- **Para Linux (Ubuntu/Debian x64):**
  ```bash
  dotnet publish erp_pessoal/Presentation/Presentation.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/linux-x64
  ```
  *Execução no Linux:*
  ```bash
  chmod +x ./publish/linux-x64/Presentation
  ./publish/linux-x64/Presentation
  ```

- **Para Windows (x64):**
  ```bash
  dotnet publish erp_pessoal/Presentation/Presentation.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish/win-x64
  ```

### 4.3. Otimizações de Publicação Recomendadas
- `-p:PublishSingleFile=true`: Agrupa a aplicação em um único arquivo binário executável.
- `-p:PublishReadyToRun=true`: Compila o código em formato ReadyToRun (R2R) antecipado, acelerando drasticamente o tempo de startup da API.
- `-p:PublishTrimmed=false`: Mantido como false para preservar a integridade das chamadas reflexivas dinâmicas do Dapper.

Exemplo de comando completo otimizado para produção:
```bash
dotnet publish erp_pessoal/Presentation/Presentation.csproj -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o ./publish/production
```

---

## 5. Estrutura de Endpoints Principais

### Autenticação (`/contas`)
- `POST /contas/cadastro`: Cadastro de novo usuário.
- `POST /contas/login`: Autenticação e emissão de token JWT.

### Cadastros Gerais (`/user_plan`) [Requer Bearer JWT]
- **Rendas:** `ler_renda`, `ler_renda_view`, `criar_renda`, `atualizar_renda`, `inativar_renda/{id}`
- **Gastos:** `ler_gastos`, `ler_gastos_view`, `criar_gasto`, `atualizar_gasto`, `inativar_gasto/{id}`
- **Investimentos:** `ler_investimentos`, `ler_investimentos_view`, `criar_investimento`, `atualizar_investimento`, `inativar_investimento`, `resgatar_investimento`, `ler_investimentos_resgatados`
- **Dívidas:** `ler_dividas`, `ler_dividas_view`, `criar_divida`, `atualizar_divida`, `inativar_divida`, `quitar_divida`, `ler_dividas_quitadas`
- **Metas:** `ler_metas`, `ler_metas_view`, `ler_composicao_meta/{goalId}`, `criar_meta`, `atualizar_meta`, `inativar_meta/{id}`, `concluir_meta/{id}`, `ler_metas_concluidas`

### Extrato e Lançamentos Específicos (`/extrato`) [Requer Bearer JWT]
- `GET /extrato/ler_extrato`: Retorna movimentações com cálculo de saldo e vínculo com metas (`GoalId`, `GoalName`).
- `POST /extrato/incluir_lancamento`: Cria lançamento financeiro no extrato, vinculando a tabelas específicas e opcionalmente a uma meta.
- `PUT /extrato/atualizar_lancamento`: Atualiza lançamento e sincroniza o vínculo com a meta associada.
- `DELETE /extrato/remover_lancamento`: Inativa o lançamento e seus vínculos com segurança.
- `GET /extrato/obter_meta_pgto`: Retorna todos os pagamentos vinculados a metas, indicando o tipo de origem (`OriginType`: divida, investimento ou meta) e nome de origem (`OriginName`).
- `GET /extrato/obter_divida_pgto`: Pagamentos de dívidas.
- `GET /extrato/obter_investimento_pgto`: Aportes de investimentos.
- `GET /extrato/obter_gastos_pgto`: Pagamentos de despesas.
- `GET /extrato/obter_renda_pgto`: Entradas de renda.

### Indicadores e Preferências (`/thinking`) [Requer Bearer JWT]
- `GET /thinking/indicadores`: Gera sugestões de cortes orçamentários com base no histórico.
- `GET /thinking/ler_preferencias` e `POST /thinking/criar_preferencias`: Gerencia preferências do usuário sobre gastos essenciais/reduzíveis/bloqueados.
