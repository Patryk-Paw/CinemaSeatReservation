using Microsoft.Maui.Controls;
using SeatReservation.Models;
using SeatReservation.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SeatReservation.ViewModels
{
    public class SeatSelectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        internal readonly SeatReservationDatabase _db = new();

        public ObservableCollection<SeatViewModel> Seats { get; } = new();

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged();
                    if (_selectedMovie != null)
                        _ = LoadSeatsAsync();
                }
            }
        }

        public ICommand ReserveSeatCommand { get; }

        public SeatSelectionViewModel()
        {
            ReserveSeatCommand = new Command<SeatViewModel>(async (seatVm) => await ReserveSeatAsync(seatVm));
        }

        public async Task LoadSeatsAsync()
        {
            if (SelectedMovie == null) return;

            Seats.Clear();

            var allSeats = await _db.GetSeatsAsync();
            var movieSeats = allSeats.Where(s => s.MovieId == SelectedMovie.MovieId)
                                     .OrderBy(s => s.SeatRow)
                                     .ThenBy(s => s.SeatNumber);

            foreach (var seat in movieSeats)
            {
                Seats.Add(new SeatViewModel(seat));
            }
        }

        private async Task ReserveSeatAsync(SeatViewModel seatVm)
        {
            if (seatVm.Seat.IsReserved)
            {
                await Application.Current.MainPage.DisplayAlert("Zajęte", "To miejsce jest już zarezerwowane", "OK");
                return;
            }

            string? userName = await Application.Current.MainPage.DisplayPromptAsync("Rezerwacja", "Podaj imię");
            if (string.IsNullOrWhiteSpace(userName)) return;

            seatVm.IsReserved = true;

            await _db.SaveSeatAsync(seatVm.Seat);

            var booking = new Booking
            {
                SeatId = seatVm.Seat.SeatId,
                MovieId = seatVm.Seat.MovieId,
                UserName = userName,
                BookingTime = System.DateTime.Now,
            };

            await _db.SaveBookingAsync(booking);

            await LoadSeatsAsync();
        }
    }
}
