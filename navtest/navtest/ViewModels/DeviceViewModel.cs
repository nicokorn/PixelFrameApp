using navtest.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace navtest.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        IBluetoothLE ble;
        IAdapter adapter;
        INavigation navigation;
        NativeDevice connectedDevice;

        private string _deviceName;
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
            set
            {
                _deviceName = value;
                RaisePropertyChanged("DeviceName");
            }
        }

        private string _lblRow;
        public string LblRow
        {
            get
            {
                return _lblRow;
            }
            set
            {
                _lblRow = value;
                RaisePropertyChanged("LblRow");
            }
        }

        private string _lblCol;
        public string LblCol
        {
            get
            {
                return _lblCol;
            }
            set
            {
                _lblCol = value;
                RaisePropertyChanged("LblCol");
            }
        }

        public Command PixelCommand { get; set; }

        private async void sendPixel()
        {
            Random rnd = new Random();
            Debug.WriteLine("Send pixel");
            int row = rnd.Next() % 18;
            byte[] data = { (byte)row, 0x00,    0x00, 0x00, (byte)rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next() };

            try
            {
                UUID_WS2812B_PIXEL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PIXEL_CHAR_UID));
                await UUID_WS2812B_PIXEL_CHAR.WriteAsync(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while sending pixel characteristics");
                //_userDialogs.HideLoading();
                //await _userDialogs.AlertAsync(ex.Message);
            }
        }

        public DeviceViewModel()
        {
        }

        public DeviceViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            connectedDevice = BaseViewModel.connectedDevice;

            PixelCommand = new Command(() => sendPixel());

            _deviceName = connectedDevice.Name;
            _lblRow = "na";
            _lblCol = "na";

            LoadServicesWS2812B();

            // register callbacks
            //ble.StateChanged += OnStateChanged;
            //adapter.DeviceDiscovered += OnDeviceDiscovered;
            //adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
            //adapter.DeviceDisconnected += OnDeviceDisconnected;
            //adapter.DeviceConnectionLost += OnDeviceConnectionLost;
        }

        private async void LoadServicesWS2812B()
        {
            try
            {
                // get uv service
                UUID_WS2812B_SERVICE = await connectedDevice.Device.GetServiceAsync(Guid.Parse(UUID_WS2812B_SERVICE_UID));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while discovering services");
                //await _userDialogs.AlertAsync(ex.Message, "Error while discovering services");
                //Trace.Message(ex.Message);
                //await _navigation.ChangePresentation(new MvxPopToRootPresentationHint());
            }
            //finally
            //{
            //    _userDialogs.HideLoading();
            //}

            try
            {
                // get characteristics of the service
                UUID_WS2812B_CMD_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_CMD_CHAR_UID));
                UUID_WS2812B_COL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_COL_CHAR_UID));
                UUID_WS2812B_ROW_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_ROW_CHAR_UID));
                UUID_WS2812B_PIXEL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PIXEL_CHAR_UID));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while discovering characteristics");
                //await _userDialogs.AlertAsync(ex.Message, "Error while discovering characteristics");
                //Trace.Message(ex.Message);
                //await _navigation.ChangePresentation(new MvxPopToRootPresentationHint());
            }
            //finally
            //{
            //    _userDialogs.HideLoading();
            //}

            try
            {
                // set cols on the uid curer state notification
                var bytesCOL = await UUID_WS2812B_COL_CHAR.ReadAsync();
                if (Buffer.ByteLength(bytesCOL) > 0)
                {
                    short num = BitConverter.ToInt16(bytesCOL, 0);
                    _lblCol = num.ToString();
                    RaisePropertyChanged("LblCol");

                    UUID_WS2812B_COL_CHAR.ValueUpdated += (o, args) =>
                    {
                        _lblCol = BitConverter.ToInt16(args.Characteristic.Value, 0).ToString();
                        RaisePropertyChanged("LblCol");
                    };
                    await UUID_WS2812B_COL_CHAR.StartUpdatesAsync();
                }

                // set cols on the uid curer state notification
                var bytesROW = await UUID_WS2812B_ROW_CHAR.ReadAsync();
                if (Buffer.ByteLength(bytesCOL) > 0)
                {
                    short num = BitConverter.ToInt16(bytesROW, 0);
                    _lblRow = num.ToString();
                    RaisePropertyChanged("LblRow");

                    UUID_WS2812B_ROW_CHAR.ValueUpdated += (o, args) =>
                    {
                        _lblRow = BitConverter.ToInt16(args.Characteristic.Value, 0).ToString();
                        RaisePropertyChanged("LblRow");
                    };
                    await UUID_WS2812B_ROW_CHAR.StartUpdatesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while reading characteristics");
                //await _userDialogs.AlertAsync(ex.Message, "Error while reading characteristics");
                //Trace.Message(ex.Message);
                //await _navigation.ChangePresentation(new MvxPopToRootPresentationHint());
            }
            //finally
            //{
            //    _userDialogs.HideLoading();
            //}
        }

    }
}
