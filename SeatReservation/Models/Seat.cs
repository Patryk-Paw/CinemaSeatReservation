using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatReservation.Models
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public bool IsReserved { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public Booking? Booking { get; set; }  
    }
}
