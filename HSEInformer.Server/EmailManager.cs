using HSEInformer.Server.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server
{
    public class EmailManager : IEmailManager
    {
        public async Task<bool> SendConfirmationEmailAsync(string email, string confirmation_code)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("HSE Informer", "cepbuch@outlook.com"));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = "HSEInformer : Подтверждение действий";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"Ваш код: {confirmation_code}"
            };

            try
            {
                using (var client = new SmtpClient())
                {

                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync("hseinformerMail@gmail.com", "hseinformerApp");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);

                    return true;
                }
            }
            catch (Exception ex) { return false; }
        }
    }
}
