using oculus_sport.ViewModels.Main;

namespace oculus_sport.Views.Main;

public partial class BookingPage : ContentPage
{
    public BookingPage(BookingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}