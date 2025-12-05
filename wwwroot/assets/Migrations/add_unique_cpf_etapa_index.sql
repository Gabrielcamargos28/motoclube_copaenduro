-- Script para adicionar índice único CPF + Etapa em banco existente
-- Este script previne inscrições duplicadas do mesmo CPF na mesma etapa

USE motoclube_copaenduro;

-- Verificar se existem duplicatas antes de criar o índice
SELECT
    cpf,
    id_etapa,
    COUNT(*) as quantidade
FROM inscritos
GROUP BY cpf, id_etapa
HAVING COUNT(*) > 1;

-- Se a query acima retornar resultados, você terá que resolver as duplicatas manualmente
-- antes de executar o comando abaixo

-- Adicionar índice único composto (CPF + Etapa)
ALTER TABLE inscritos
ADD UNIQUE INDEX idx_cpf_etapa (cpf, id_etapa)
COMMENT 'Previne inscrição duplicada do mesmo CPF na mesma etapa';

SELECT 'Índice único CPF + Etapa criado com sucesso!' as Resultado;
