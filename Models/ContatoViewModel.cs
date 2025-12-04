using System.ComponentModel.DataAnnotations;

namespace MotoClubeCerrado.Models
{
    public class ContatoViewModel
    {
        [Required(ErrorMessage ="Campo Nome obrigatório")]
        [MinLength(3, ErrorMessage = "Nome deve ter no mínimo 3 caracteres")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Campo Email obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Campo Assunto obrigatório")]
        public string Assunto { get; set; }
        [Required(ErrorMessage = "Campo Telefone obrigatório")]
        public string Telefone { get; set; }
        [Required(ErrorMessage = "Campo Mensagem obrigatório")]
        public string Mensagem {  get; set; }

    }
}
