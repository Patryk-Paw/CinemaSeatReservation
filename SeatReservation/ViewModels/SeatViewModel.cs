using Microsoft.Maui.Controls;
using SeatReservation.Models;
using SeatReservation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SeatReservation.ViewModels
{
    public class SeatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly SeatReservationDatabase _db = new();

        public ObservableCollection<Seat> Seats { get; set; } = new();

        public Movie? SelectedMovie { get; set; }

        public ICommand ReserveSeatCommand { get; }

        public SeatViewModel(Movie selectedMovie)
        {
            SelectedMovie = selectedMovie;
            ReserveSeatCommand = new Command<Seat>(async (seat) => await ReserveSeatAsync(seat));
            _ = LoadSeatsAsync();
        }

        private async Task LoadSeatsAsync()
        {
            if (SelectedMovie == null) return;

            var allSeats = await _db.GetSeatsAsync();
            var movieSeats = allSeats.Where(s => s.MovieId == SelectedMovie.MovieId);

            Seats.Clear();
            foreach (var seat in movieSeats)
                Seats.Add(seat);
        }

        private async Task ReserveSeatAsync(Seat seat)
        {
            if (seat != null && !seat.IsReserved)
            {
                string? userName = await Application.Current.MainPage.DisplayPromptAsync("Rezerwacja", "Podaj swoje imię");
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    seat.IsReserved = true;
                    await _db.SaveSeatAsync(seat);

                    var booking = new Booking
                    {
                        UserName = userName,
                        SeatId = seat.SeatId,
                        BookingTime = DateTime.Now
                    };
                    await _db.SaveBookingAsync(booking);

                    await LoadSeatsAsync();
                }
            }
        }
    }
}