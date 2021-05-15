using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Exceptions;

using System.Diagnostics;

using static navtest.ViewModels.DeviceListViewModel;
using navtest.ViewModels;

namespace navtest
{
    public partial class MainPage : ContentPage
    {
        IBluetoothLE ble;
        IAdapter adapter;
        ObservableCollection<IDevice> deviceList;
        IDevice device;
        Button btnScanCode;
        DeviceListViewModel test;

        public MainPage()
        {
            InitializeComponent();
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            deviceList = new ObservableCollection<IDevice>();
            lv.ItemsSource = deviceList;

            // register callbacks
            ble.StateChanged += OnStateChanged;
            adapter.DeviceDiscovered += OnDeviceDiscovered;
            adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            adapter.DeviceDisconnected += OnDeviceDisconnected;
            adapter.DeviceConnectionLost += OnDeviceConnectionLost;

            // buttons
            btnScanCode = this.FindByName<Button>("btnScan");

            // device list model
            test = new DeviceListViewModel();
        }

        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Debug.WriteLine("State change!");
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Debug.WriteLine("Device found!");
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("Timeout!");
            btnScanCode.IsEnabled = true;
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs args)
        {

        }

        private void OnDeviceConnectionLost(object sender, DeviceEventArgs args)
        {

        }
        private void btn_startScan(object sender, EventArgs e)
        {
            test.setRefresh();
            bleScan();
        }

        public async void bleScan()
        {
            var state = ble.State;

            if (state == BluetoothState.Off)
            {
                Debug.WriteLine("BLE off!");
                return;
            }

            btnScanCode.IsEnabled = false;

            Debug.WriteLine("Start Scan!");
            try
            {
                deviceList.Clear();
                adapter.DeviceDiscovered += (s, a) =>
                {
                    Debug.WriteLine("Discovered a device!");
                    deviceList.Add(a.Device);
                };

                //We have to test if the device is scanning 
                if (!ble.Adapter.IsScanning)
                {
                    await adapter.StartScanningForDevicesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Scan!");
            }
        }

        private void btn_stopScan(object sender, EventArgs e)
        {
            Debug.WriteLine("Stop Scan!");
        }

        private void lv_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (lv.SelectedItem == null)
            {
                return;
            }
            device = lv.SelectedItem as IDevice;
        }
    }
}
