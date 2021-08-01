﻿using navtest.Models;
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
        public Command EraseCommand { get; set; }

        private async void sendPixel()
        {
            Random rnd = new Random();
            Debug.WriteLine("Send pixel");
            int row = rnd.Next() % int.Parse(_lblRow);
            int col = rnd.Next() % int.Parse(_lblCol);
            byte[] data = { (byte)row, 0x00, (byte)col, 0x00, (byte)rnd.Next(), (byte)rnd.Next(), (byte)rnd.Next() };

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

        private async void erasePixel()
        {
            Debug.WriteLine("Erase all pixel");

            try
            {
                //UUID_WS2812B_PIXEL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PIXEL_CHAR_UID));
                //for(int row=0; row < int.Parse(_lblRow); row++)
                //{
                //    for (int col = 0; col < int.Parse(_lblCol); col++)
                //    {
                //        byte[] data = { (byte)col, 0x00, (byte)row, 0x00, 0x00, 0x00, 0x00 };
                //        await UUID_WS2812B_PIXEL_CHAR.WriteAsync(data);
                //    }
                //}

                UUID_WS2812B_PICTURE_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PICTURE_CHAR_UID));

                var data = new byte[MAX_BLE_SIZE];
                int pixelcount = int.Parse(_lblCol) * int.Parse(_lblRow);
                int bytecount = pixelcount*3; // because 1 pixel = 3*8bit
                int packetcount = bytecount / MAX_PICTURE_PAYLOAD + 1;
                int offset;
                int size;

                for (int packetnr=0; packetnr < packetcount; packetnr++)
                {
                    // set header
                    offset = packetnr * MAX_PICTURE_PAYLOAD;
                    byte[] offsetB = BitConverter.GetBytes(offset);
                    data[0] = offsetB[0];
                    data[1] = offsetB[1];

                    if(packetnr== packetcount-1)
                    {
                        // last packet
                        size = bytecount % MAX_PICTURE_PAYLOAD;
                        byte[] sizeB = BitConverter.GetBytes(size);
                        data[2] = sizeB[0];
                        data[3] = sizeB[1];
                        data[4] = 1;
                    }
                    else
                    {
                        size = MAX_PICTURE_PAYLOAD;
                        byte[] sizeB = BitConverter.GetBytes(size);
                        data[2] = sizeB[0];
                        data[3] = sizeB[1];
                        data[4] = 0;
                    }

                    // set picture data (3*8bits=1pixel)
                    for(int pixelbyte=0; pixelbyte < size; pixelbyte++)
                    {
                        data[PICTURE_HEADER_OFFSET + pixelbyte] = 0x00;
                    }

                    // send the packet
                    await UUID_WS2812B_PICTURE_CHAR.WriteAsync(data);
                    Debug.WriteLine("Packet: "+(packetnr+1)+"/"+ packetcount + " sended, with "+(size+4)+" bytes");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while sending picture");
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
            EraseCommand = new Command(() => erasePixel());

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