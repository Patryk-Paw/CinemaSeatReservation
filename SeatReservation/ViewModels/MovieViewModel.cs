using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SeatReservation.Models;
using SeatReservation.Services;
using Microsoft.Maui.Controls;

namespace SeatReservation.ViewModels
{
    public class MovieViewModel : INotifyPropertyChanged
    {
        private SeatReservationDatabase _db = new();
        public ObservableCollection<Movie> Movies { get; set; } = new();

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                _selectedMovie = value;
                OnPropertyChanged(nameof(SelectedMovie));
            }
        }

        public ICommand AddMovieCommand { get; }
        public ICommand SelectMovieCommand { get; }

        public MovieViewModel()
        {
            AddMovieCommand = new Command(async () => await AddMovieAsync());
            SelectMovieCommand = new Command(async () => await GoToSeatPageAsync());
            LoadMovies();
        }

        private async Task AddMovieAsync()
        {
            string title = await Application.Current.MainPage.DisplayPromptAsync("Nowy Film", "Tytuł:");
            if (string.IsNullOrWhiteSpace(title)) return;

            var movie = new Movie
            {
                MovieTitle = title,
                ShowTime = DateTime.Now.AddHours(2)
            };

            await _db.SaveMovieAsync(movie, rows: 6, seatsPerRow: 8);
            LoadMovies();
        }

        private async Task GoToSeatPageAsync()
        {
            if (SelectedMovie != null)
                await Shell.Current.GoToAsync($"SeatSelectionPage?movieId={SelectedMovie.MovieId}");
        }

        private async void LoadMovies()
        {
            Movies.Clear();
            var all = await _db.GetMoviesAsync();
            foreach (var m in all)
                Movies.Add(m);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
