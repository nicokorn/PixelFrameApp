using navtest.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace navtest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var scanView = new ScanView();
            var navPage = new NavigationPage(scanView);
            this.MainPage = navPage;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
