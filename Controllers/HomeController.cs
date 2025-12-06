using Microsoft.AspNetCore.Mvc;
using MotoClubeCerrado.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using MotoClubeCerrado.Service;

namespace MotoClubeCerrado.Controllers
{
    public class HomeController : Controller
    {

        private readonly EmailService _emailService;
        public HomeController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Diretoria()
        {
            return View();
        }

        public IActionResult Editais()
        {
            return View();
        }

        public IActionResult Documentos()
        {
            return View();
        }

        public IActionResult Contato()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Organograma()
        {
            return View();
        }

        public IActionResult RelatorioAtividades()
        {
            return View();
        }

        public IActionResult Filiados()
        {
            return View();
        }

        public IActionResult ProjetosPublicos()
        {
            return View();
        }

        public IActionResult Atas()
        {
            return View();
        }

        public IActionResult Estatutos()
        {
            return View();
        }

        public IActionResult PrestacaoContas()
        {
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendContact(ContatoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["message"] = "Informações inválidas!";
                return View("Contato", model);
            }
            
            string body = "<p>Nome: " + model.Nome + "</p><p>E-mail: " + model.Email + "</p>" +
                      "<p>Telefone: " + model.Telefone + "</p><p> Assunto: " + 
                      model.Assunto + "</p><p> Mensagem: " + model.Mensagem + "</p>";
            
            Task<bool> retorno = _emailService.EnviarEmailAsync("viniciusgabrielpe@gmail.com", model.Assunto, body);
            
            if (retorno.Result)
            {
                ViewData["msgsucesso"] = "E-mail enviado com sucesso";

                return View("Contato");
            }
            else
            {
                ViewData["message"] = retorno;
                return View("Contato", model);

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
