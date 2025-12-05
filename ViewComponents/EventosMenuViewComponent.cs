using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MotoClubeCerrado.Data;

namespace MotoClubeCerrado.ViewComponents
{
    public class EventosMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public EventosMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var eventos = await _context.Eventos
                .Where(e => e.Ativo && e.ExibirMenu)
                .OrderBy(e => e.Ordem)
                .ThenByDescending(e => e.Ano)
                .Include(e => e.Etapas)
                .ToListAsync();

            return View(eventos);
        }
    }
}
