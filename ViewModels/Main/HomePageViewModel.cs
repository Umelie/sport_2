using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using oculus_sport.Models;
using oculus_sport.ViewModels.Base;

namespace oculus_sport.ViewModels.Main
{
    public partial class HomePageViewModel : BaseViewModel
    {
        // FIX: Use private fields (Standard MVVM Toolkit syntax)
        [ObservableProperty]
        private string _userName = "Tony";

        [ObservableProperty]
        private ObservableCollection<SportCategory> _categories = new();

        [ObservableProperty]
        private ObservableCollection<Facility> _facilities = new();

        public HomePageViewModel()
        {
            Title = "Home";
            LoadData();
        }

        private void LoadData()
        {
            // 1. Create Categories
            Categories.Add(new SportCategory { Name = "Pickleball", IsSelected = true });
            Categories.Add(new SportCategory { Name = "Badminton" });
            Categories.Add(new SportCategory { Name = "Football" });
            Categories.Add(new SportCategory { Name = "Tennis" });

            // 2. Create Facilities
            Facilities.Add(new Facility
            {
                Name = "Court 1",
                Location = "UTS Indoor Hall",
                Price = "RM 15/hr",
                Rating = 4.5,
                ImageUrl = "badminton_court.png"
            });

            Facilities.Add(new Facility
            {
                Name = "Court 2",
                Location = "UTS Indoor Hall",
                Price = "RM 20/hr",
                Rating = 4.8,
                ImageUrl = "badminton_court.png"
            });
        }

        [RelayCommand]
        void SelectCategory(SportCategory category)
        {
            if (Categories == null) return;

            foreach (var c in Categories)
            {
                c.IsSelected = false;
            }

            category.IsSelected = true;
        }

        [RelayCommand]
        async Task BookFacility(Facility facility)
        {
            if (facility == null) return;

            // Navigate to BookingPage and pass the selected Facility object
            var navigationParameter = new Dictionary<string, object>
            {
                { "Facility", facility }
            };

            await Shell.Current.GoToAsync(nameof(Views.Main.BookingPage), navigationParameter);
        }
    }
}