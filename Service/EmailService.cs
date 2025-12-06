namespace MotoClubeCerrado.Service;
using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> EnviarEmailAsync(string para, string assunto, string mensagemHtml)
    {
        try
        {
            string email = _config["Email:Usuario"];
            string senha = _config["Email:Senha"];


            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(email);
            mail.To.Add(para);
            mail.Subject = assunto;
            mail.Body = mensagemHtml;
            mail.IsBodyHtml = true;

            using (SmtpClient smtp = new SmtpClient("smtp.hostinger.com"))
            {
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(email, senha);
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(mail);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            return false;
        }
    }

}
