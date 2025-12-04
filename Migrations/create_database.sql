-- Script SQL para criar o banco de dados e tabelas manualmente
-- Execute este script caso prefira criar o banco manualmente ao invés de usar migrations

CREATE DATABASE IF NOT EXISTS motoclube_copaenduro CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE motoclube_copaenduro;

-- Tabela de Categorias
CREATE TABLE IF NOT EXISTS categorias (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(500) NULL,
    ativo TINYINT(1) NOT NULL DEFAULT 1,
    INDEX idx_ativo (ativo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabela de Etapas
CREATE TABLE IF NOT EXISTS etapas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(200) NOT NULL,
    data_evento DATETIME NULL,
    local VARCHAR(200) NULL,
    descricao VARCHAR(1000) NULL,
    ativo TINYINT(1) NOT NULL DEFAULT 1,
    inscricoes_abertas TINYINT(1) NOT NULL DEFAULT 1,
    INDEX idx_ativo (ativo),
    INDEX idx_inscricoes_abertas (inscricoes_abertas)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Tabela de Inscritos
CREATE TABLE IF NOT EXISTS inscritos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(200) NOT NULL,
    cpf VARCHAR(11) NOT NULL,
    telefone VARCHAR(20) NOT NULL,
    cidade VARCHAR(100) NOT NULL,
    data_nascimento DATETIME NOT NULL,
    instagram VARCHAR(100) NULL,
    uf VARCHAR(2) NOT NULL,
    patrocinador VARCHAR(500) NULL,
    id_categoria INT NOT NULL,
    id_mineiro INT NOT NULL DEFAULT 0,
    valor DECIMAL(10,2) NOT NULL DEFAULT 0,
    email VARCHAR(200) NOT NULL,
    id_etapa INT NOT NULL,
    pagamento INT NOT NULL DEFAULT 0 COMMENT '0=Pendente, 1=Confirmado',
    visivel INT NOT NULL DEFAULT 1,
    data_inscricao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_id_categoria (id_categoria),
    INDEX idx_id_etapa (id_etapa),
    INDEX idx_cpf (cpf),
    INDEX idx_visivel (visivel),
    INDEX idx_pagamento (pagamento),
    FOREIGN KEY (id_categoria) REFERENCES categorias(id) ON DELETE RESTRICT,
    FOREIGN KEY (id_etapa) REFERENCES etapas(id) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Inserir categorias padrão
INSERT INTO categorias (id, nome, descricao, ativo) VALUES
(1, 'Elite', 'Categoria Elite', 1),
(2, 'Importada Pró', 'Categoria Importada Pró', 1),
(3, 'Nacional Pró', 'Categoria Nacional Pró', 1),
(4, 'Over 35', 'Categoria Over 35', 1),
(5, 'Over 40', 'Categoria Over 40', 1),
(6, 'Over 45', 'Categoria Over 45', 1),
(7, 'Importada Estreante', 'Categoria Importada Estreante', 1),
(8, 'Nacional Estreante', 'Categoria Nacional Estreante', 1),
(9, 'Over 50', 'Categoria Over 50', 1),
(10, 'Local', 'Categoria Local', 1),
(15, 'Feminina', 'Categoria Feminina', 1)
ON DUPLICATE KEY UPDATE nome=VALUES(nome);

-- Inserir etapa atual
INSERT INTO etapas (id, nome, data_evento, local, descricao, ativo, inscricoes_abertas) VALUES
(16, '6ª Etapa Copa Cerrado de Enduro', '2025-12-15 08:00:00', 'Patrocínio - MG', 'Sexta etapa da Copa Cerrado de Enduro', 1, 1)
ON DUPLICATE KEY UPDATE nome=VALUES(nome);
