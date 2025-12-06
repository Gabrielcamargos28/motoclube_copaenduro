using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoClubeCerrado.Data;
using MotoClubeCerrado.Models;
using MotoClubeCerrado.Service;

namespace MotoClubeCerrado.Controllers
{
    public class InscricaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public InscricaoController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Inscricao
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias.Where(c => c.Ativo).ToListAsync();
            var etapasDisponiveis = await _context.Etapas
                .Where(e => e.Ativo && e.InscricoesAbertas)
                .OrderByDescending(e => e.DataEvento)
                .ToListAsync();

            ViewBag.Categorias = categorias;
            ViewBag.EtapasDisponiveis = etapasDisponiveis;

            return View();
        }

        // POST: Inscricao/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar([FromForm] Inscrito inscrito)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, message = "Dados inválidos: " + string.Join(", ", errors) });
                }

                // Validar se a etapa existe e está com inscrições abertas
                var etapa = await _context.Etapas.FindAsync(inscrito.IdEtapa);

                if (etapa == null)
                {
                    return Json(new { success = false, message = "Etapa inválida." });
                }

                if (!etapa.InscricoesAbertas)
                {
                    return Json(new { success = false, message = "As inscrições para esta etapa estão fechadas." });
                }

                // Validar se já existe inscrição com o mesmo CPF nesta etapa
                var cpfJaInscrito = await _context.Inscritos
                    .AnyAsync(i => i.Cpf == inscrito.Cpf && i.IdEtapa == inscrito.IdEtapa);

                if (cpfJaInscrito)
                {
                    return Json(new { success = false, message = $"CPF {inscrito.Cpf} já possui inscrição nesta etapa." });
                }

                inscrito.DataInscricao = DateTime.Now;

                // Gerar número de inscrição único
                inscrito.NumeroInscricao = await GerarNumeroInscricaoUnico();

                // Gerar valor com centavos únicos para identificação
                var ultimoInscrito = await _context.Inscritos
                    .Where(i => i.IdEtapa == inscrito.IdEtapa)
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefaultAsync();

                decimal valorBase = inscrito.IdCategoria == 10 ? 180.00m : inscrito.IdCategoria == 15 ? 0.00m : 200.00m;

                if (ultimoInscrito != null)
                {
                    // Incrementa os centavos do último inscrito
                    decimal centavos = (ultimoInscrito.Valor - Math.Floor(ultimoInscrito.Valor)) * 100;
                    centavos += 0.01m;
                    if (centavos >= 1.00m) centavos = 0.01m;
                    inscrito.Valor = valorBase + (centavos / 100);
                }
                else
                {
                    inscrito.Valor = valorBase + 0.01m;
                }

                _context.Inscritos.Add(inscrito);
                await _context.SaveChangesAsync();
                
                // Envio de e-mail de confirmação
                
                string assunto = "Confirmação de Inscrição";
                string mensagem = $@"
                    <h2>Inscrição realizada com sucesso!</h2>
                    <p>Olá, {inscrito.Nome}!</p>
                    <p>Sua inscrição na etapa <strong>{etapa.Nome}</strong> foi realizada com sucesso.</p>
                    <p><strong>Número da inscrição:</strong> {inscrito.NumeroInscricao}</p>
                    <p><strong>Valor para pagamento:</strong> R$ {inscrito.Valor:F2}</p>
                ";

                await _emailService.EnviarEmailAsync(inscrito.Email, assunto, mensagem);

                return Json(new {
                    success = true,
                    message = $"Inscrição realizada com sucesso na {etapa.Nome}!\n\nNúmero da Inscrição: {inscrito.NumeroInscricao}\nValor para pagamento: R$ {inscrito.Valor:F2}",
                    numeroInscricao = inscrito.NumeroInscricao,
                    valor = inscrito.Valor
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao realizar inscrição: " + ex.Message });
            }
        }

        // GET: Inscricao/Inscritos
        public async Task<IActionResult> Inscritos()
        {
            var etapas = await _context.Etapas
                .Where(e => e.Ativo)
                .OrderByDescending(e => e.DataEvento)
                .ToListAsync();

            ViewBag.Etapas = etapas;

            return View();
        }

        // GET: API endpoint para DataTables
        [HttpGet]
        public async Task<IActionResult> PesquisaInscritos(int? idEtapa = null)
        {
            try
            {
                // Se idEtapa for 0, significa "Todas as etapas"
                bool mostrarTodas = (idEtapa == 0);

                // Se não informar etapa e não for "todas", pega a mais recente ativa
                if (idEtapa == null && !mostrarTodas)
                {
                    var etapaAtual = await _context.Etapas
                        .Where(e => e.Ativo)
                        .OrderByDescending(e => e.DataEvento)
                        .FirstOrDefaultAsync();

                    idEtapa = etapaAtual?.Id;
                }

                if (idEtapa == null && !mostrarTodas)
                {
                    return Json(new
                    {
                        draw = 0,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = new List<object>(),
                        message = "Nenhuma etapa ativa encontrada"
                    });
                }

                var query = _context.Inscritos
                    .Include(i => i.Categoria)
                    .Include(i => i.Etapa)
                    .Where(i => i.Visivel == 1);

                // Se não for "todas", filtra pela etapa específica
                if (!mostrarTodas && idEtapa.HasValue)
                {
                    query = query.Where(i => i.IdEtapa == idEtapa.Value);
                }

                var inscritos = await query
                    .OrderBy(i => i.Categoria!.Nome)
                    .ThenBy(i => i.Nome)
                    .Select(i => new
                    {
                        id = i.Id,
                        numeroInscricao = i.NumeroInscricao,
                        nome = i.Nome,
                        cpf = i.Cpf,
                        telefone = i.Telefone,
                        cidade = i.Cidade,
                        dataNascimento = i.DataNascimento,
                        instagram = i.Instagram,
                        uf = i.Uf,
                        patrocinador = i.Patrocinador,
                        idCategoria = i.IdCategoria,
                        idMineiro = i.IdMineiro,
                        valor = i.Valor,
                        email = i.Email,
                        idEtapa = i.IdEtapa,
                        pagamento = i.Pagamento,
                        visivel = i.Visivel,
                        dataInscricao = i.DataInscricao,
                        categoria = i.Categoria!.Nome,
                        etapa = i.Etapa!.Nome
                    })
                    .ToListAsync();

                return Json(new
                {
                    draw = 0,
                    recordsTotal = inscritos.Count,
                    recordsFiltered = inscritos.Count,
                    data = inscritos,
                    message = (string?)null
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    draw = 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    message = "Erro: " + ex.Message
                });
            }
        }

        // GET: Inscricao/Calendario
        public async Task<IActionResult> Calendario()
        {
            var eventos = await _context.Eventos
                .Where(e => e.Ativo)
                .Include(e => e.Etapas)
                .OrderByDescending(e => e.Ano)
                .ThenBy(e => e.Ordem)
                .ToListAsync();

            return View(eventos);
        }

        // GET: Inscricao/Consultar
        public IActionResult Consultar()
        {
            return View();
        }

        // POST: Inscricao/ConsultarInscricao
        [HttpPost]
        public async Task<IActionResult> ConsultarInscricao(string cpf)
        {
            try
            {
                // Limpar CPF (remover pontos e traços)
                cpf = cpf?.Replace(".", "").Replace("-", "").Trim() ?? "";

                if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                {
                    return Json(new { success = false, message = "CPF inválido. Digite apenas números." });
                }

                var inscricoes = await _context.Inscritos
                    .Include(i => i.Categoria)
                    .Include(i => i.Etapa)
                    .Where(i => i.Cpf == cpf)
                    .OrderByDescending(i => i.DataInscricao)
                    .Select(i => new
                    {
                        nome = i.Nome,
                        cpf = i.Cpf,
                        email = i.Email,
                        telefone = i.Telefone,
                        cidade = i.Cidade,
                        uf = i.Uf,
                        categoria = i.Categoria!.Nome,
                        etapa = i.Etapa!.Nome,
                        dataEtapa = i.Etapa!.DataEvento,
                        valor = i.Valor,
                        numeroInscricao = i.NumeroInscricao,
                        numeroPiloto = i.NumeroPiloto,
                        dataInscricao = i.DataInscricao,
                        pagamento = i.Pagamento,
                        statusInscricao = i.StatusInscricao
                    })
                    .ToListAsync();

                if (!inscricoes.Any())
                {
                    return Json(new { success = false, message = "Nenhuma inscrição encontrada para este CPF." });
                }

                return Json(new { success = true, inscricoes = inscricoes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao consultar inscrição: " + ex.Message });
            }
        }

        // Método auxiliar para gerar número de inscrição único
        private async Task<string> GerarNumeroInscricaoUnico()
        {
            string numeroInscricao;
            bool existe;
            int tentativas = 0;
            const int maxTentativas = 100;

            do
            {
                // Gera número no formato YYYY-NNNNNN (ano + 6 dígitos aleatórios)
                var ano = DateTime.Now.Year;
                var numeroAleatorio = Random.Shared.Next(100000, 999999);
                numeroInscricao = $"{ano}-{numeroAleatorio}";

                // Verifica se já existe no banco
                existe = await _context.Inscritos.AnyAsync(i => i.NumeroInscricao == numeroInscricao);

                tentativas++;
                if (tentativas >= maxTentativas)
                {
                    // Fallback: usa timestamp se não conseguir gerar número único
                    numeroInscricao = $"{ano}-{DateTimeOffset.Now.ToUnixTimeMilliseconds() % 1000000:D6}";
                    break;
                }
            }
            while (existe);

            return numeroInscricao;
        }
    }
}
