
# Ajuste a versão para o TargetFramework da sua aplicação.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copia a solução e os quatro projetos
COPY . .

# Compila a Presentation e suas dependências
RUN dotnet publish Presentation/Presentation.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# Imagem de execução
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# O container precisa escutar na porta fornecida pela Vercel
CMD ["sh", "-c", "dotnet Presentation.dll --urls http://0.0.0.0:${PORT:-80}"]
