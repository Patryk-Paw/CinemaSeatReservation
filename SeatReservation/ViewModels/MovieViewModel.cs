using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged(nameof(SelectedMovie));
                    OnPropertyChanged(nameof(IsMovieSelected));
                    ((Command)SelectMovieCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsMovieSelected => SelectedMovie != null;

        public ICommand AddMovieCommand { get; }
        public ICommand SelectMovieCommand { get; }

        public MovieViewModel()
        {
            AddMovieCommand = new Command(async () => await AddMovieAsync());
            SelectMovieCommand = new Command(async () => await GoToSeatPageAsync(), () => IsMovieSelected);

            LoadMovies();
        }

        private async Task AddMovieAsync()
        {
            string? title = await Application.Current.MainPage.DisplayPromptAsync("Nowy Film", "Podaj tytuł filmu:");
            if (string.IsNullOrWhiteSpace(title)) return;

            var movie = new Movie
            {
                MovieTitle = title,
                ShowTime = DateTime.Now.AddHours(2)
            };

            await _db.SaveMovieAsync(movie, rows: 6, seatsPerRow: 8);
            await LoadMovies();
        }

        private async Task GoToSeatPageAsync()
        {
            if (SelectedMovie == null)
            {
                System.Diagnostics.Debug.WriteLine("SelectedMovie is null!");
                return;
            }

            var movieId = SelectedMovie.MovieId;
            await Shell.Current.GoToAsync($"SeatSelectionPage?movieId={movieId}");
        }


        private async Task LoadMovies()
        {
            Movies.Clear();
            var all = await _db.GetMoviesAsync();
            foreach (var movie in all)
            {
                Movies.Add(movie);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
