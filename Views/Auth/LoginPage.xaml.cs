using oculus_sport.ViewModels.Auth;

namespace oculus_sport.Views.Auth;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}