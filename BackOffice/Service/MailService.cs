using System;
using System.Net;
using System.Net.Mail;

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

            // Résoudre les enregistrements MX (Mail Exchange) du domaine
            var hostEntry = Dns.GetHostEntry(domain);

            // Si la résolution réussit, le domaine existe
            return hostEntry != null;
        }
        catch (Exception ex)
        {
            // Si une exception est levée (domaine introuvable), le domaine n'existe pas
            Console.WriteLine($"Erreur DNS lors de la validation du domaine : {ex.Message}");
            return false;
        }
    }

    public static bool SendEmail(string toEmail, string subject, string body)
    {
        // Vérifier la validité du domaine avant d'envoyer l'email
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
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(SmtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            client.Send(mailMessage);

            Console.WriteLine($"Email envoyé à {toEmail} avec succès.");
            return true;
        }
        catch (SmtpException smtpEx)
        {
            // Capture les exceptions SMTP spécifiques (erreurs de serveur)
            Console.WriteLine($"Erreur SMTP lors de l'envoi de l'email : {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            // Capture les autres exceptions génériques
            Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
        }

        return false;
    }
}
