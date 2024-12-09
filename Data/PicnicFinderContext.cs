using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;

public class PicnicFinderContext : DbContext
{
    public PicnicFinderContext(DbContextOptions<PicnicFinderContext> options) : base(options) { }
    
    public DbSet<Space> Spaces { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Space>()
                .HasKey(s => s.SpaceID);

            modelBuilder.Entity<Review>()
                .HasKey(r => r.ReviewID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Space)
                .WithMany() // Peut être modifié pour établir une relation bidirectionnelle
                .HasForeignKey(r => r.SpaceID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Employee)
                .WithMany() // Peut être modifié pour établir une relation bidirectionnelle
                .HasForeignKey(r => r.EmployeeID);

            modelBuilder.Entity<Reservation>()
                .HasKey(res => res.ReservationID);

            modelBuilder.Entity<Reservation>()
                .HasOne(res => res.Space)
                .WithMany() // Peut être modifié pour établir une relation bidirectionnelle
                .HasForeignKey(res => res.SpaceID);

            modelBuilder.Entity<Reservation>()
                .HasOne(res => res.Employee)
                .WithMany() // Peut être modifié pour établir une relation bidirectionnelle
                .HasForeignKey(res => res.EmployeeID);
        }
}
