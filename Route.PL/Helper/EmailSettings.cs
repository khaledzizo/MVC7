using Route.DAL.Entities;
using System.Net;
using System.Net.Mail;

namespace Route.PL.Helper
{
    public class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            //client.EnableSsl = true;
            client.Credentials = new NetworkCredential("mmohamedfawzi23@gmail.com", "zpzguhnqzaufmwmm");

            client.Send("mmohamedfawzi23@gmail.com", email.To, email.Title, email.Body);
        }
    }
}
