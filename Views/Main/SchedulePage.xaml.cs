using oculus_sport.ViewModels.Main;

namespace oculus_sport.Views.Main;

public partial class SchedulePage : ContentPage
{
    public SchedulePage(SchedulePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}