using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoClubeCerrado.Models
{
    [Table("eventos")]
    public class Evento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do evento é obrigatório")]
        [Column("nome")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Column("descricao")]
        [MaxLength(1000)]
        public string? Descricao { get; set; }

        [Column("ano")]
        public int Ano { get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        [Column("exibir_menu")]
        public bool ExibirMenu { get; set; } = true;

        [Column("ordem")]
        public int Ordem { get; set; } = 0;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<Etapa>? Etapas { get; set; }
    }
}
