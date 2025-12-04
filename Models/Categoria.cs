using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoClubeCerrado.Models
{
    [Table("categorias")]
    public class Categoria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [MaxLength(500)]
        public string? Descricao { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        // Navigation property
        public virtual ICollection<Inscrito> Inscritos { get; set; } = new List<Inscrito>();
    }
}
