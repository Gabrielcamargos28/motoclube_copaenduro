using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoClubeCerrado.Models
{
    [Table("etapas")]
    public class Etapa
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Column("data_evento")]
        public DateTime? DataEvento { get; set; }

        [Column("local")]
        [MaxLength(200)]
        public string? Local { get; set; }

        [Column("descricao")]
        [MaxLength(1000)]
        public string? Descricao { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("inscricoes_abertas")]
        public bool InscricoesAbertas { get; set; } = true;

        // Navigation property
        public virtual ICollection<Inscrito> Inscritos { get; set; } = new List<Inscrito>();
    }
}
