using SeatReservation.ViewModels;

namespace SeatReservation.Views;

public partial class SeatSelectionPage : ContentPage
{
    private SeatSelectionViewModel _viewModel;

    public SeatSelectionPage(int movieId)
    {
        InitializeComponent();

        _viewModel = new SeatSelectionViewModel();
        BindingContext = _viewModel;

        LoadMovieAsync(movieId);
    }

    private async void LoadMovieAsync(int movieId)
    {
        var movies = await _viewModel._db.GetMoviesAsync();
        _viewModel.SelectedMovie = movies.FirstOrDefault(m => m.MovieId == movieId);

        await _viewModel.LoadSeatsAsync();
    }
}
