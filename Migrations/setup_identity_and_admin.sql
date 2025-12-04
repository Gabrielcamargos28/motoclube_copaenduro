-- Script completo para criar tabelas do ASP.NET Identity e usuário administrador
-- Executar este script no MySQL após criar o banco de dados

USE motoclube_copaenduro;

-- ==================================================
-- CRIAR TABELAS DO ASP.NET IDENTITY
-- ==================================================

-- Tabela de Usuários
CREATE TABLE IF NOT EXISTS AspNetUsers (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    UserName VARCHAR(256) NULL,
    NormalizedUserName VARCHAR(256) NULL,
    Email VARCHAR(256) NULL,
    NormalizedEmail VARCHAR(256) NULL,
    EmailConfirmed TINYINT(1) NOT NULL DEFAULT 0,
    PasswordHash LONGTEXT NULL,
    SecurityStamp LONGTEXT NULL,
    ConcurrencyStamp LONGTEXT NULL,
    PhoneNumber TEXT NULL,
    PhoneNumberConfirmed TINYINT(1) NOT NULL DEFAULT 0,
    TwoFactorEnabled TINYINT(1) NOT NULL DEFAULT 0,
    LockoutEnd DATETIME(6) NULL,
    LockoutEnabled TINYINT(1) NOT NULL DEFAULT 0,
    AccessFailedCount INT NOT NULL DEFAULT 0,
    -- Campos customizados
    NomeCompleto VARCHAR(200) NOT NULL,
    DataCriacao DATETIME(6) NOT NULL,
    UNIQUE INDEX UserNameIndex (NormalizedUserName),
    INDEX EmailIndex (NormalizedEmail)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabela de Roles (Funções)
CREATE TABLE IF NOT EXISTS AspNetRoles (
    Id VARCHAR(255) NOT NULL PRIMARY KEY,
    Name VARCHAR(256) NULL,
    NormalizedName VARCHAR(256) NULL,
    ConcurrencyStamp LONGTEXT NULL,
    UNIQUE INDEX RoleNameIndex (NormalizedName)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabela de relacionamento Usuários-Roles
CREATE TABLE IF NOT EXISTS AspNetUserRoles (
    UserId VARCHAR(255) NOT NULL,
    RoleId VARCHAR(255) NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_Roles FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
    INDEX IX_AspNetUserRoles_RoleId (RoleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabela de Claims de Usuários
CREATE TABLE IF NOT EXISTS AspNetUserClaims (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL,
    ClaimType LONGTEXT NULL,
    ClaimValue LONGTEXT NULL,
    CONSTRAINT FK_AspNetUserClaims_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    INDEX IX_AspNetUserClaims_UserId (UserId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabela de Claims de Roles
CREATE TABLE IF NOT EXISTS AspNetRoleClaims (
    Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    RoleId VARCHAR(255) NOT NULL,
    ClaimType LONGTEXT NULL,
    ClaimValue LONGTEXT NULL,
    CONSTRAINT FK_AspNetRoleClaims_Roles FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE,
    INDEX IX_AspNetRoleClaims_RoleId (RoleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabela de Logins externos
CREATE TABLE IF NOT EXISTS AspNetUserLogins (
    LoginProvider VARCHAR(255) NOT NULL,
    ProviderKey VARCHAR(255) NOT NULL,
    ProviderDisplayName LONGTEXT NULL,
    UserId VARCHAR(255) NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    INDEX IX_AspNetUserLogins_UserId (UserId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Tabela de Tokens de Usuários
CREATE TABLE IF NOT EXISTS AspNetUserTokens (
    UserId VARCHAR(255) NOT NULL,
    LoginProvider VARCHAR(255) NOT NULL,
    Name VARCHAR(255) NOT NULL,
    Value LONGTEXT NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    CONSTRAINT FK_AspNetUserTokens_Users FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ==================================================
-- CRIAR USUÁRIO ADMINISTRADOR PADRÃO
-- ==================================================

-- Limpar dados anteriores (se existirem)
DELETE FROM AspNetUserRoles WHERE UserId = 'admin-user-id-001';
DELETE FROM AspNetUsers WHERE Id = 'admin-user-id-001';
DELETE FROM AspNetRoles WHERE Id = 'admin-role-id-001';

-- Criar role Admin
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES (
    'admin-role-id-001',
    'Admin',
    'ADMIN',
    UUID()
);

-- Criar usuário admin
-- Email: admin@copacerrado.com.br
-- Senha: Admin@123
INSERT INTO AspNetUsers (
    Id,
    UserName,
    NormalizedUserName,
    Email,
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnd,
    LockoutEnabled,
    AccessFailedCount,
    NomeCompleto,
    DataCriacao
)
VALUES (
    'admin-user-id-001',
    'admin@copacerrado.com.br',
    'ADMIN@COPACERRADO.COM.BR',
    'admin@copacerrado.com.br',
    'ADMIN@COPACERRADO.COM.BR',
    1,
    'AQAAAAIAAYagAAAAEHxH5vX8ZQqN9tKJ7xKW9YqF0pMXVzJZOvGJxF6qK0gM9vL8hVN5WJqR3tK2pL4mXw==',
    UPPER(CONCAT(
        SUBSTRING(MD5(RAND()), 1, 8), '-',
        SUBSTRING(MD5(RAND()), 1, 4), '-',
        SUBSTRING(MD5(RAND()), 1, 4), '-',
        SUBSTRING(MD5(RAND()), 1, 4), '-',
        SUBSTRING(MD5(RAND()), 1, 12)
    )),
    UUID(),
    NULL,
    0,
    0,
    NULL,
    1,
    0,
    'Administrador',
    NOW()
);

-- Associar usuário admin ao role Admin
INSERT INTO AspNetUserRoles (UserId, RoleId)
VALUES (
    'admin-user-id-001',
    'admin-role-id-001'
);

-- ==================================================
-- MENSAGENS DE CONFIRMAÇÃO
-- ==================================================

SELECT '===================================================' as '';
SELECT 'TABELAS DO ASP.NET IDENTITY CRIADAS COM SUCESSO!' as '';
SELECT '===================================================' as '';
SELECT '' as '';
SELECT 'Usuário administrador criado:' as '';
SELECT 'Email: admin@copacerrado.com.br' as '';
SELECT 'Senha: Admin@123' as '';
SELECT '' as '';
SELECT 'Acesse: http://localhost:5000/Admin/Login' as '';
SELECT '===================================================' as '';
