using oculus_sport.ViewModels.Main;
namespace oculus_sport.Views.Main;

public partial class BookingDetailsPage : ContentPage
{
	public BookingDetailsPage(BookingDetailsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}