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
        // R�cup�ration de l'utilisateur depuis la base de donn�es
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return null;
        }
        var tokenString = generateTokenString(user);
        return tokenString;
    }

    public async Task<string> SignupAsync(
        string email,
        string password,
        string role,
        string phone,
        string name
    )
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (existingUser != null)
        {
            // L'utilisateur existe d�j�
            return null;
        }

        // Convertir le r�le string en UserRole
        if (!Enum.TryParse<UserRole>(role, true, out var userRole))
        {
            // Si la conversion �choue, retourner false
            return null;
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
            return generateTokenString(user);
        }

        return null;
    }

    public TokenModel DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        // Vérifiez si le token est valide (si nécessaire)
        if (!handler.CanReadToken(token))
        {
            throw new ArgumentException("Token invalide.");
        }

        var jwtToken = handler.ReadJwtToken(token);

        // Récupérer les claims (données utilisateur)
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var expiration = jwtToken.ValidTo;

        return new TokenModel
        {
            UserId = userId,
            Name = name,
            Role = role,
            Expiration = expiration,
        };
    }

    public async Task<UserInfoModel> GetUserInfoFromTokenAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token))
        {
            throw new ArgumentException("Token invalide.");
        }

        var jwtToken = handler.ReadJwtToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("L'ID utilisateur est manquant dans le token.");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
        if (user == null)
        {
            throw new InvalidOperationException("Utilisateur non trouvé.");
        }

        return new UserInfoModel
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            Phone = user.Phone,
        };
    }

    public string generateTokenString(User user)
    {
        var secretKey = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("La cl� secr�te JWT n'est pas configur�e.");
        }

        // Cr�ation du r�le
        var role = user.Role.ToString();
        var userId = user.Id.ToString();
        var name = user.Name.ToString();

        // Cr�ation des claims (incluant le r�le)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", userId),
            new Claim("Role", role),
            new Claim("Name", name),
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

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenString;
    }
}
