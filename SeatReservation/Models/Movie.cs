using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatReservation.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime ShowTime { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}