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

        private static string dbPath =>
            Path.Combine(FileSystem.AppDataDirectory, "SeatReservation.db");

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Filename={dbPath}");
        }
    }
}