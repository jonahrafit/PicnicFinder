using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AdminBO.Models;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AdminBO.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly AdminBOContext _dbContext;

    public AuthService(IConfiguration configuration, AdminBOContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<string> AuthenticateAsync(string username, string password)
    {
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La cl� secr�te JWT n'est pas configur�e.");
        }

        // R�cup�ration de l'utilisateur depuis la base de donn�es
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null; // Mauvais identifiants
        }

        // Cr�ation du r�le
        var role = user.Role.ToString();
        var userId = user.Id.ToString();
        Console.WriteLine($"ROLE TO STRING {role}");

        // Cr�ation des claims (incluant le r�le)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", userId),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // G�n�ration du token JWT
        var token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> SignupAsync(
        string email,
        string password,
        string role,
        string phone,
        string name
    )
    {
        // V�rifier si l'utilisateur existe d�j�
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            // L'utilisateur existe d�j�
            return false;
        }

        // Convertir le r�le string en UserRole
        if (!Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            // Si la conversion �choue, retourner false
            return false;
        }

        // Hacher le mot de passe
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Cr�er un nouvel utilisateur
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
            // Ajouter l'utilisateur � la base de donn�es
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
