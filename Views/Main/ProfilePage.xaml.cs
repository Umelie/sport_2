using oculus_sport.ViewModels.Main;

namespace oculus_sport.Views.Main;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfilePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}