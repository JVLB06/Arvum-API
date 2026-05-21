# ERP_pessoal_API
## Camadas
### Presentation
	Program.cs - inclusão de CORS, nginx (certificados) e importa os métodos usados e namespaces (das outas camadas)
	Controller - cuida dos endpoints
	WebModels - model de referência para recepção de dados
### Application
	DependencyInjection.cs - inclui esquema para chamar dependências
	Models - model de saída
	OutputMappers - converte DTO pra model de saída
	InputMappers - conversor da model de entrada pra entidade (DTO) (tanto WebModel quanto BaseModel)
	DTOs - model entidade de processamento
	Interfaces - injeção de dependências (injeta Persistence e Services)
	Services - chama a camada de domínio (Entities), junto a Persistence e é chamada pela camada de presentation (controller)
### Domain
	DependencyInjection.cs - inclui esquema para chamar dependências
	Entities - estrutura principal de processamento (regra de negócio)
	Helpers - estrutura adicional do Entities utilizada de forma repetitiva ou fragmentada
### Infrastructure
	DependencyInjection.cs - inclui esquema para chamar dependências e possivelmente configurações para chamadas externas (bancos e APIs com barreiras específicas)
	BaseModels - model vinda direta do banco via Dapper
	Persistence - salva as queries a serem utilizadas e faz a conexão com o Dapper
	Repositories - configura conexão ao banco de dados
	ServicosExternos - configura conexão a APIs externas (se tiver)

## Instruções
Dentro de cada .csproj incluir a referência a camada que ele deve ver (ex: Presentation chama apenas Application e assim por diante)

## Recursos
Adquirir domínio, DNS e certificado SSL com a CloudFlare (50 a 60 reais por ano)
Banco de dados (Neon.tech ou Supabase)
Backend (Render, necessário criar scheduler para acordar a máquina via server Oracle)
	Alternativos Back (Hostinger/ Locaweb - VPS Ubuntu - cerca de 20-30 reais/mês)
