using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailServiceConsumer.Email
{
    public class EmailService
    {
        public void EnviarEmail(string nomeRemetente, string emailDestinatario, string assunto, string conteudo)
        {
            var porta = 587;
            var smtp = "smtp.titan.email";
            var isSSL = false;
            var usuario = "aula@varsolutions.com.br";
            var senha = "aulaemail@01";

            var objEmail = new MailMessage(usuario, emailDestinatario, assunto, conteudo);
            objEmail.From = new MailAddress(nomeRemetente + "<" + usuario + ">");
            objEmail.IsBodyHtml = true;
            objEmail.SubjectEncoding = Encoding.GetEncoding("UTF-8");
            objEmail.BodyEncoding = Encoding.GetEncoding("UTF-8");
            objEmail.Subject = assunto;
            objEmail.Body = conteudo;

            using (var objSmtp = new SmtpClient(smtp, porta))
            {
                objSmtp.EnableSsl = isSSL;
                objSmtp.UseDefaultCredentials = false;
                objSmtp.Credentials = new NetworkCredential(usuario, senha);

                objSmtp.Send(objEmail);
                objEmail.Dispose();
            }
        }
    }
}
