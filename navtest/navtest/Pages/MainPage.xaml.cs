using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;

using AndroidX.Core.Content;
using Android;
using Android.Content.PM;
using System.Diagnostics;
using AndroidX.Core.App;
using Android.App;

namespace navtest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void btn_startScan(object sender, EventArgs e)
        {
            Debug.WriteLine("Start Scan!");
        }

        private void btn_stopScan(object sender, EventArgs e)
        {
            Debug.WriteLine("Stop Scan!");
        }

        //IBluetoothLE ble;
        //IAdapter adapter;
        //ObservableCollection<IDevice> deviceList;
        //IDevice device;
        //public List<DeviceListItemViewModel> SystemDevices { get; private set; } = new List<DeviceListItemViewModel>();

        private void TryStartScanning(bool refresh = false)
        {

        
            //if (IsStateOn && (refresh || !Devices.Any()) && !IsRefreshing)
            //{
            //    //ScanForDevices();
            //}
        }
    }
}
