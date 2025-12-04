using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoClubeCerrado.Data;
using MotoClubeCerrado.Models;

namespace MotoClubeCerrado.Controllers
{
    public class InscricaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InscricaoController(ApplicationDbContext context)
        {
            _context = context;
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

                return Json(new {
                    success = true,
                    message = $"Inscrição realizada com sucesso na {etapa.Nome}! Valor para pagamento: R$ {inscrito.Valor:F2}",
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
                // Se não informar etapa, pega a mais recente ativa
                if (idEtapa == null)
                {
                    var etapaAtual = await _context.Etapas
                        .Where(e => e.Ativo)
                        .OrderByDescending(e => e.DataEvento)
                        .FirstOrDefaultAsync();

                    idEtapa = etapaAtual?.Id;
                }

                if (idEtapa == null)
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

                var inscritos = await _context.Inscritos
                    .Include(i => i.Categoria)
                    .Include(i => i.Etapa)
                    .Where(i => i.IdEtapa == idEtapa && i.Visivel == 1)
                    .OrderBy(i => i.Categoria!.Nome)
                    .ThenBy(i => i.Nome)
                    .Select(i => new
                    {
                        id = i.Id,
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
            var etapas = await _context.Etapas
                .Where(e => e.Ativo)
                .OrderBy(e => e.DataEvento)
                .ToListAsync();

            return View(etapas);
        }
    }
}
