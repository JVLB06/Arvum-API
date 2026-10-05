# Especificação Técnica e Arquitetura - Arvum API (ERP Pessoal)

## 1. Visão Geral do Sistema
O **Arvum API** (anteriormente referenciado como `erp_pessoal`) é um backend construído em **.NET 9 (C#)** voltado para a **gestão financeira pessoal**. O objetivo do sistema é permitir que os usuários gerenciem suas finanças através de duas categorias fundamentais de registros:
1. **Cadastros Gerais (Tipos/Estruturas de Lançamento):**
   - **Rendas:** Fontes de receita previstas/recorrentes (`rendas`).
   - **Gastos:** Despesas orçadas fixas ou variáveis (`gastos`).
   - **Investimentos:** Aplicações financeiras ativas ou finalizadas com taxa de juros (`investimentos`).
   - **Dívidas:** Empréstimos, financiamentos e passivos a pagar (`divida`).
   - **Metas:** Objetivos financeiros com prazo e valor alvo (`meta`).
2. **Cadastros Específicos (Lançamentos / Extrato Realizado):**
   - Transações financeiras efetivamente ocorridas (`extrato`), que se ramificam em tabelas de vínculo específicas (`pagamentos`, `divida_pgto`, `investimento_pgto`, `renda_pgto`, `meta_pgto`).
   - Mantém controle cronológico de saldos acumulados e liquidações.
3. **Módulo de Inteligência / Indicadores (`Thinking`):**
   - Análise de comportamento financeiro, preferências de restrição de gastos (`restricoes_usuario`) e sugestões de corte/revisão orçamentária.

> **Importante:** A pasta/projeto legado `erp_old` está totalmente em desuso e deve ser desconsiderada. Todo o desenvolvimento ativo reside nos projetos da solução `erp_pessoal`.

---

## 2. Arquitetura da Solução

O sistema adota uma **Arquitetura em Camadas (N-Tier / Clean Architecture simplificada)**, dividida em quatro projetos principais dentro da pasta `erp_pessoal`:

```
erp_pessoal/
├── Presentation/       # Camada de Apresentação (Web API, Controllers, WebModels, Mappers de entrada)
├── Application/        # Camada de Aplicação (Services, Interfaces, DTOs)
├── Domain/             # Camada de Domínio (Entities, Regras de Negócio Puras, Helpers)
├── Infrastructure/     # Camada de Infraestrutura (Dapper, PostgreSQL/Npgsql, Repositories, Persistence Readers/Writers)
└── erp_old/            # [DESCONTINUADO] Código legado
```

### 2.1. Fluxo de Dependências
- **Presentation** referencia: `Application`, `Infrastructure`.
- **Application** referencia: `Domain`.
- **Infrastructure** referencia: `Domain`, `Application` (para implementar as interfaces definidas em `Application/Interfaces`).
- **Domain** não referencia nenhuma camada externa (independente).

### 2.2. Responsabilidade das Camadas

#### Camada de Apresentação (`Presentation`)
- Configuração do pipeline HTTP em `Program.cs` (CORS, JWT Bearer Authentication, Swagger).
- Controllers:
  - `AuthController`: Cadastro de usuários e login com geração de token JWT.
  - `GeneralRegistersController` (`/user_plan`): Gerenciamento dos cadastros gerais (Rendas, Gastos, Dívidas, Investimentos, Metas) e visualizações consolidadas (`*_view`).
  - `SpecificRegistersController` (`/extrato`): Gerenciamento do extrato e lançamentos específicos vinculados.
  - `ThinkingController` (`/thinking`): Gestão de preferências e regras heurísticas financeiras.
- `WebModels`: Modelos que definem o payload recebido e enviado via HTTP.
- `InputMappers`: Mapeamento de `WebModel` para `DTO` da camada de aplicação.

#### Camada de Aplicação (`Application`)
- `Interfaces`: Define os contratos de repositório (`I*Reader`, `I*Writer`) e de serviços (`I*Service`).
- `Services`: Orquestram a lógica do fluxo, chamam os leitores/escritores de persistência e realizam cálculos agregados.
- `DTOs`: Objetos de transferência de dados desacoplados de bibliotecas externas e da UI.

#### Camada de Domínio (`Domain`)
- `Entities`: Representações do modelo de negócio (ex.: `ExtractEntity`, `GoalEntity`, `DebtEntity`, etc.).
- `Helpers`: Lógicas utilitárias de domínio, ex.: `ExtractHelper` (normalização de sinal financeiro positivo/negativo) e `AuthHelper` (hashing e assinatura JWT).

#### Camada de Infraestrutura (`Infrastructure`)
- `Repositories/MainRepository`: Fábrica de conexões PostgreSQL via `NpgsqlConnection`.
- `BaseModels`: Modelos que refletem 1:1 o mapeamento de tabelas e colunas do PostgreSQL.
- `BaseMappers`: Mapeamento entre `BaseModel` e `DTO`.
- `Persistence/Readers`: Implementam as interfaces de leitura utilizando **Dapper** para execução de consultas SQL de alta performance.
- `Persistence/Writers`: Implementam as interfaces de escrita (INSERT, UPDATE, DELETE lógico) utilizando **Dapper**.

---

## 3. Modelo de Banco de Dados (PostgreSQL)

O banco de dados relacional utiliza o PostgreSQL. Principais tabelas e seus relacionamentos:

### 3.1. Cadastros Gerais
- **`usuarios`**: `id`, `nome`, `email`, `senha`, `nascimento`, `tipo`, `ativo`.
- **`rendas`**: `id_renda`, `user_id`, `nome`, `vlr_min`, `vlr_max`, `data_pag`, `ativo`, `valor`.
- **`gastos`**: `id_gasto`, `user_id`, `nome`, `vlr_min`, `vlr_max`, `data_venc`, `fixvar`, `prioridade`, `ativo`.
- **`investimentos`**: `id_invest`, `user_id`, `nome`, `vlr`, `juro`, `data_init`, `data_fim`, `resgate`, `data_resgate`, `ativo`.
- **`divida`**: `id_invest` (PK), `user_id`, `nome`, `vlr`, `data`, `data_prev`, `quitada`, `ativo`.
- **`meta`**: `id_meta`, `user_id`, `nome`, `vlr`, `data_meta`, `progresso`, `ativo`.

### 3.2. Lançamentos Específicos (Extrato e Detalhes)
- **`extrato`**: Registra cada movimentação bancária real do usuário.
  - Colunas: `id_lcto` (PK), `user_id`, `data`, `historico`, `vlr`, `saldo`, `ativo`.
- **`pagamentos`**: Detalhe de pagamento de gastos (`lcto_id -> extrato.id_lcto`, `gasto_id -> gastos.id_gasto`).
- **`divida_pgto`**: Detalhe de amortização/parcela de dívida (`lcto_id -> extrato.id_lcto`, `divida_id -> divida.id_invest`).
- **`investimento_pgto`**: Detalhe de aporte em investimento (`lcto_id -> extrato.id_lcto`, `invest_id -> investimentos.id_invest`).
- **`renda_pgto`**: Detalhe de recebimento de renda (`lcto_id -> extrato.id_lcto`, `renda_id -> rendas.id_renda`).
- **`meta_pgto`**: Detalhe de destinação para uma meta financeira (`lcto_id -> extrato.id_lcto`, `meta_invest_id -> meta.id_meta`).

---

## 4. Composição Multi-Origem de Metas (Dívidas e Investimentos compondo Metas)

### 4.1. Cenário Anterior
- Cada lançamento de extrato (`NewExtractModel`) pertencia estritamente a um único `Kind`:
  - Se fosse `"divida"`, gerava registro apenas em `divida_pgto`.
  - Se fosse `"investimento"`, gerava registro apenas em `investimento_pgto`.
  - Se fosse `"meta"`, gerava registro apenas em `meta_pgto`.
- **Limitação:** Não era possível representar que o pagamento de um financiamento imobiliário (Dívida) ou a aplicação em um CDB (Investimento) faziam parte da meta de "Comprar uma Casa". O usuário ficava impossibilitado de vincular seus lançamentos de investimentos e amortizações de dívidas às suas metas financeiras.

### 4.2. Solução Arquitetural Implementada
1. **Lançamento com Vínculo de Meta Opcional (`GoalId`):**
   - Ao lançar uma `"divida"` ou `"investimento"` no extrato (`NewExtractModel`), o usuário pode opcionalmente informar `GoalId`.
   - O extrato cria o lançamento financeiro principal (`extrato`) e o detalhe correspondente (`divida_pgto` ou `investimento_pgto`).
   - Se `GoalId` for informado, o sistema registra simultaneamente uma entrada em `meta_pgto`, associando aquele mesmo `lcto_id` à meta desejada.
2. **Atualização e Remoção Integradas:**
   - Na atualização de um lançamento (`PUT /extrato/atualizar_lancamento`), a associação com a meta é sincronizada através de `UpsertGoalExtractAsync` (criação, atualização ou desvinculação em `meta_pgto`).
   - Na inativação do lançamento no extrato (`DELETE /extrato/remover_lancamento`), tanto o detalhe da dívida/investimento quanto o registro em `meta_pgto` são inativados com segurança.
   - O recálculo de saldos (`CalculateBalancesAsync`) foi corrigido para utilizar o `id_lcto` correto do extrato.
3. **Leitura e Identificação de Origem no Extrato:**
   - `ReadExtractByUser` retorna `ExternalId`, `GoalId` e `GoalName` em cada linha do extrato.
   - A precedência no `CASE` do SQL garante que um pagamento de dívida ou investimento vinculado a uma meta mantenha sua classificação primária (`divida` ou `investimento`), sem perder a referência da meta.
4. **Visão de Detalhes dos Pagamentos da Meta (`obter_meta_pgto`):**
   - O endpoint agora identifica se a contribuição para a meta veio de uma **dívida** (amortização), de um **investimento** (aporte) ou de um **aporte direto de meta**, incluindo `OriginType` e `OriginName`.
5. **Cálculo Dinâmico de Progresso das Metas (`ler_metas` e `ler_metas_view`):**
   - O progresso de cada meta passa a ser calculado dinamicamente com base no montante real acumulado em `meta_pgto` (somando investimentos, dívidas pagas e aportes diretos vinculados).
   - Disponibilizado o endpoint `GET /user_plan/ler_metas_view` com `GoalPaid` (valor já acumulado) e `Progress` (% calculado).
   - Disponibilizado o endpoint `GET /user_plan/ler_composicao_meta/{goalId}` detalhando quais dívidas, investimentos e aportes compõem a meta.

---

## 5. Exemplos de Payloads para Integração Frontend

### 5.1. Criar Lançamento de Dívida Vinculado a uma Meta
**Endpoint:** `POST /extrato/incluir_lancamento`
```json
{
  "name": "Parcela 01/360 - Financiamento Habitacional",
  "value": 2500.00,
  "extractDate": "2026-10-15T00:00:00",
  "kind": "divida",
  "externalId": 4,
  "goalId": 1
}
```
*Resultado:*
- Cria registro em `extrato` (valor: `-2500.00`).
- Cria registro em `divida_pgto` (vinculando a `divida_id = 4`).
- Cria registro em `meta_pgto` (vinculando a `meta_invest_id = 1` com valor positivo `2500.00`).
- Recalcula os saldos cronológicos do extrato.

### 5.2. Criar Lançamento de Investimento Vinculado a uma Meta
**Endpoint:** `POST /extrato/incluir_lancamento`
```json
{
  "name": "Aporte CDB Liquidez Diária",
  "value": 1500.00,
  "extractDate": "2026-10-20T00:00:00",
  "kind": "investimento",
  "externalId": 2,
  "goalId": 1
}
```
*Resultado:*
- Cria registro em `extrato` (valor: `-1500.00`).
- Cria registro em `investimento_pgto` (vinculando a `invest_id = 2`).
- Cria registro em `meta_pgto` (vinculando a `meta_invest_id = 1` com valor positivo `1500.00`).

### 5.3. Consultar Composição da Meta
**Endpoint:** `GET /user_plan/ler_composicao_meta/1`
**Resposta:**
```json
{
  "goalId": 1,
  "goalName": "Comprar uma Casa",
  "targetValue": 300000.00,
  "goalDate": "2030-12-31T00:00:00",
  "totalAccumulated": 4000.00,
  "progressPercentage": 1.33,
  "items": [
    {
      "id": 4,
      "name": "Financiamento Habitacional",
      "totalContributed": 2500.00,
      "type": "divida"
    },
    {
      "id": 2,
      "name": "CDB Liquidez Diária",
      "totalContributed": 1500.00,
      "type": "investimento"
    }
  ]
}
```

### 5.4. Consultar Pagamentos com Origem Multi-Tipo
**Endpoint:** `GET /extrato/obter_meta_pgto?InitialDate=2026-01-01&EndDate=2026-12-31`
**Resposta:**
```json
[
  {
    "id": 14,
    "specificId": 8,
    "extractDate": "2026-10-15T00:00:00",
    "description": "Parcela 01/360 - Financiamento Habitacional",
    "entryValue": 2500.00,
    "goalId": 1,
    "goalName": "Comprar uma Casa",
    "fullGoalValue": 300000.00,
    "goalDate": "2030-12-31T00:00:00",
    "progress": 1.33,
    "balance": 28000.00,
    "originType": "divida",
    "originName": "Financiamento Habitacional"
  },
  {
    "id": 15,
    "specificId": 9,
    "extractDate": "2026-10-20T00:00:00",
    "description": "Aporte CDB Liquidez Diária",
    "entryValue": 1500.00,
    "goalId": 1,
    "goalName": "Comprar uma Casa",
    "fullGoalValue": 300000.00,
    "goalDate": "2030-12-31T00:00:00",
    "progress": 1.33,
    "balance": 26500.00,
    "originType": "investimento",
    "originName": "CDB Liquidez Diária"
  }
]
```

---

## 6. Mapeamento dos Arquivos Alterados

| Camada | Arquivo | Responsabilidade Alterada |
| :--- | :--- | :--- |
| **Domain** | [`ExtractEntity.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Domain/Entities/ExtractEntity.cs) | Inclusão de `GoalId` no construtor e propriedade. |
| **Domain** | [`SpecificGoalEntity.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Domain/Entities/SpecificGoalEntity.cs) | Inclusão de `OriginType` e `OriginName` para identificar proveniência do aporte. |
| **Domain** | [`GoalEntity.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Domain/Entities/GoalEntity.cs) | Inclusão de `GoalPaid` (valor monetário acumulado). |
| **Application** | [`ExtractDTO.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/DTOs/ExtractDTO.cs) | Inclusão de `GoalId` e `GoalName`. |
| **Application** | [`SpecificGoalDTO.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/DTOs/SpecificGoalDTO.cs) | Inclusão de `OriginType` e `OriginName`. |
| **Application** | [`GoalDTO.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/DTOs/GoalDTO.cs) | Inclusão de `GoalPaid`. |
| **Application** | [`GoalCompositionDTO.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/DTOs/GoalCompositionDTO.cs) | Novo DTO para detalhamento e breakdown de metas compostas por dívidas e investimentos. |
| **Application** | [`ISpecificRegistersWriter.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/Interfaces/ISpecificRegistersWriter.cs) | Assinatura de `UpsertGoalExtractAsync`. |
| **Application** | [`IGeneralGoalsReader.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/Interfaces/IGeneralGoalsReader.cs) | Assinatura de `GetGoalCompositionAsync`. |
| **Application** | [`IGeneralGoalsService.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/Interfaces/IGeneralGoalsService.cs) | Assinaturas de `GetGoalsProgressAsync` e `GetGoalCompositionAsync`. |
| **Application** | [`GeneralGoalsService.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/Services/GeneralGoalsService.cs) | Implementação dos métodos de agregação de metas e composição. |
| **Application** | [`SpecificRegistersService.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Application/Services/SpecificRegistersService.cs) | Orquestração da vinculação automática de `meta_pgto` em lançamentos de dívida e investimento; correção do ID para cálculo de saldos. |
| **Infrastructure** | [`ExtractBaseModel.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/BaseModels/ExtractBaseModel.cs) | Suporte a `ExternalId`, `GoalId` e `GoalName`. |
| **Infrastructure** | [`SpecificGoalBaseModel.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/BaseModels/SpecificGoalBaseModel.cs) | Suporte a `OriginType` e `OriginName`. |
| **Infrastructure** | [`GoalBaseModel.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/BaseModels/GoalBaseModel.cs) | Suporte a `GoalPaid`. |
| **Infrastructure** | [`ExtractMapper.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/BaseMappers/ExtractMapper.cs) | Mapeamento dos novos campos de extrato. |
| **Infrastructure** | [`SpecificGoalMapper.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/BaseMappers/SpecificGoalMapper.cs) | Mapeamento de `OriginType` e `OriginName`. |
| **Infrastructure** | [`GoalMapper.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/BaseMappers/GoalMapper.cs) | Mapeamento de `GoalPaid`. |
| **Infrastructure** | [`SpecificRegistersWriter.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/Persistence/Writers/SpecificRegistersWriter.cs) | Implementação de `UpsertGoalExtractAsync` e normalização de valor absoluto. |
| **Infrastructure** | [`SpecificRegistersReader.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/Persistence/Readers/SpecificRegistersReader.cs) | Atualização das queries SQL para extrair meta associada e identificar origem de pagamentos. |
| **Infrastructure** | [`GeneralGoalsReader.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Infrastructure/Persistence/Readers/GeneralGoalsReader.cs) | Cálculo de progresso dinâmico via agregação de pagamentos e consulta da composição da meta. |
| **Presentation** | [`NewExtractModel.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Presentation/WebModels/NewExtractModel.cs) | Campo opcional `GoalId` para recebimento no endpoint de inclusão/atualização. |
| **Presentation** | [`NewExtractMapper.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Presentation/InputMappers/NewExtractMapper.cs) | Mapeamento de `GoalId` para `ExtractDTO`. |
| **Presentation** | [`GeneralRegistersController.cs`](file:///C:/Users/User/Documents/Arvum-API/erp_pessoal/Presentation/Controllers/GeneralRegistersController.cs) | Novos endpoints `ler_metas_view` e `ler_composicao_meta/{goalId}`. |
| **Raiz** | [`README.md`](file:///C:/Users/User/Documents/Arvum-API/README.md) | Guia completo de build para testes e publicação para runtime/produção. |
| **Raiz** | [`SPEC.md`](file:///C:/Users/User/Documents/Arvum-API/SPEC.md) | Especificação técnica completa do sistema e das regras de negócio implementadas. |
