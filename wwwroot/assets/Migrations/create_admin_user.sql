-- Script para criar usuário administrador padrão
-- Executar APÓS rodar as migrations do Identity

USE motoclube_copaenduro;

-- Usuário: admin@copacer rado.com.br
-- Senha: Admin@123

-- Inserir usuário admin (hash da senha Admin@123)
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, NomeCompleto, DataCriacao)
VALUES (
    'admin-user-id-001',
    'admin@copacerrado.com.br',
    'ADMIN@COPACERRADO.COM.BR',
    'admin@copacerrado.com.br',
    'ADMIN@COPACERRADO.COM.BR',
    1,
    'AQAAAAIAAYagAAAAEHxH5vX8ZQqN9tKJ7xKW9YqF0pMXVzJZOvGJxF6qK0gM9vL8hVN5WJqR3tK2pL4mXw==', -- Hash para Admin@123
    CONCAT(SUBSTRING(MD5(RAND()), 1, 32)),
    CONCAT(UUID()),
    0,
    0,
    1,
    0,
    'Administrador',
    NOW()
)
ON DUPLICATE KEY UPDATE Email = Email;

-- Criar role Admin se não existir
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    'admin-role-id-001',
    'Admin',
    'ADMIN',
    CONCAT(UUID())
)
ON DUPLICATE KEY UPDATE Name = Name;

-- Associar usuário admin ao role Admin
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (
    'admin-user-id-001',
    'admin-role-id-001'
)
ON DUPLICATE KEY UPDATE UserId = UserId;

SELECT 'Usuário admin criado com sucesso!' as Resultado;
SELECT 'Email: admin@copacerrado.com.br' as Credenciais;
SELECT 'Senha: Admin@123' as Senha;
