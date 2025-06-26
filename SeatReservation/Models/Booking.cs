using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatReservation.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; } = DateTime.Now;

        public int SeatId { get; set; }
        public Seat? Seat { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
    }
}