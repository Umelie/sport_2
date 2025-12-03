using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using oculus_sport.Models;
using oculus_sport.Services.Storage;
using oculus_sport.ViewModels.Base;
using System.Collections.ObjectModel;

namespace oculus_sport.ViewModels.Main
{
    [QueryProperty(nameof(CurrentUser), "User")]
    public partial class HomePageViewModel : BaseViewModel
    {
        //------------ services
        private readonly FirebaseDataService _firebaseService;
        private string? _idToken;
        private List<Facility> _allFacilities = new();

        //------------- observable properties
        [ObservableProperty] private ObservableCollection<SportCategory> _categories = new();
        [ObservableProperty] private ObservableCollection<Facility> _facilities = new();
        [ObservableProperty] private string userName;
        [ObservableProperty] private User currentUser; 




        //----------------- sync user name with login
        partial void OnCurrentUserChanged(User value)
        {
            if (value != null)
                UserName = value.Name;
        }

        // ------------- constructor
        public HomePageViewModel(FirebaseDataService firebaseService)
        {
            _firebaseService = firebaseService;
            Title = "Home";

            Categories.Add(new SportCategory { Name = "Badminton", IsSelected = true });
            Categories.Add(new SportCategory { Name = "Ping-Pong" });
            Categories.Add(new SportCategory { Name = "Basketball" });

            // Default filter
            //FilterFacilities("Badminton");
        }


        // --------- sycn homepage user name
        public async Task UserHomepageSync(string uid, string idToken)
        {
            _idToken = idToken;

            var user = await _firebaseService.GetUserFromFirestore(uid, idToken);
            if (user != null)
            {
                user.IdToken = idToken;
                CurrentUser = user;
            }
        }

        // --------- load all facilities
        public async Task LoadFacilitiesAsync()
        {
            Facilities.Clear();

            _allFacilities = await _firebaseService.GetFacilitiesAsync(); // store all facilities

            foreach (var facility in _allFacilities)
            {
                Facilities.Add(facility);
                Console.WriteLine($"[DEBUG] Loaded facility: {facility.Name}");
            }

            Console.WriteLine($"[DEBUG] Facilities count: {Facilities.Count}");
        }



        //---------------- command category
        [RelayCommand]
        void SelectCategory(SportCategory category)
        {
            if (Categories == null) return;
            foreach (var c in Categories) c.IsSelected = false;
            category.IsSelected = true;
            FilterFacilities(category.Name);
        }


        //private async Task LoadData()
        //{
        //    Categories.Add(new SportCategory { Name = "Badminton", IsSelected = true });
        //    Categories.Add(new SportCategory { Name = "Ping-Pong" });
        //    Categories.Add(new SportCategory { Name = "Basketball" });

        //    _allFacilities.Clear();
        //    for (int i = 1; i <= 3; i++)
        //        _allFacilities.Add(new Facility { Name = $"Badminton Court {i}", Location = "UTS Indoor Hall", Price = "Free", Rating = 4.5, ImageUrl = "court_badminton.png" });

        //    for (int i = 1; i <= 4; i++)
        //        _allFacilities.Add(new Facility { Name = $"Ping-Pong Table {i}", Location = "Student Center L2", Price = "Free", Rating = 4.8, ImageUrl = "court_pingpong.png" });

        //    _allFacilities.Add(new Facility { Name = "Basketball Court 1", Location = "Outdoor Complex", Price = "Free", Rating = 4.2, ImageUrl = "court_basketball.png" });

        //    FilterFacilities("Badminton");
        //}


        // --------------------------------------------------------------------|


        private void FilterFacilities(string categoryName)
        {
            Facilities.Clear();
            var filtered = _allFacilities.Where(f => f.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase));
            foreach (var facility in filtered) Facilities.Add(facility);
        }

        [RelayCommand]
        async Task BookFacility(Facility facility)
        {
            var navigationParameter = new Dictionary<string, object> { { "Facility", facility } };
            await Shell.Current.GoToAsync("BookingPage", navigationParameter);
        }

        [RelayCommand]
        async Task GoToNotifications()
        {
            await Shell.Current.GoToAsync("NotificationPage");
        }
    }
}