using Plugin.BLE.Abstractions.Contracts;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Exceptions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System;

using framecontroller.ViewModels;
using framecontroller.Models;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Threading;
using framecontroller.Views;
using Newtonsoft.Json;

namespace framecontroller.ViewModels
{
    public class ScanViewModel : BaseViewModel
    {
        IBluetoothLE ble;
        IAdapter adapter;
        INavigation navigation;
        bool _isRefreshing;
        bool _scanEnabled;
        NativeDevice connectedDevice;

        private ObservableCollection<NativeDevice> _items;
        public ObservableCollection<NativeDevice> Items
        {
            get 
            { 
                return _items; 
            }
            set
            {
                _items = value;
                RaisePropertyChanged();
            }
        }

        public NativeDevice SelectedDevice
        {
            get => null;
            set
            {
                if (value != null)
                {
                    HandleSelectedDevice(value);
                }

                RaisePropertyChanged();
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                _isRefreshing = value;
                RaisePropertyChanged("IsRefreshing");
            }
        }

        public bool ScanEnabled
        {
            get
            {
                return _scanEnabled;
            }
            set
            {
                _scanEnabled = value;
                ScanCommand.ChangeCanExecute();
                RaisePropertyChanged("ScanEnabled");
            }
        }

        public Command RefreshCommand { get; set; }
        public Command ScanCommand { get; set; }
        public Command AppearingCommand { get; set; }
        public Command DisappearingCommand { get; set; }

        public ScanViewModel()
        {
        }

        public ScanViewModel( INavigation navigation )
        {
            this.navigation = navigation;

            RefreshCommand = new Command(() => OnStartScan(), () => ScanEnabled);
            ScanCommand = new Command(() => OnStartScan(), () => ScanEnabled);
            AppearingCommand = new Command(() => OnAppearing());
            DisappearingCommand = new Command(() => OnDisappearing());

            _isRefreshing = false;
            _scanEnabled = true;

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            _items = new ObservableCollection<NativeDevice>();

            // register callbacks
            ble.StateChanged += OnStateChanged;
            adapter.DeviceDiscovered += OnDeviceDiscovered;
            adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            adapter.DeviceDisconnected += OnDeviceDisconnected;
            adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        }

        private void OnStateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Debug.WriteLine("State change!");
        }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs args)
        {
            Debug.WriteLine("Discovered a device!");
            bool deviceStored = false;
            for( int i=0; i<_items.Count; i++ )
            {
                if(_items[i].Id.Equals(args.Device.Id.ToString()) )
                {
                    _items[i] = new NativeDevice(args.Device);
                    deviceStored = true;
                    break;
                }
            }

            if( deviceStored != true )
            {
                _items.Add(new NativeDevice(args.Device));
            }
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("Timeout!");
            Debug.WriteLine("Item count: " + _items.Count);
            controlsEnabled(true);
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs args)
        {
            goBackNavigation();
            Debug.WriteLine("Disconnected from device!");
        }

        private void OnDeviceConnectionLost(object sender, DeviceEventArgs args)
        {
            goBackNavigation();
            Debug.WriteLine("Connection lost to device!");
        }

        public async void bleScan()
        {
            var state = ble.State;

            if (state == BluetoothState.Off)
            {
                Debug.WriteLine("BLE off!");
                return;
            }

            Debug.WriteLine("Start Scan!");
            try
            {
                _items.Clear();

                //We have to test if the device is scanning 
                if (!ble.Adapter.IsScanning)
                {
                    if (Application.Current.Properties.ContainsKey("deviceId") && Application.Current.Properties.ContainsKey("deviceName"))
                    {
                        string deviceId = Application.Current.Properties["deviceId"] as string;
                        string deviceName = Application.Current.Properties["deviceName"] as string;
                        NativeDevice stored = new NativeDevice(deviceId, deviceName);
                        _items.Add(stored);
                    }

                    await adapter.StartScanningForDevicesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Scan!: "+ex.Message + ", " + ex.StackTrace);
            }
        }

        private void OnStartScan()
        {
            System.Diagnostics.Debug.WriteLine("Scanning!");
            bleScan();
            controlsEnabled(false);
        }

        private void controlsEnabled( bool enabled )
        {
            if( enabled )
            {
                _scanEnabled = true;
                _isRefreshing = false;
                ScanCommand.ChangeCanExecute();
                RefreshCommand.ChangeCanExecute();
                RaisePropertyChanged("IsRefreshing");
            }
            else
            {
                _scanEnabled = false;
                _isRefreshing = false;
                ScanCommand.ChangeCanExecute();
                RefreshCommand.ChangeCanExecute();
                RaisePropertyChanged("IsRefreshing");
            }
        }

        private async void HandleSelectedDevice(NativeDevice device)
        {
            //We have to test if the device is scanning 
            if (ble.Adapter.IsScanning)
            {
                await adapter.StopScanningForDevicesAsync();
            }
            controlsEnabled(false);
            await App.Current.MainPage.DisplayAlert("Connecting", "Connecting to device.", "OK");
            if(device.IDeviceNull==false)
            {
                if (await ConnectDeviceAsync(device))
                {
                    Debug.WriteLine("Connected!");
                    BaseViewModel.connectedDevice = device;
                    await this.navigation.PushAsync(new DeviceView());
                    controlsEnabled(true);
                }
            }
            else
            {
                if (await ConnectToIdDeviceAsync(device))
                {
                    Debug.WriteLine("Connected!");
                    BaseViewModel.connectedDevice = device;
                    await this.navigation.PushAsync(new DeviceView());
                    controlsEnabled(true);
                }
            }
        }

        private async Task<bool> ConnectDeviceAsync(NativeDevice device, bool showPrompt = true)
        {
            try
            {
                await adapter.ConnectToDeviceAsync(device.Device, new ConnectParameters(autoConnect: true, forceBleTransport: true));
                connectedDevice = device;
                Application.Current.Properties["deviceId"] = device.Id;
                Application.Current.Properties["deviceName"] = device.Name;
                await Application.Current.SavePropertiesAsync();
                Debug.WriteLine($"Connected to {device.Device.Name}.");
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Connection error: "+ ex.Message);
                return false;
            }
        }

        private async Task<bool> ConnectToIdDeviceAsync(NativeDevice device, bool showPrompt = true)
        {
            try
            {
                device.Device = await adapter.ConnectToKnownDeviceAsync(Guid.Parse(device.Id), new ConnectParameters(autoConnect: true, forceBleTransport: true));
                connectedDevice = device;
                Application.Current.Properties["deviceId"] = device.Id;
                Application.Current.Properties["deviceName"] = device.Name;
                await Application.Current.SavePropertiesAsync();
                Debug.WriteLine($"Connected to {device.Name}.");
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Connection error: " + ex.Message);
                return false;
            }
        }

        private void OnAppearing()
        {
            DisconnectDeviceCurrent();
            OnStartScan();
        }

        private void OnDisappearing()
        {
            adapter.StopScanningForDevicesAsync();
        }

        private async void DisconnectDevice(NativeDevice device)
        {
            await adapter.DisconnectDeviceAsync(device.Device);
        }

        public async void DisconnectDeviceCurrent()
        {
            if( connectedDevice!=null )
            {
                Debug.WriteLine("Disconnecting!");
                await adapter.DisconnectDeviceAsync(connectedDevice.Device);
            }
        }

        private void goBackNavigation()
        {
            Device.BeginInvokeOnMainThread(async () => await navigation.PopAsync());
        }

        private async void ConnectionLostDialog()
        {
            await App.Current.MainPage.DisplayAlert("Error", "Connecting to the device lost.", "OK");
        }
    }
}
