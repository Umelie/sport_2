using oculus_sport.ViewModels.Main;

namespace oculus_sport.Views.Main;

public partial class EventPage : ContentPage
{
    public EventPage(EventPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}