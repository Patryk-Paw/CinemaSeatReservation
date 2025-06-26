using Microsoft.Maui.Controls;
using SeatReservation.ViewModels;
using SeatReservation.Views;

namespace SeatReservation
{
    public partial class MainPage : ContentPage
    {
        private MovieViewModel _viewModel = new MovieViewModel();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = _viewModel;
        }

        private async void OnSelectMovieClicked(object sender, EventArgs e)
        {
            if (_viewModel.SelectedMovie != null)
            {
                await Navigation.PushAsync(new SeatSelectionPage(_viewModel.SelectedMovie.MovieId));
            }
        }
    }
}
