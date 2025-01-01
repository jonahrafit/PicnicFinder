using System;
using System.Net;
using System.Net.Mail;

namespace AdminBO.Services;

public class EmailService
{
    private const string SmtpServer = "smtp.gmail.com"; // Serveur SMTP de Gmail
    private const int SmtpPort = 587; // Port SMTP pour STARTTLS
    private const string SmtpUser = "jonahrafit@gmail.com"; // Votre adresse Gmail
    private const string SmtpPassword = "sfad gduq rwde zbxt"; // Mot de passe d'application Gmail

    public static bool IsDomainValid(string email)
    {
        try
        {
            // Extraire le domaine de l'adresse email
            var domain = email.Split('@')[1];

            // R�soudre les enregistrements MX (Mail Exchange) du domaine
            var hostEntry = Dns.GetHostEntry(domain);

            // Si la r�solution r�ussit, le domaine existe
            return hostEntry != null;
        }
        catch (Exception ex)
        {
            // Si une exception est lev�e (domaine introuvable), le domaine n'existe pas
            Console.WriteLine($"Erreur DNS lors de la validation du domaine : {ex.Message}");
            return false;
        }
    }

    public static bool SendEmail(string toEmail, string subject, string body)
    {
        // V�rifier la validit� du domaine avant d'envoyer l'email
        if (!IsDomainValid(toEmail))
        {
            Console.WriteLine($"Le domaine du mail {toEmail} est introuvable.");
            return false;
        }

        try
        {
            using var client = new SmtpClient(SmtpServer, SmtpPort)
            {
                Credentials = new NetworkCredential(SmtpUser, SmtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(SmtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);
            client.Send(mailMessage);

            Console.WriteLine($"Email envoy� � {toEmail} avec succ�s.");
            return true;
        }
        catch (SmtpException smtpEx)
        {
            // Capture les exceptions SMTP sp�cifiques (erreurs de serveur)
            Console.WriteLine($"Erreur SMTP lors de l'envoi de l'email : {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            // Capture les autres exceptions g�n�riques
            Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
        }

        return false;
    }
}
