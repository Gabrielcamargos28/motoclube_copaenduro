# Guia de Configuração do Banco de Dados

Este guia explica como configurar o banco de dados MySQL para o sistema Copa Cerrado de Enduro.

## 📋 Ordem de Execução dos Scripts

Execute os scripts SQL na seguinte ordem:

### 1. Criar o Banco de Dados e Tabelas Principais
```bash
mysql -u root -p < create_database.sql
```

Este script cria:
- Banco de dados `motoclube_copaenduro`
- Tabela `categorias` com 11 categorias pré-cadastradas
- Tabela `etapas` com 1 etapa inicial
- Tabela `inscritos` com índice único `(cpf, id_etapa)` para prevenir duplicação

### 2. Criar Tabelas do ASP.NET Identity e Usuário Admin
```bash
mysql -u root -p < setup_identity_and_admin.sql
```

Este script cria:
- Todas as tabelas necessárias do ASP.NET Identity:
  - `AspNetUsers`
  - `AspNetRoles`
  - `AspNetUserRoles`
  - `AspNetUserClaims`
  - `AspNetRoleClaims`
  - `AspNetUserLogins`
  - `AspNetUserTokens`
- Role `Admin`
- Usuário administrador com credenciais:
  - **Email:** admin@copacerrado.com.br
  - **Senha:** Admin@123

### 3. (Opcional) Adicionar Índice Único em Banco Existente

Se você já tem um banco de dados criado e precisa adicionar o índice único para prevenir CPF duplicado:

```bash
mysql -u root -p < add_unique_cpf_etapa_index.sql
```

**⚠️ ATENÇÃO:** Este script verifica se há duplicatas existentes. Se houver, você precisará resolvê-las manualmente antes que o índice único seja criado.

## 🔐 Credenciais de Acesso

### Área Administrativa

- **URL:** http://localhost:5000/Admin/Login
- **Email:** admin@copacerrado.com.br
- **Senha:** Admin@123

## ✅ Verificações Importantes

### 1. Verificar se o usuário admin foi criado
```sql
USE motoclube_copaenduro;

SELECT u.Email, u.NomeCompleto, r.Name as Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@copacerrado.com.br';
```

Deve retornar:
```
Email                          | NomeCompleto   | Role
admin@copacerrado.com.br       | Administrador  | Admin
```

### 2. Verificar se o índice único foi criado
```sql
SHOW INDEX FROM inscritos WHERE Key_name = 'idx_cpf_etapa';
```

Deve retornar uma linha indicando que o índice único existe.

### 3. Testar prevenção de duplicação
```sql
-- Esta query deve retornar 0 duplicatas
SELECT cpf, id_etapa, COUNT(*) as quantidade
FROM inscritos
GROUP BY cpf, id_etapa
HAVING COUNT(*) > 1;
```

## 🔧 Solução de Problemas

### Problema: Não consigo fazer login no admin

**Causa:** Tabelas do Identity não foram criadas ou usuário admin não existe.

**Solução:**
1. Execute o script `setup_identity_and_admin.sql`
2. Verifique se o usuário foi criado usando a query de verificação acima
3. Reinicie a aplicação

### Problema: Erro ao tentar fazer inscrição duplicada

**Causa:** Índice único não foi criado.

**Solução:**
1. Execute o script `add_unique_cpf_etapa_index.sql`
2. Se houver duplicatas, remova-as manualmente primeiro

### Problema: Erro "Duplicate entry" ao criar índice

**Causa:** Já existem inscrições duplicadas no banco.

**Solução:**
```sql
-- 1. Encontre as duplicatas
SELECT cpf, id_etapa, COUNT(*) as quantidade
FROM inscritos
GROUP BY cpf, id_etapa
HAVING COUNT(*) > 1;

-- 2. Para cada duplicata, mantenha apenas a primeira inscrição
-- (ajuste o WHERE conforme necessário)
DELETE i1 FROM inscritos i1
INNER JOIN inscritos i2
WHERE i1.id > i2.id
  AND i1.cpf = i2.cpf
  AND i1.id_etapa = i2.id_etapa;

-- 3. Tente criar o índice novamente
```

## 📝 Notas Importantes

1. **Backup:** Sempre faça backup do banco antes de executar scripts de migração
2. **Ordem:** Execute os scripts na ordem especificada
3. **Senha do Admin:** Altere a senha padrão após o primeiro login
4. **CPF Único:** O sistema agora previne que o mesmo CPF se inscreva mais de uma vez na mesma etapa
5. **PIX Único:** Cada inscrição gera um valor PIX único com centavos incrementais (200.01, 200.02, etc.)

## 🆘 Suporte

Em caso de dúvidas ou problemas, verifique:
- Logs da aplicação em `bin/Debug/net8.0/`
- Configuração de conexão em `appsettings.json`
- Permissões do usuário MySQL

---

**Última atualização:** 04/12/2025
