using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoClubeCerrado.Data;
using MotoClubeCerrado.Models;

namespace MotoClubeCerrado.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: Admin/Login
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Dashboard));
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Conta bloqueada temporariamente. Tente novamente mais tarde.");
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email ou senha inválidos.");
                    return View(model);
                }
            }

            return View(model);
        }

        // POST: Admin/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Admin/Dashboard
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var totalInscritos = await _context.Inscritos.CountAsync();
            var inscritosPagos = await _context.Inscritos.Where(i => i.Pagamento == 1).CountAsync();
            var etapasAtivas = await _context.Etapas.Where(e => e.Ativo).CountAsync();

            ViewBag.TotalInscritos = totalInscritos;
            ViewBag.InscritosPagos = inscritosPagos;
            ViewBag.InscritosPendentes = totalInscritos - inscritosPagos;
            ViewBag.EtapasAtivas = etapasAtivas;

            return View();
        }

        // GET: Admin/AccessDenied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Admin/Etapas
        [Authorize]
        public async Task<IActionResult>Etapas()
        {
            var etapas = await _context.Etapas
                .OrderByDescending(e => e.DataEvento)
                .ToListAsync();
            return View(etapas);
        }

        // GET: Admin/CreateEtapa
        [Authorize]
        public IActionResult CreateEtapa()
        {
            return View();
        }

        // POST: Admin/CreateEtapa
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEtapa(Etapa etapa)
        {
            if (ModelState.IsValid)
            {
                _context.Etapas.Add(etapa);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Etapa criada com sucesso!";
                return RedirectToAction(nameof(Etapas));
            }
            return View(etapa);
        }

        // GET: Admin/EditEtapa/5
        [Authorize]
        public async Task<IActionResult> EditEtapa(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa == null)
            {
                return NotFound();
            }
            return View(etapa);
        }

        // POST: Admin/EditEtapa/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEtapa(int id, Etapa etapa)
        {
            if (id != etapa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(etapa);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Etapa atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Etapas.Any(e => e.Id == etapa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Etapas));
            }
            return View(etapa);
        }

        // POST: Admin/DeleteEtapa/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEtapa(int id)
        {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa != null)
            {
                _context.Etapas.Remove(etapa);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Etapa excluída com sucesso!";
            }
            return RedirectToAction(nameof(Etapas));
        }

        // GET: Admin/InscritosEtapa/5
        [Authorize]
        public async Task<IActionResult> InscritosEtapa(int id)
        {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa == null)
            {
                return NotFound();
            }

            var inscritos = await _context.Inscritos
                .Include(i => i.Categoria)
                .Where(i => i.IdEtapa == id)
                .OrderBy(i => i.Categoria!.Nome)
                .ThenBy(i => i.Nome)
                .ToListAsync();

            ViewBag.Etapa = etapa;
            return View(inscritos);
        }

        // POST: Admin/TogglePagamento/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePagamento(int id)
        {
            var inscrito = await _context.Inscritos.FindAsync(id);
            if (inscrito != null)
            {
                inscrito.Pagamento = inscrito.Pagamento == 1 ? 0 : 1;
                await _context.SaveChangesAsync();
                return Json(new { success = true, pagamento = inscrito.Pagamento });
            }
            return Json(new { success = false });
        }
    }
}
