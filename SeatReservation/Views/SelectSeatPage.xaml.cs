using SeatReservation.ViewModels;
namespace SeatReservation.Views;

[QueryProperty(nameof(MovieId), "movieId")]
public partial class SeatSelectionPage : ContentPage
{
    public string MovieId { get; set; }

    public SeatSelectionPage()
    {
        InitializeComponent();
        BindingContext = new SeatSelectionViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is SeatSelectionViewModel vm && int.TryParse(MovieId, out int id))
            await vm.LoadSeatsAsync(id);
    }
}
