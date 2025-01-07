using System.Text;
using AdminBO.Controllers;
using AdminBO.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuration de Serilog
builder.Host.UseSerilog(
    (context, services, configuration) =>
    {
        configuration
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", context.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("EnvironmentName", context.HostingEnvironment.EnvironmentName)
            .ReadFrom.Configuration(context.Configuration);
    }
);

// Ajout des services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Configuration de DbContext avec SQL Server
builder.Services.AddDbContext<AdminBOContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configuration de l'authentification JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey =
    jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("La clé secrète JWT n'est pas configurée.");

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        };
    });

// Injection des dépendances des services
builder.Services.AddScoped<MenuService>();
builder.Services.AddScoped<SpaceService>();
builder.Services.AddScoped<ReservationService>();

// Configuration de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});
var app = builder.Build();

// Application des politiques CORS
app.UseCors("AllowAll");

// Gestion des erreurs et sécurité
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Migration de la base de données
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AdminBOContext>();
    context.Database.Migrate();
}
catch (Exception ex)
{
    Log.Error($"Erreur pendant la migration de la base de données : {ex.Message}");
}

// Pipeline middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Middleware pour injecter le JWT depuis les cookies
app.Use(
    async (context, next) =>
    {
        var token = context.Request.Cookies["jwt"];
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
            Log.Information("JWT injecté dans l'en-tête Authorization.");
        }
        await next();
    }
);

app.UseAuthentication();
app.UseAuthorization();

// Configuration des routes
app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=LoginBasic}/{id?}");

// Démarrage de l'application
app.Run();
