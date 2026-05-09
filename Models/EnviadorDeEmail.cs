using System.Net.Mail;
using System.Net;

namespace AutomacaoEmail.Models
{
    public class EnviadorDeEmail
    {
        public string Provedor { get; private set;}
        public string User{get; private set;}
        public string Password{get; private set;}
        public EnviadorDeEmail(string provedor, string user, string password)
        {
            this.Provedor = provedor;
            this.User = user;
            this.Password = password;
        }

        public void EnviarEmail(List<string> emails, string assunto, string corpo)
        {
            var msg = MessageProp(emails, assunto, corpo);

            SmtpClient smt = new SmtpClient();
            smt.Host = Provedor;
            smt.Port = 587; //Porta comum para SMTP
            smt.EnableSsl = true;
            smt. Timeout = 35000;
            smt.UseDefaultCredentials = false;
            smt.Credentials = new NetworkCredential(User, Password);
            smt.Send(msg);
            smt.Dispose();


        }

        private MailMessage MessageProp(List<string> emails, string assunto, string corpo)
        {
            var mail = new MailMessage();
            mail.From  = new MailAddress(User);

            foreach(var item in emails)
            {
                mail.To.Add(new MailAddress(item));
            }
            mail.Subject = assunto;
            mail.Body = corpo;
            mail.IsBodyHtml = true;
            return mail;
        }
    }
    
}