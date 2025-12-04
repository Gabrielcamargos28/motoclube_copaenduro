using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoClubeCerrado.Models
{
    [Table("inscritos")]
    public class Inscrito
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [Column("nome")]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "CPF é obrigatório")]
        [Column("cpf")]
        [MaxLength(11)]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Column("telefone")]
        [MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cidade é obrigatória")]
        [Column("cidade")]
        [MaxLength(100)]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        [Column("data_nascimento")]
        public DateTime DataNascimento { get; set; }

        [Column("instagram")]
        [MaxLength(100)]
        public string? Instagram { get; set; }

        [Required(ErrorMessage = "UF é obrigatória")]
        [Column("uf")]
        [MaxLength(2)]
        public string Uf { get; set; } = string.Empty;

        [Column("patrocinador")]
        [MaxLength(500)]
        public string? Patrocinador { get; set; }

        [Required]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("id_mineiro")]
        public int IdMineiro { get; set; } = 0;

        [Column("valor")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Column("email")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("id_etapa")]
        public int IdEtapa { get; set; }

        [Column("pagamento")]
        public int Pagamento { get; set; } = 0; // 0 = Pendente, 1 = Confirmado

        [Column("visivel")]
        public int Visivel { get; set; } = 1;

        [Column("data_inscricao")]
        public DateTime DataInscricao { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("IdCategoria")]
        public virtual Categoria? Categoria { get; set; }

        [ForeignKey("IdEtapa")]
        public virtual Etapa? Etapa { get; set; }
    }
}
