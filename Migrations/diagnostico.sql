-- Script de diagnóstico do banco de dados
-- Execute este script para verificar se tudo está configurado corretamente

USE motoclube_copaenduro;

-- ==================================================
-- VERIFICAÇÃO 1: Verificar se as tabelas existem
-- ==================================================
SELECT 'VERIFICAÇÃO 1: Tabelas existentes' as Teste;
SELECT TABLE_NAME as Tabela
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'motoclube_copaenduro'
ORDER BY TABLE_NAME;

-- ==================================================
-- VERIFICAÇÃO 2: Verificar estrutura da tabela AspNetUsers
-- ==================================================
SELECT 'VERIFICAÇÃO 2: Estrutura AspNetUsers' as Teste;
DESCRIBE AspNetUsers;

-- ==================================================
-- VERIFICAÇÃO 3: Verificar se o usuário admin existe
-- ==================================================
SELECT 'VERIFICAÇÃO 3: Usuário Admin' as Teste;
SELECT
    Id,
    UserName,
    Email,
    EmailConfirmed,
    NomeCompleto,
    DataCriacao
FROM AspNetUsers
WHERE Email = 'admin@copacerrado.com.br';

-- ==================================================
-- VERIFICAÇÃO 4: Verificar se a role Admin existe
-- ==================================================
SELECT 'VERIFICAÇÃO 4: Role Admin' as Teste;
SELECT
    Id,
    Name,
    NormalizedName
FROM AspNetRoles
WHERE NormalizedName = 'ADMIN';

-- ==================================================
-- VERIFICAÇÃO 5: Verificar se o usuário está associado à role
-- ==================================================
SELECT 'VERIFICAÇÃO 5: Usuário vinculado à Role' as Teste;
SELECT
    u.Email,
    u.NomeCompleto,
    r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'admin@copacerrado.com.br';

-- ==================================================
-- VERIFICAÇÃO 6: Verificar etapas
-- ==================================================
SELECT 'VERIFICAÇÃO 6: Etapas cadastradas' as Teste;
SELECT
    id,
    nome,
    data_evento,
    local,
    ativo,
    inscricoes_abertas
FROM etapas;

-- ==================================================
-- VERIFICAÇÃO 7: Verificar categorias
-- ==================================================
SELECT 'VERIFICAÇÃO 7: Categorias cadastradas' as Teste;
SELECT
    id,
    nome,
    ativo
FROM categorias
ORDER BY id;

-- ==================================================
-- VERIFICAÇÃO 8: Verificar índice único CPF+Etapa
-- ==================================================
SELECT 'VERIFICAÇÃO 8: Índice único CPF+Etapa' as Teste;
SHOW INDEX FROM inscritos WHERE Key_name = 'idx_cpf_etapa';

-- ==================================================
-- VERIFICAÇÃO 9: Verificar inscritos (se houver)
-- ==================================================
SELECT 'VERIFICAÇÃO 9: Total de inscritos' as Teste;
SELECT
    COUNT(*) as total_inscritos,
    SUM(CASE WHEN pagamento = 1 THEN 1 ELSE 0 END) as pagos,
    SUM(CASE WHEN pagamento = 0 THEN 1 ELSE 0 END) as pendentes
FROM inscritos;

-- ==================================================
-- VERIFICAÇÃO 10: Verificar possíveis duplicatas de CPF
-- ==================================================
SELECT 'VERIFICAÇÃO 10: Verificar duplicatas de CPF na mesma etapa' as Teste;
SELECT
    cpf,
    id_etapa,
    COUNT(*) as quantidade
FROM inscritos
GROUP BY cpf, id_etapa
HAVING COUNT(*) > 1;

-- ==================================================
-- RESULTADO ESPERADO
-- ==================================================
SELECT '===================================================' as '';
SELECT 'RESULTADOS ESPERADOS:' as '';
SELECT '===================================================' as '';
SELECT '1. Deve haver pelo menos 7 tabelas do Identity' as '';
SELECT '2. Tabela AspNetUsers deve ter campo NomeCompleto' as '';
SELECT '3. Deve existir 1 usuário: admin@copacerrado.com.br' as '';
SELECT '4. Deve existir 1 role: Admin' as '';
SELECT '5. Usuário admin deve estar vinculado à role Admin' as '';
SELECT '6. Deve haver pelo menos 1 etapa cadastrada' as '';
SELECT '7. Devem haver 11 categorias cadastradas' as '';
SELECT '8. Deve existir índice idx_cpf_etapa na tabela inscritos' as '';
SELECT '9. Pode haver 0 ou mais inscritos' as '';
SELECT '10. NÃO deve haver duplicatas (resultado vazio)' as '';
SELECT '===================================================' as '';
