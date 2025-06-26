using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SeatReservation.Models;
using SeatReservation.Services;
using SeatReservation.ViewModels;
using Microsoft.Maui.Controls;

namespace SeatReservation.ViewModels
{
    public class SeatSelectionViewModel : INotifyPropertyChanged
    {
        private SeatReservationDatabase _db = new();
        public ObservableCollection<SeatViewModel> GroupedSeats { get; set; } = new();
        public string MovieTitle { get; set; } = string.Empty;

        public ICommand SelectSeatCommand { get; }

        public SeatSelectionViewModel()
        {
            SelectSeatCommand = new Command<SeatViewModel>(async (seat) => await ReserveSeatAsync(seat));
        }

        public async Task LoadSeatsAsync(int movieId)
        {
            GroupedSeats.Clear();
            var movie = (await _db.GetMoviesAsync()).FirstOrDefault(m => m.MovieId == movieId);
            if (movie == null) return;

            MovieTitle = $"{movie.MovieTitle} ({movie.ShowTime:HH:mm})";
            OnPropertyChanged(nameof(MovieTitle));

            var seats = movie.Seats.OrderBy(s => s.SeatRow).ThenBy(s => s.SeatNumber);
            foreach (var s in seats)
            {
                GroupedSeats.Add(new SeatViewModel(s));
            }
        }

        private async Task ReserveSeatAsync(SeatViewModel seatVM)
        {
            if (seatVM.Seat.IsReserved)
            {
                await Application.Current.MainPage.DisplayAlert("Zajęte", "To miejsce jest już zarezerwowane", "OK");
                return;
            }

            string? userName = await Application.Current.MainPage.DisplayPromptAsync("Rezerwacja", "Podaj imię");
            if (string.IsNullOrWhiteSpace(userName)) return;

            var booking = new Booking
            {
                SeatId = seatVM.Seat.SeatId,
                BookingTime = DateTime.Now,
                UserName = userName
            };

            seatVM.Seat.IsReserved = true;

            await _db.SaveBookingAsync(booking);
            await _db.SaveSeatAsync(seatVM.Seat);
            await LoadSeatsAsync(seatVM.Seat.MovieId); 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
