using oculus_sport.ViewModels.Main;

namespace oculus_sport.Views.Main;

public partial class HomePage : ContentPage
{
    // 1. Inject the ViewModel through the constructor
    public HomePage(HomePageViewModel vm)
    {
        InitializeComponent();

        // 2. Set the BindingContext. This connects the XAML to the Data.
        BindingContext = vm;
    }
}