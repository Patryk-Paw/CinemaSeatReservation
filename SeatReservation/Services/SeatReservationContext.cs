using Microsoft.EntityFrameworkCore;
using SeatReservation.Models;
using System.IO;

namespace SeatReservation.Services
{
    public class SeatReservationContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public SeatReservationContext(DbContextOptions<SeatReservationContext> options) : base(options)
        {
        }

        public SeatReservationContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "SeatReservation.db");
                options.UseSqlite($"Filename={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Seats)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Booking)
                .WithOne(b => b.Seat)
                .HasForeignKey<Booking>(b => b.SeatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
