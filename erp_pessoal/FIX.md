# Registro de Correções

## Bugs Corrigidos

### 1. SpecificRegistersWriter.cs - CreateDebtExtractAsync: Coluna incorreta
**Arquivo:** Infrastructure/Persistence/Writers/SpecificRegistersWriter.cs  
**Linha:** 61  
**Problema:** INSERT usava coluna `invest_id` quando a tabela `divida_pgto` possui `divida_id`  
**Correção:** Alterado `invest_id` para `divida_id`  
**Impacto:** Criação de lançamentos de dívida falhava silenciosamente

### 2-6. SpecificRegistersWriter.cs - Métodos CreatexxxExtractAsync: Retorno incorreto
**Arquivo:** Infrastructure/Persistence/Writers/SpecificRegistersWriter.cs  
**Linhas:** 24, 44, 66, 110, 132  
**Problema:** Métodos usavam `ExecuteAsync()` para retornar ID do INSERT...RETURNING  
- CreateMainExtractAsync
- CreateExpenseExtractAsync
- CreateDebtExtractAsync (também)
- CreateGoalExtractAsync
- CreateReceiptExtractAsync

**Correção:** Alterado para `QueryFirstOrDefaultAsync<int>()` para capturar o RETURNING  
**Impacto:** Criação de lançamentos não retornava ID correto, causando erro no cálculo de saldos

### 7. SpecificRegistersReader.cs - ReadInvestmentsEntryByUser: Filtro de datas ausente
**Arquivo:** Infrastructure/Persistence/Readers/SpecificRegistersReader.cs  
**Linha:** 265  
**Problema:** Query não aplicava filtro de datas BETWEEN  
**Correção:** Adicionado `AND e.data BETWEEN @initialDate AND @endDate` ao WHERE  
**Impacto:** Leitura de lançamentos de investimento retornava todos os dados, sem respeitar range de datas

### 8-9. ThinkingWriter.cs - SetPreferenceAsync e PutPreferenceAsync: Coluna incorreta
**Arquivo:** Infrastructure/Persistence/Writers/ThinkingWriter.cs  
**Linhas:** 21, 44  
**Problema:** INSERT/UPDATE usavam coluna `BLOQUEAR` quando a tabela possui `BLOQUEADO`  
**Correção:** Alterado `BLOQUEAR` para `BLOQUEADO`  
**Impacto:** Criação/atualização de preferências de usuário falhava

### 10. GeneralGoalsWriter.cs - CreateGoalAsync: Campo ausente no INSERT
**Arquivo:** Infrastructure/Persistence/Writers/GeneralGoalsWriter.cs  
**Linha:** 14  
**Problema:** INSERT especificava `UserId` mas coluna na tabela é `user_id`; coluna não estava no INSERT  
**Correção:** Alterado para usar `user_id` corretamente  
**Impacto:** Criação de metas falhava ou criava registros sem associação ao usuário

### 11. GeneralGoalsWriter.cs - EndGoalAsync: Sintaxe SQL inválida
**Arquivo:** Infrastructure/Persistence/Writers/GeneralGoalsWriter.cs  
**Linha:** 74  
**Problema:** Faltava vírgula entre `ativo = FALSE` e `progresso = 100`  
**Correção:** Adicionado `,` após `ativo = FALSE`  
**Impacto:** Conclusão de metas resultava em erro de sintaxe SQL

## Resumo
- **Total de bugs:** 11
- **Bugs críticos (execução):** 8
- **Bugs de dados:** 3
- **Camadas afetadas:** Infrastructure (Readers/Writers)
- **Funcionalidades impactadas:** Lançamentos, Investimentos, Metas, Preferências

## Testes Recomendados
1. Criar lançamento de dívida com ID retornado correto
2. Criar lançamento de investimento com filtro de datas
3. Criar e finalizar meta com progresso atualizado
4. Criar preferência de usuário sem erro de sintaxe
5. Ler lançamentos de investimento respeitando intervalo de datas
