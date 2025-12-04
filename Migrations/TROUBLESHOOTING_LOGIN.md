# Troubleshooting - Problema de Login Administrativo

## 🔍 Diagnóstico do Problema

Se o botão de login não está respondendo ou a rota não está sendo chamada, siga este guia passo a passo.

## 📋 Passo 1: Executar Script de Diagnóstico

Execute o script de diagnóstico para verificar se o banco está configurado corretamente:

```bash
mysql -u root -p < Migrations/diagnostico.sql
```

### ✅ O que você deve ver:

1. **Pelo menos 7 tabelas do Identity:**
   - AspNetUsers
   - AspNetRoles
   - AspNetUserRoles
   - AspNetUserClaims
   - AspNetRoleClaims
   - AspNetUserLogins
   - AspNetUserTokens

2. **1 usuário admin:**
   - Email: admin@copacerrado.com.br
   - NomeCompleto: Administrador

3. **1 role Admin vinculada ao usuário**

4. **Índice único idx_cpf_etapa na tabela inscritos**

### ❌ Se algo estiver faltando:

Se alguma verificação falhar, execute o script de setup:

```bash
mysql -u root -p < Migrations/setup_identity_and_admin.sql
```

## 🔧 Passo 2: Verificar Configuração do Banco

### Verificar string de conexão em `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=motoclube_copaenduro;User=root;Password=SUA_SENHA_AQUI;"
  }
}
```

**⚠️ IMPORTANTE:** Substitua `SUA_SENHA_AQUI` pela senha real do MySQL.

### Testar conexão com o banco:

```bash
mysql -u root -p motoclube_copaenduro -e "SELECT 1;"
```

Se der erro, verifique:
- MySQL está rodando?
- Senha está correta?
- Banco `motoclube_copaenduro` existe?

## 🌐 Passo 3: Verificar se a Aplicação está Rodando

### Iniciar a aplicação:

```bash
dotnet run
```

### Ou através do Rider:
1. Clique no botão **Run** (▶️)
2. Ou pressione **Shift+F10**

### ✅ Você deve ver algo como:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

## 🔍 Passo 4: Verificar no Navegador

### 1. Abrir o Console do Navegador

- **Chrome/Edge:** Pressione `F12` ou `Ctrl+Shift+I`
- **Firefox:** Pressione `F12`

### 2. Acessar a página de login

```
http://localhost:5000/Admin/Login
```

### 3. Verificar os logs no Console

Você deve ver:
```
Login page loaded successfully
```

### 4. Clicar no botão "Entrar"

Sem preencher os campos, você deve ver:
```
Login button clicked
Form submitting...
Form validation failed
```

Com os campos preenchidos, você deve ver:
```
Login button clicked
Form submitting...
Form is valid, proceeding with submission
```

## ⚠️ Problemas Comuns e Soluções

### Problema 1: "Não consigo acessar http://localhost:5000"

**Causa:** Aplicação não está rodando

**Solução:**
```bash
cd /home/user/motoclube_copaenduro
dotnet run
```

### Problema 2: "Erro de conexão com banco de dados"

**Causa:** String de conexão incorreta ou MySQL não está rodando

**Solução:**
1. Verificar se MySQL está rodando:
   ```bash
   sudo systemctl status mysql
   # ou
   sudo service mysql status
   ```

2. Iniciar MySQL se necessário:
   ```bash
   sudo systemctl start mysql
   # ou
   sudo service mysql start
   ```

3. Verificar senha no `appsettings.json`

### Problema 3: "Email ou senha inválidos"

**Causa:** Usuário não foi criado ou senha hash incorreta

**Solução:**
1. Execute o diagnóstico:
   ```bash
   mysql -u root -p < Migrations/diagnostico.sql
   ```

2. Se o usuário não aparecer, execute:
   ```bash
   mysql -u root -p < Migrations/setup_identity_and_admin.sql
   ```

3. Tente fazer login novamente com:
   - Email: `admin@copacerrado.com.br`
   - Senha: `Admin@123`

### Problema 4: "Formulário não submete"

**Causa:** Erro de JavaScript ou validação

**Solução:**
1. Limpar cache do navegador: `Ctrl+Shift+Delete`
2. Fazer hard refresh: `Ctrl+F5`
3. Verificar console do navegador (F12) por erros JavaScript

### Problema 5: "Tabelas do Identity não existem"

**Causa:** Script não foi executado

**Solução:**
```bash
mysql -u root -p < Migrations/setup_identity_and_admin.sql
```

### Problema 6: "Error: SignInManager not registered"

**Causa:** Identity não está configurado no Program.cs

**Solução:** O arquivo Program.cs já está configurado corretamente. Reinicie a aplicação:
```bash
# Parar a aplicação (Ctrl+C)
# Iniciar novamente
dotnet run
```

## 🧪 Teste Completo Passo a Passo

1. **Parar a aplicação** (se estiver rodando): `Ctrl+C`

2. **Executar diagnóstico:**
   ```bash
   mysql -u root -p < Migrations/diagnostico.sql
   ```

3. **Se necessário, recriar usuário admin:**
   ```bash
   mysql -u root -p < Migrations/setup_identity_and_admin.sql
   ```

4. **Limpar e compilar projeto:**
   ```bash
   dotnet clean
   dotnet build
   ```

5. **Iniciar aplicação:**
   ```bash
   dotnet run
   ```

6. **Abrir navegador em modo anônimo** (Ctrl+Shift+N no Chrome)

7. **Acessar:** `http://localhost:5000/Admin/Login`

8. **Abrir console do navegador:** `F12`

9. **Preencher:**
   - Email: `admin@copacerrado.com.br`
   - Senha: `Admin@123`

10. **Clicar em "Entrar"**

11. **Verificar logs no console do navegador e terminal**

## 📞 Informações de Debug

Se o problema persistir, verifique:

### No Terminal da Aplicação:
- Há erros em vermelho?
- A aplicação iniciou corretamente?
- Qual porta está sendo usada?

### No Console do Navegador (F12 → Console):
- Há erros JavaScript?
- Os logs aparecem quando clica em "Entrar"?
- Algum arquivo CSS/JS não carregou (404)?

### No Console do Navegador (F12 → Network):
- Quando clica em "Entrar", uma requisição POST é feita para `/Admin/Login`?
- Qual é o status code da resposta? (200, 302, 400, 500?)
- Há algum erro de CORS?

## ✅ Checklist Final

- [ ] MySQL está rodando
- [ ] Banco `motoclube_copaenduro` existe
- [ ] Todas as tabelas Identity foram criadas
- [ ] Usuário admin existe e está vinculado à role Admin
- [ ] String de conexão em appsettings.json está correta
- [ ] Aplicação está rodando (dotnet run)
- [ ] Consegue acessar http://localhost:5000
- [ ] Console do navegador não mostra erros
- [ ] Cache do navegador foi limpo

---

**Se seguiu todos os passos e ainda não funciona, forneça:**
1. Output completo do script de diagnóstico
2. Erros do console do navegador (F12)
3. Erros do terminal da aplicação
4. Versão do .NET: `dotnet --version`
