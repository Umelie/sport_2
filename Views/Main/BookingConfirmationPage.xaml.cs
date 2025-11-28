using oculus_sport.ViewModels.Main;
namespace oculus_sport.Views.Main;

public partial class BookingConfirmationPage : ContentPage
{
	public BookingConfirmationPage(BookingConfirmationViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}