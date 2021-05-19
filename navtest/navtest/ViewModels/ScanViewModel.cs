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

using static navtest.ViewModels.DeviceListViewModel;
using navtest.ViewModels;
using navtest.Models;

namespace navtest.ViewModels
{
    public class ScanViewModel : INotifyPropertyChanged
    {
        private IDevice _nativeDevice;
        public event PropertyChangedEventHandler PropertyChanged;
        bool _isRefreshing;
        bool _scanEnabled;

        IBluetoothLE ble;
        IAdapter adapter;

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

        public IDevice NativeDevice
        {
            get
            {
                return _nativeDevice;
            }
            set
            {
                _nativeDevice = value;
                RaisePropertyChanged();
            }
        }

        public ScanViewModel()
        {
            RefreshCommand = new Command(() => OnStartScan());
            ScanCommand = new Command(() => OnStartScan(), () => ScanEnabled);

            _isRefreshing = true;
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
            _items.Add(new NativeDevice(args.Device));
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("Timeout!");
            Debug.WriteLine("Item count: " + _items.Count);
            _scanEnabled = true;
            ScanCommand.ChangeCanExecute();
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs args)
        {

        }

        private void OnDeviceConnectionLost(object sender, DeviceEventArgs args)
        {

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
                    await adapter.StartScanningForDevicesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error Scan!");
            }
        }

        private void lv__itemselected(object sender, SelectedItemChangedEventArgs e)
        {
            //if (lv.SelectedItem == null)
            //{
            //    return;
            //}
            //device = lv.SelectedItem as IDevice;
        }

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
            }
        }

        public Command RefreshCommand { get; set; }

        private void OnRefresh()
        {
            System.Diagnostics.Debug.WriteLine("¨Pull down refresh!");
        }

        public Command ScanCommand { get; set; }

        private void OnStartScan()
        {
            System.Diagnostics.Debug.WriteLine("Scan button clicked!");
            bleScan();
            _scanEnabled = false;
            ScanCommand.ChangeCanExecute();
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
