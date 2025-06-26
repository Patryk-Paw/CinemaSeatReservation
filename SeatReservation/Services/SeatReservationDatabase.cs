using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SeatReservation.Models;

namespace SeatReservation.Services
{
    public class SeatReservationDatabase
    {
        public SeatReservationDatabase()
        {
            using var db = new SeatReservationContext();
            db.Database.EnsureCreated();
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            using var db = new SeatReservationContext();
            return await db.Movies.Include(m => m.Seats).ToListAsync();
        }

        public async Task SaveMovieAsync(Movie movie, int rows = 0, int seatsPerRow = 0)
        {
            using var db = new SeatReservationContext();

            if (movie.MovieId != 0)
            {
                db.Movies.Update(movie);
            }
            else
            {
                db.Movies.Add(movie);
                await db.SaveChangesAsync();

                if (rows > 0 && seatsPerRow > 0)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        for (int seat = 0; seat < seatsPerRow; seat++)
                        {
                            var s = new Seat
                            {
                                MovieId = movie.MovieId,
                                SeatRow = row,
                                SeatNumber = seat,
                                IsReserved = false
                            };
                            db.Seats.Add(s);
                        }
                    }
                }
            }

            await db.SaveChangesAsync();
        }

        public async Task DeleteMovieAsync(Movie movie)
        {
            using var db = new SeatReservationContext();
            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
        }

        public async Task<List<Seat>> GetSeatsAsync()
        {
            using var db = new SeatReservationContext();
            return await db.Seats.Include(s => s.Movie).ToListAsync();
        }

        public async Task SaveSeatAsync(Seat seat)
        {
            using var db = new SeatReservationContext();
            if (seat.SeatId != 0)
                db.Seats.Update(seat);
            else
                db.Seats.Add(seat);

            await db.SaveChangesAsync();
        }

        public async Task DeleteSeatAsync(Seat seat)
        {
            using var db = new SeatReservationContext();
            db.Seats.Remove(seat);
            await db.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetBookingsAsync()
        {
            using var db = new SeatReservationContext();
            return await db.Bookings
                           .Include(b => b.Seat)
                           .ThenInclude(s => s.Movie)
                           .ToListAsync();
        }

        public async Task SaveBookingAsync(Booking booking)
        {
            using var db = new SeatReservationContext();
            if (booking.BookingId != 0)
                db.Bookings.Update(booking);
            else
                db.Bookings.Add(booking);

            await db.SaveChangesAsync();
        }

        public async Task DeleteBookingAsync(Booking booking)
        {
            using var db = new SeatReservationContext();
            db.Bookings.Remove(booking);
            await db.SaveChangesAsync();
        }
    }
}
