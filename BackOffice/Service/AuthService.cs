using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PicnicFinder.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly PicnicFinderContext _dbContext;

    public AuthService(IConfiguration configuration, PicnicFinderContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La clé secrète JWT n'est pas configurée.");
        }

        // Récupération de l'utilisateur depuis la base de données
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null; // Mauvais identifiants
        }

        // Création du rôle
        var role = user.Role.ToString();
        var userId = user.Id.ToString();
        Console.WriteLine($"ROLE TO STRING {role}");

        // Création des claims (incluant le rôle)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", userId)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Génération du token JWT
        var token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> SignupAsync(string email, string password, string role, string phone, string name)
    {
        // Vérifier si l'utilisateur existe déjà
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            // L'utilisateur existe déjà
            return false;
        }

        // Convertir le rôle string en UserRole
        if (!Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            // Si la conversion échoue, retourner false
            return false;
        }

        // Hacher le mot de passe
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Créer un nouvel utilisateur
        var user = new User
        {
            Email = email,
            Password = hashedPassword,
            Role = userRole,
            Phone = phone,
            Name = name,
        };
        // Envoyer un email de confirmation
        bool send_mail = EmailService.SendEmail(
            user.Email,
            "Merci pour votre inscription",
            "Merci pour votre inscription. Veuillez attendre l'approbation de l'administrateur."
        );

        Console.WriteLine($"SEND MAIL = {send_mail} ");

        if (send_mail == true)
        {
            // Ajouter l'utilisateur à la base de données
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
