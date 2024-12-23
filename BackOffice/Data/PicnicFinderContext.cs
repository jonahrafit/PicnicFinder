using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;

public class PicnicFinderContext : DbContext
{
    // public PicnicFinderContext(DbContextOptions<PicnicFinderContext> options) : base(options) { }
    
    private readonly IConfiguration _configuration;
    public DbSet<User> Users { get; set; }
    public DbSet<Space> Space { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Menu> Menus { get; set; }

    public PicnicFinderContext(DbContextOptions<PicnicFinderContext> options, IConfiguration configuration)
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
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Employee)
            .WithMany() // Ou .WithMany(r => r.Reservations) si la relation inverse existe
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict); // Pas de cascade

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Space)
            .WithMany() // Ou .WithMany(s => s.Reservations) si la relation inverse existe
            .HasForeignKey(r => r.SpaceId)
            .OnDelete(DeleteBehavior.Restrict); // Pas de cascade
    }
}
