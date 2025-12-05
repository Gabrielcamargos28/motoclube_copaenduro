-- Criar tabela de eventos
-- Data: 2025-12-05

-- Criar tabela eventos
CREATE TABLE IF NOT EXISTS eventos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(200) NOT NULL,
    descricao VARCHAR(1000) NULL,
    ano INT NOT NULL,
    ativo TINYINT(1) NOT NULL DEFAULT 1,
    exibir_menu TINYINT(1) NOT NULL DEFAULT 1,
    ordem INT NOT NULL DEFAULT 0,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_ativo (ativo),
    INDEX idx_exibir_menu (exibir_menu),
    INDEX idx_ano (ano)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Adicionar coluna id_evento na tabela etapas
ALTER TABLE etapas
ADD COLUMN id_evento INT NULL AFTER id,
ADD INDEX idx_id_evento (id_evento),
ADD CONSTRAINT fk_etapas_eventos
    FOREIGN KEY (id_evento) REFERENCES eventos(id)
    ON DELETE SET NULL
    ON UPDATE CASCADE;

-- Inserir evento padrão "Copa Cerrado de Enduro 2025"
INSERT INTO eventos (nome, descricao, ano, ativo, exibir_menu, ordem)
VALUES
('Copa Cerrado de Enduro 2025', 'Campeonato de Enduro FIM da região do Cerrado', 2025, 1, 1, 1);

-- Associar todas as etapas existentes ao evento padrão
UPDATE etapas
SET id_evento = (SELECT id FROM eventos WHERE ano = 2025 LIMIT 1)
WHERE id_evento IS NULL;
