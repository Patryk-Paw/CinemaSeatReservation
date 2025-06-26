using System.ComponentModel;
using System.Runtime.CompilerServices;
using SeatReservation.Models;

namespace SeatReservation.ViewModels
{
    public class SeatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private Seat _seat;

        public Seat Seat
        {
            get => _seat;
            set
            {
                if (_seat != value)
                {
                    _seat = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsReserved));
                    OnPropertyChanged(nameof(SeatRow));
                    OnPropertyChanged(nameof(SeatNumber));
                }
            }
        }

        public bool IsReserved
        {
            get => Seat.IsReserved;
            set
            {
                if (Seat.IsReserved != value)
                {
                    Seat.IsReserved = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SeatRow => Seat.SeatRow;
        public int SeatNumber => Seat.SeatNumber;

        public SeatViewModel(Seat seat)
        {
            Seat = seat;
        }
    }
}
