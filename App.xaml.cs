
namespace oculus_sport
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        internal static void Configure()
        {
            throw new NotImplementedException();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}