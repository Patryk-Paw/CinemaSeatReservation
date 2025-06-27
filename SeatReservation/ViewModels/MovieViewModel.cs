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
        private readonly SeatReservationDatabase _db = new();

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
                    ((Command)DeleteMovieCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsMovieSelected => SelectedMovie != null;

        public ICommand AddMovieCommand { get; }
        public ICommand SelectMovieCommand { get; }
        public ICommand DeleteMovieCommand { get; }

        public MovieViewModel()
        {
            AddMovieCommand = new Command(async () => await AddMovieAsync());
            SelectMovieCommand = new Command(async () => await GoToSeatPageAsync(), () => IsMovieSelected);
            DeleteMovieCommand = new Command(async () => await DeleteSelectedMovieAsync(), () => IsMovieSelected);

            LoadMovies();
        }

        private async Task AddMovieAsync()
        {
            string? title = await Application.Current.MainPage.DisplayPromptAsync("Nowy Film", "Podaj tytuł filmu:");
            if (string.IsNullOrWhiteSpace(title)) return;

            string? dateTime = await Application.Current.MainPage.DisplayPromptAsync(title, "Podaj datę i/lub godzinę filmu:");
            if (string.IsNullOrWhiteSpace(dateTime)) return;

            string? rowsNumber = await Application.Current.MainPage.DisplayPromptAsync(title, "Podaj liczbę rzędów:");
            if (string.IsNullOrWhiteSpace(rowsNumber)) return;

            if (!int.TryParse(rowsNumber, out int rowsNumberInt))
            {
                await Application.Current.MainPage.DisplayAlert("Błąd", "Niepoprawna liczba rzędów", "OK");
                return;
            }

            var movie = new Movie
            {
                MovieTitle = title,
                ShowTime = dateTime != null ? DateTime.Parse(dateTime) : DateTime.Now,
            };

            await _db.SaveMovieAsync(movie, rows: rowsNumberInt, seatsPerRow: 8);
            await LoadMovies();
        }

        private async Task GoToSeatPageAsync()
        {
            if (SelectedMovie == null) return;

            var movieId = SelectedMovie.MovieId;
            await Shell.Current.GoToAsync($"SeatSelectionPage?movieId={movieId}");
        }

        private async Task DeleteSelectedMovieAsync()
        {
            if (SelectedMovie == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Potwierdzenie", $"Czy na pewno chcesz usunąć film \"{SelectedMovie.MovieTitle}\"?", "Tak", "Nie");

            if (!confirm) return;

            await _db.DeleteMovieAsync(SelectedMovie);
            SelectedMovie = null;
            await LoadMovies();
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
