namespace TressMontage.Client.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var app = TressMontage.Client.App.Container;

            var setup = new Setup();
            setup.Bootstrap();

            LoadApplication(new TressMontage.Client.App());
        }
    }
}
