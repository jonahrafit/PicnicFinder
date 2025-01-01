using AdminBO.Models;
using Microsoft.EntityFrameworkCore;

public class AdminBOContext : DbContext
{
    // public AdminBOContext(DbContextOptions<AdminBOContext> options) : base(options) { }

    private readonly IConfiguration _configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<Space> Space { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    public AdminBOContext(DbContextOptions<AdminBOContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Si la chaîne de connexion n'est pas configurée via AddDbContext
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Reservation>()
            .HasOne(r => r.Employee)
            .WithMany() // Ou .WithMany(r => r.Reservations) si la relation inverse existe
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict); // Pas de cascade

        modelBuilder
            .Entity<Reservation>()
            .HasOne(r => r.Space)
            .WithMany() // Ou .WithMany(s => s.Reservations) si la relation inverse existe
            .HasForeignKey(r => r.SpaceId)
            .OnDelete(DeleteBehavior.Restrict); // Pas de cascade

        // Configure relations if necessary
        modelBuilder
            .Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany() // Pas de relation inverse
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Favorite>()
            .HasOne(f => f.Space)
            .WithMany() // Pas de relation inverse
            .HasForeignKey(f => f.SpaceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder
            .Entity<Space>()
            .HasOne(s => s.Owner) // Chaque Space a un seul Owner
            .WithMany() // Il n'y a pas de collection inverse dans User
            .HasForeignKey(s => s.OwnerId) // Clé étrangère dans Space
            .OnDelete(DeleteBehavior.Restrict);
    }
}
