-- Adicionar coluna numero_inscricao na tabela inscritos
-- Data: 2025-12-05

-- Adicionar a coluna numero_inscricao
ALTER TABLE inscritos
ADD COLUMN numero_inscricao VARCHAR(20) NULL
AFTER id;

-- Adicionar índice único para garantir que não haja números duplicados
ALTER TABLE inscritos
ADD UNIQUE INDEX idx_numero_inscricao (numero_inscricao);

-- Gerar números de inscrição únicos para registros existentes
-- O número terá o formato: YYYY-NNNNNN (ano + 6 dígitos aleatórios)
-- Exemplo: 2025-123456

SET @counter = 100000;

UPDATE inscritos
SET numero_inscricao = CONCAT(
    YEAR(data_inscricao),
    '-',
    LPAD(@counter := @counter + 1, 6, '0')
)
WHERE numero_inscricao IS NULL
ORDER BY id;
