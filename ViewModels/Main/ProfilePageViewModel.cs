using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oculus_sport.Services.Auth;
using oculus_sport.ViewModels.Base;

namespace oculus_sport.ViewModels.Main;

public partial class ProfilePageViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    // These would eventually come from a User Service
    [ObservableProperty]
    private string _name = "Tony Choo";

    [ObservableProperty]
    private string _studentId = "BCS23020003";

    [ObservableProperty]
    private string _email = "tony@student.uts.edu.my";

    public ProfilePageViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "My Profile";
    }

    [RelayCommand]
    async Task Logout()
    {
        bool confirm = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to log out?", "Yes", "No");
        if (confirm)
        {
            _authService.Logout();
            // Navigate back to Login Page (Absolute Route to clear stack)
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}