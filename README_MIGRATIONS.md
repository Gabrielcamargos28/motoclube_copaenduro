# Instruções para Migrations e Banco de Dados

## Opção 1: Usando Entity Framework Migrations (Recomendado)

### No Rider da JetBrains:

1. Abra o Terminal integrado no Rider (Alt+F12)
2. Execute os seguintes comandos:

```bash
# Criar a migration inicial
dotnet ef migrations add InitialCreate

# Aplicar a migration no banco de dados
dotnet ef database update
```

### Ou use o Package Manager Console:

1. No Rider, vá em `Tools > NuGet > Package Manager Console`
2. Execute:

```powershell
Add-Migration InitialCreate
Update-Database
```

## Opção 2: Criando o banco manualmente (Alternativa)

Se preferir criar o banco de dados manualmente:

1. Abra seu cliente MySQL (MySQL Workbench, phpMyAdmin, etc.)
2. Execute o script SQL em: `Migrations/create_database.sql`

## Configuração do Banco de Dados

Antes de executar as migrations, certifique-se de:

1. Ter o MySQL instalado e rodando
2. Atualizar a connection string em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=motoclube_copaenduro;User=seu_usuario;Password=sua_senha;"
  }
}
```

## Estrutura do Banco de Dados

O sistema criará 3 tabelas principais:

### categorias
- Armazena as categorias de competição (Elite, Importada Pró, Nacional Pró, etc.)

### etapas
- Armazena as etapas do campeonato
- Controla quais etapas estão com inscrições abertas

### inscritos
- Armazena os dados de inscrição dos pilotos
- Relaciona-se com categorias e etapas

## Verificando a Criação

Após executar as migrations, verifique se as tabelas foram criadas:

```sql
USE motoclube_copaenduro;
SHOW TABLES;
SELECT * FROM categorias;
SELECT * FROM etapas;
```

## Rodando a Aplicação

1. Configure a connection string corretamente
2. Execute as migrations
3. No Rider, pressione F5 ou clique em "Run"
4. Acesse: `https://localhost:5001` ou `http://localhost:5000`

## Rotas Disponíveis

- `/Inscricao` - Formulário de inscrição
- `/Inscricao/Inscritos` - Lista de inscritos
- `/Inscricao/Calendario` - Calendário de etapas
- `/Inscricao/PesquisaInscritos` - API JSON para DataTables

## Troubleshooting

### Erro de conexão com o banco:
- Verifique se o MySQL está rodando
- Confirme usuário e senha na connection string
- Verifique se o banco de dados existe

### Erro ao executar migrations:
```bash
# Limpar migrations
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Tabelas não criadas:
- Execute o script SQL manual em `Migrations/create_database.sql`
