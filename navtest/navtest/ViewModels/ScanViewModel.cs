﻿using Plugin.BLE.Abstractions.Contracts;
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

using navtest.ViewModels;
using navtest.Models;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Threading;
using navtest.Views;

namespace navtest.ViewModels
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


        public ScanViewModel()
        {
        }

        public ScanViewModel( INavigation navigation )
        {
            this.navigation = navigation;

            RefreshCommand = new Command(() => OnStartScan(), () => ScanEnabled);
            ScanCommand = new Command(() => OnStartScan(), () => ScanEnabled);

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
            _items.Add(new NativeDevice(args.Device));
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("Timeout!");
            Debug.WriteLine("Item count: " + _items.Count);
            _scanEnabled = true;
            _isRefreshing = false;
            ScanCommand.ChangeCanExecute();
            RefreshCommand.ChangeCanExecute();
            RaisePropertyChanged("IsRefreshing");
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

        public Command RefreshCommand { get; set; }

        public Command ScanCommand { get; set; }

        private void OnStartScan()
        {
            System.Diagnostics.Debug.WriteLine("Scan button clicked!");
            bleScan();
            _scanEnabled = false;
            _isRefreshing = false;
            ScanCommand.ChangeCanExecute();
            RefreshCommand.ChangeCanExecute();
            RaisePropertyChanged("IsRefreshing");
        }

        private async void HandleSelectedDevice(NativeDevice device)
        {
            await App.Current.MainPage.DisplayAlert("Connectig", "Connecting to device.", "OK");
            if ( await ConnectDeviceAsync(device) )
           {
                Debug.WriteLine("Connected!");
                BaseViewModel.connectedDevice=device;
                await this.navigation.PushAsync(new DeviceView(), true);
            }
        }

        private async Task<bool> ConnectDeviceAsync(NativeDevice device, bool showPrompt = true)
        {
            try
            {
                await adapter.ConnectToDeviceAsync(device.Device, new ConnectParameters(autoConnect: true, forceBleTransport: true));
                connectedDevice = device;
                Debug.WriteLine($"Connected to {device.Device.Name}.");
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Connection error");
                return false;
            }
        }

        private async void DisconnectDevice(NativeDevice device)
        {
            await adapter.DisconnectDeviceAsync(device.Device);
        }
    }
}
