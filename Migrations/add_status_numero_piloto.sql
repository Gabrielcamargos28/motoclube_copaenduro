-- Script para adicionar campos status_inscricao e numero_piloto na tabela inscritos
-- Execute este script no MySQL

USE motoclube_copaenduro;

-- Adicionar coluna status_inscricao
ALTER TABLE inscritos
ADD COLUMN status_inscricao INT NOT NULL DEFAULT 0 COMMENT '0=Pendente, 1=Aprovada, 2=Reprovada'
AFTER pagamento;

-- Adicionar coluna numero_piloto
ALTER TABLE inscritos
ADD COLUMN numero_piloto VARCHAR(3) NULL
AFTER status_inscricao;

-- Adicionar índice para facilitar buscas por status
ALTER TABLE inscritos
ADD INDEX idx_status_inscricao (status_inscricao);

SELECT 'Colunas status_inscricao e numero_piloto adicionadas com sucesso!' as Resultado;

-- Verificar estrutura atualizada
DESCRIBE inscritos;
