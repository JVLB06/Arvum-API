# Especificação Técnica - ERP Pessoal

## Visão Geral

Sistema de controle de finanças pessoal desenvolvido em .NET com arquitetura em camadas (Domain, Application, Infrastructure, Presentation) utilizando padrões de repositório e injeção de dependência.

## Estrutura de Camadas

### Domain
Contém as entidades de negócio e helpers de validação.

**Entidades principais:**
- `UserEntity`: Usuário do sistema com validações de nome, email e senha
- `ReceiptEntity`: Renda (rendas)
- `ExpenseEntity`: Gasto (gastos)
- `DebtEntity`: Dívida (divida)
- `InvestmentEntity`: Investimento (investimentos)
- `GoalEntity`: Meta (meta)
- `ExtractEntity`: Lançamento do extrato (extrato)
- `PreferenceEntity`: Restrição do usuário (restricoes_usuario)
- `LoginEntity`: Entidade de login
- `ConnectionEntity`: Entidade de validação de conexão

**Helpers:**
- `AuthHelper`: Contém chave JWT e métodos de autenticação
- `ExtractHelper`: Normaliza valores de lançamentos conforme o tipo
- `ThinkingHelper`: Lógica para sugestões inteligentes

### Application
Camada de aplicação com serviços, DTOs e interfaces.

**Serviços:**
- `AuthService`: Registro e login de usuários
- `GeneralReceiptsService`: CRUD de rendas
- `GeneralInvestmentsService`: CRUD de investimentos
- `GeneralDebtsService`: CRUD de dívidas
- `GeneralGoalsService`: CRUD de metas
- `GeneralExpensesService`: CRUD de gastos
- `SpecificRegistersService`: Gerenciamento de lançamentos no extrato
- `ThinkingService`: Sugestões e preferências do usuário

**DTOs:**
- Mapeiam dados entre camadas
- Usados em controllers e readers/writers

**Interfaces:**
- Segregadas em Reader (leitura), Writer (escrita) e Service (orquestração)

### Infrastructure
Camada de persistência com Dapper e PostgreSQL.

**Estrutura:**
- `MainRepository`: Gerencia conexão com PostgreSQL
- `Readers`: Leem dados do banco (QueryAsync com Dapper)
- `Writers`: Persistem dados no banco (ExecuteAsync com Dapper)
- `BaseMappers`: Convertem BaseModels em DTOs
- `BaseModels`: Modelos de dados lidos do banco

### Presentation
Camada de apresentação com controllers ASP.NET Core.

**Controllers:**
- `AuthController`: Autenticação e autorização
- `GeneralRegistersController`: CRUD de cadastros gerais
- `SpecificRegistersController`: Lançamentos específicos
- `ThinkingController`: Sugestões inteligentes

**InputMappers:**
- Convertem WebModels em DTOs

## Banco de Dados

### Tabelas principais
- `usuarios`: Usuários do sistema
- `rendas` / `renda_pgto`: Rendas e seus lançamentos
- `gastos` / `pagamentos`: Gastos e seus lançamentos
- `divida` / `divida_pgto`: Dívidas e seus lançamentos
- `investimentos` / `investimento_pgto`: Investimentos e seus lançamentos
- `meta` / `meta_pgto`: Metas e seus lançamentos
- `extrato`: Lançamentos centralizados
- `restricoes_usuario`: Preferências do usuário

### Fluxo de dados
1. Solicitação chega ao Controller
2. Controller mapeia entrada e chama Service
3. Service orquestra Reader/Writer
4. Reader/Writer usa Dapper para executar SQL
5. Resultado é mapeado para DTO/Entity
6. Response é retornada

## Padrões utilizados

- **Repository**: MainRepository centraliza conexão
- **Dependency Injection**: Registrado em DependencyInjection.cs
- **DTO Pattern**: Separação entre camadas
- **Mapper Pattern**: Conversão entre modelos
- **Segregated Interfaces**: Reader/Writer separadas

## Autenticação

- JWT com chave secreta armazenada em AuthHelper
- Tokens gerados no login
- Validação em endpoints com [Authorize]
- ClaimTypes.NameIdentifier usado para extrair userId

## Fluxo de lançamentos

1. Usuário cria lançamento via SetExtractAsync
2. Sistema insere em `extrato` (tabela central)
3. Sistema insere em tabela específica (pagamentos, divida_pgto, etc)
4. Sistema recalcula saldos acumulados de lançamentos posteriores
5. Lançamentos deletados apenas marcam ativo = FALSE

## Considerações de implementação

- Null safety: Uso de null coalescing e Optional null-checks
- Async/await: Todas operações de banco são assíncronas
- Soft deletes: Registros deletados apenas marcam ativo = FALSE
- Connection pool: MainRepository cria novas conexões por operação
- Dapper mapping: Aliases no SQL mapeiam colunas para properties


## 7. Problemas Conhecidos Corrigidos

### 7.1. Erro de ambiguidade SQL em consulta de dívidas
**PROBLEMA:** Erro "42702: column reference 'user_id' is ambiguous" em `ler_dividas_view`.
**SOLUÇÃO:** Qualificar colunas com alias de tabela na query SQL do `ReadDebtsProgressAsync` (d.user_id, d.ativo).

### 7.2. Meta não atualiza no banco
**PROBLEMA:** Atualização de meta retorna sucesso mas não persiste no banco.
**SOLUÇÃO:** Corrigir ordem de parâmetros no `UpdateGoalAsync` - o construtor de GoalEntity espera (userId, id, ...) e não (id, userId, ...).

### 7.3. Campos de dívida incompatíveis
**PROBLEMA:** Erro "the description field is required" ao atualizar dívida.
**SOLUÇÃO:** Backend espera `Description`, `InitDate` e `EndDate` mas frontend enviava `name`, `initialDate` e `receiveDate`.

### 8.1. Thinking - endpoint desalinhado e falta de precisão
**PROBLEMA:** O endpoint `/thinking/indicadores` retornava estrutura com campos em inglês (`ExclusionSugestions`, `ReductionSugestions`) incompatível com o frontend, que consome `pensamentos`, `reducoes` e `exclusoes`. Resultado: a tela de recomendações não exibia nenhum dado real. Também não havia comparação de gastos x renda e os pensamentos eram estáticos.
**SOLUÇÃO:** `ThinkingService.GeneratePreferencesAsync` reestruturado para retornar `pensamentos` (gerados dinamicamente com base no indicador de saúde financeira, situação do caixa e endividamento), `reducoes` e `exclusoes` no formato `{gastoId, nome, valorAtual, valorSugerido}`, e um comparativo `{renda, gastos, gastosFixos, gastosVariaveis, saldoPositivo, razaoGastosRenda, mensagem}` que informa se o usuário precisa mesmo reduzir.

### 8.2. Investimentos - juros não enviados para o front
**PROBLEMA:** O mapper `InvestmentMapper.ToDTO` omitia o campo `Interest`. A taxa de juros, embora salva no banco, não era enviada ao frontend, e a projeção de valor futuro (calculada com base na taxa) ficava igual ao valor aplicado.
**SOLUÇÃO:** Campo `Interest` incluído no mapeamento de retorno (`Infrastructure/BaseMappers/InvestmentMapper.cs`), corrigindo a exibição da taxa na listagem e o cálculo da previsão de rendimentos.

