using Microsoft.AspNetCore.Identity;

namespace MotoClubeCerrado.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? NomeCompleto { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
