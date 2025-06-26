using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace SeatReservation.Services
{
    public class SeatReservationContextFactory : IDesignTimeDbContextFactory<SeatReservationContext>
    {
        public SeatReservationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SeatReservationContext>();

            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "SeatReservation.db");

            optionsBuilder.UseSqlite($"Filename={dbPath}");

            return new SeatReservationContext(optionsBuilder.Options);
        }
    }
}
