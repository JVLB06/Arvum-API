# Sugestões de Melhoria

## Arquitetura e Padrões

### 1. Gerenciamento de Conexão
**Localização:** Infrastructure/Repositories/MainRepository.cs  
**Problema:** Cada operação abre nova conexão, sem pool configuration explícita  
**Sugestão:** Configurar pool de conexões em appsettings e usar IDbConnection injetado via DI  
**Benefício:** Melhor performance e reutilização de conexões

### 2. Credenciais Hardcoded
**Localização:** MainRepository.cs linha 9  
**Problema:** String de conexão com senha em código-fonte  
**Sugestão:** Mover para appsettings.json e usar Configuration  
**Benefício:** Segurança e flexibilidade de ambiente

### 3. Chave JWT Hardcoded
**Localização:** Domain/Helpers/AuthHelper.cs  
**Problema:** Chave secreta em código-fonte  
**Sugestão:** Armazenar em appsettings.json ou Azure Key Vault  
**Benefício:** Segurança e conformidade

## Consistência e Nomenclatura

### 4. Typo no Nome do Método
**Localização:** ISpecificRegistersService.cs linha 10  
**Problema:** `GetExpensePayementsAsync` (typo: "Payements" ao invés de "Payments")  
**Sugestão:** Renomear para `GetExpensePaymentsAsync`  
**Impacto:** Compatibilidade com padrões de nomenclatura

### 5. Inconsistência de Case em SQL
**Localização:** Múltiplos Writers  
**Problema:** Alguns SELECTs usam UPPER CASE, outros mixed case  
**Sugestão:** Padronizar em UPPER CASE para keywords SQL  
**Benefício:** Legibilidade consistente

## Segurança e Validação

### 6. Falta de Validações em DTOs
**Localização:** Application/DTOs  
**Problema:** DTOs sem DataAnnotations (Required, StringLength, Range, etc)  
**Sugestão:** Adicionar validações com [Required], [StringLength], [Range]  
**Benefício:** Validação automática e documentação do contrato

### 7. Falta de Sanitização de Entrada
**Localização:** Controllers  
**Problema:** Sem trim/normalização de strings de entrada  
**Sugestão:** Adicionar sanitização em mappers de entrada  
**Benefício:** Prevenção de dados mal-formatados

## Performance e Eficiência

### 8. N+1 Queries em GoalEntity
**Localização:** GeneralGoalsService.cs:22-32  
**Problema:** Mapeia cada resultado em new GoalEntity sem bulk operations  
**Sugestão:** Considerar batch processing para grandes volumes  
**Benefício:** Redução de alocações em heap

### 9. Verificações Redundantes
**Localização:** Readers (ex: GeneralReceiptsReader.cs:32-33)  
**Problema:** Verifica `if (results is null) return Empty` e depois `if (!results.Any())`  
**Sugestão:** Consolidar em uma única verificação  
**Benefício:** Código mais conciso

### 10. Queries Genéricas sem Índices Aparentes
**Localização:** SpecificRegistersReader.cs  
**Problema:** Queries complexas com múltiplos JOINs sem índices documentados  
**Sugestão:** Criar índices em (user_id, ativo, data) para melhor performance  
**Benefício:** Redução de tempo de query

## Manutenibilidade

### 11. Métodos Muito Longos
**Localização:** SpecificRegistersWriter.cs (múltiplos métodos similares)  
**Problema:** 6 métodos CreatexxxExtractAsync praticamente idênticos  
**Sugestão:** Generalizar em método único com parâmetros de tabela/colunas  
**Benefício:** DRY principle, menos código para manter

### 12. Magic Strings de Tipos
**Localização:** SpecificRegistersService.cs:137-156  
**Problema:** Switch com strings "gasto", "divida", etc  
**Sugestão:** Enum ExtractKind para type-safety  
**Benefício:** Segurança em tempo de compilação

### 13. Falta de Logging
**Localização:** Todo o código  
**Problema:** Sem logs de operações ou erros  
**Sugestão:** Adicionar ILogger do Serilog/NLog  
**Benefício:** Debugging e auditoria

### 14. Ausência de Testes Unitários
**Localização:** Não há pasta Tests  
**Problema:** Sem cobertura de testes  
**Sugestão:** Criar xUnit tests para Services e Mappers  
**Benefício:** Confiabilidade e regressão prevention

## Otimizações SQL

### 15. Cálculo de Saldo Ineficiente
**Localização:** SpecificRegistersService.cs:223-244  
**Problema:** Cálcula saldos iterativamente em C# após leitura  
**Sugestão:** Usar window functions SQL (SUM OVER) para cálculo direto  
**Benefício:** Melhor performance em grandes datasets

### 16. LEFT JOINs Desnecessários
**Localização:** SpecificRegistersReader.cs:32-42  
**Problema:** 5 LEFT JOINs para determinar Kind de uma entrada  
**Sugestão:** Armazenar Kind diretamente em tabela extrato  
**Benefício:** Eliminaria a necessidade de múltiplos JOINs

## Resumo de Prioridades

**Alta:** Itens 2, 3, 6, 7, 12, 13  
**Média:** Itens 1, 4, 10, 11, 14, 15  
**Baixa:** Itens 5, 8, 9, 16
