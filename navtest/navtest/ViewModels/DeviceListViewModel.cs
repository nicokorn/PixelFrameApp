using Plugin.BLE.Abstractions.Contracts;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace navtest.ViewModels
{
    public class DeviceListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool _isRefreshing = true;

        public bool isRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged();
            }
        }

        public void setRefresh()
        {
            System.Diagnostics.Debug.WriteLine("Listview binding refresh!");
            _isRefreshing = true;
            RaisePropertyChanged();
        }

        public Command RefreshCommand { get; set; }

        public DeviceListViewModel()
        {
            RefreshCommand = new Command(() => OnRefresh());
        }

        private void OnRefresh()
        {
            //MainPage.bleScan();
            System.Diagnostics.Debug.WriteLine("¨Pull down refresh!");
        }
        protected void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
