# Área Administrativa - Copa Cerrado de Enduro

Sistema de autenticação e gerenciamento administrativo para a Copa Cerrado de Enduro.

## 🔐 Acesso à Área Administrativa

### URL de Acesso
```
https://localhost:5001/Admin/Login
```

### Credenciais Padrão
Após executar o script `create_admin_user.sql`:
- **Email**: `admin@copacerrado.com.br`
- **Senha**: `Admin@123`

⚠️ **IMPORTANTE**: Altere a senha padrão após o primeiro login!

## 🚀 Configuração Inicial

### 1. Executar Migrations do Identity

No terminal do Rider, execute:

```bash
# Criar migration do Identity
dotnet ef migrations add AddIdentityTables

# Aplicar no banco de dados
dotnet ef database update
```

### 2. Criar Usuário Administrador

Execute o script SQL:
```bash
mysql -u root -p motoclube_copaenduro < Migrations/create_admin_user.sql
```

Ou execute manualmente no MySQL Workbench/phpMyAdmin o conteúdo do arquivo `Migrations/create_admin_user.sql`.

### 3. Testar o Login

1. Acesse: `https://localhost:5001/Admin/Login`
2. Entre com as credenciais padrão
3. Você será redirecionado para o Dashboard

## 📋 Funcionalidades da Área Admin

### Dashboard
- Estatísticas em tempo real:
  - Total de inscritos
  - Pagamentos confirmados
  - Pagamentos pendentes
  - Etapas ativas
- Ações rápidas para navegação

### Gerenciamento de Etapas
- **Listar Etapas**: Ver todas as etapas cadastradas
- **Criar Etapa**: Adicionar novas etapas
- **Editar Etapa**: Modificar informações
- **Excluir Etapa**: Remover etapas (com confirmação)
- **Controlar Inscrições**: Abrir/fechar inscrições por etapa
- **Ativar/Desativar**: Controlar visibilidade de etapas

### Gerenciamento de Inscritos
- **Ver Inscritos por Etapa**: Lista completa de inscritos
- **Confirmar Pagamento**: Alterar status de pagamento com um clique
- **Filtros e Busca**: Encontrar inscritos rapidamente
- **Exportar Dados**: (Funcionalidade futura)

## 🔒 Segurança

### Políticas de Senha
- Mínimo: 6 caracteres
- Deve conter: letra maiúscula, minúscula e número
- Não requer caracteres especiais (configurável)

### Proteção de Rotas
Todas as rotas administrativas estão protegidas com `[Authorize]`:
- `/Admin/Dashboard`
- `/Admin/Etapas`
- `/Admin/CreateEtapa`
- `/Admin/EditEtapa`
- `/Admin/InscritosEtapa`

### Sessão
- Duração: 24 horas
- Sliding expiration: habilitado
- Lockout após 5 tentativas falhadas

## 📁 Estrutura de Arquivos

```
Controllers/
├── AdminController.cs          # Controller principal

Models/
├── ApplicationUser.cs          # Modelo de usuário estendido
└── LoginViewModel.cs           # ViewModel para login

Views/Admin/
├── _AdminLayout.cshtml         # Layout administrativo
├── Login.cshtml                # Tela de login
├── Dashboard.cshtml            # Dashboard principal
├── Etapas.cshtml              # Lista de etapas
├── CreateEtapa.cshtml         # Criar etapa
├── EditEtapa.cshtml           # Editar etapa
└── InscritosEtapa.cshtml      # Inscritos por etapa
```

## 🎨 Interface Administrativa

### Design
- Layout moderno com gradiente roxo
- Cards com estatísticas
- Ícones Font Awesome
- Responsivo para mobile
- Alertas de sucesso/erro com TempData

### Cores
- Primary: Gradiente #667eea → #764ba2
- Success: Verde Bootstrap
- Warning: Amarelo Bootstrap
- Danger: Vermelho Bootstrap

## 🔧 Customização

### Adicionar Novo Administrador

Via código C# (criar um serviço ou endpoint):

```csharp
var user = new ApplicationUser
{
    UserName = "novoadmin@copacerrado.com.br",
    Email = "novoadmin@copacerrado.com.br",
    NomeCompleto = "Novo Administrador",
    EmailConfirmed = true
};

await _userManager.CreateAsync(user, "SenhaSegura@123");
await _userManager.AddToRoleAsync(user, "Admin");
```

### Modificar Políticas de Senha

Edite em `Program.cs`:

```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireUppercase = true;
options.Password.RequireNonAlphanumeric = false; // Altere aqui
options.Password.RequiredLength = 6; // Altere aqui
```

## 🐛 Troubleshooting

### "Invalid login attempt"
- Verifique se executou o script SQL de criação do admin
- Confirme que o banco tem as tabelas do Identity
- Tente resetar a senha via código

### Erro 404 ao acessar /Admin/Login
- Verifique se o Identity está configurado no `Program.cs`
- Confirme que `app.UseAuthentication()` está antes de `app.UseAuthorization()`

### Não consigo fazer logout
- Limpe os cookies do navegador
- Verifique o token anti-forgery no formulário de logout

## 📊 Tabelas do Identity

O Identity cria automaticamente estas tabelas:
- `AspNetUsers` - Usuários
- `AspNetRoles` - Roles/Perfis
- `AspNetUserRoles` - Relação usuário-role
- `AspNetUserClaims` - Claims dos usuários
- `AspNetUserLogins` - Logins externos
- `AspNetUserTokens` - Tokens de segurança
- `AspNetRoleClaims` - Claims das roles

## 🎯 Próximas Melhorias

- [ ] Recuperação de senha por email
- [ ] Registro de novo administrador
- [ ] Log de ações administrativas
- [ ] Exportar lista de inscritos para Excel
- [ ] Dashboard com gráficos
- [ ] Gerenciamento de categorias
- [ ] Envio de emails em massa para inscritos

---

**Desenvolvido para Copa Cerrado de Enduro** 🏍️
