using navtest.Models;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace navtest.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static NativeDevice connectedDevice;

        protected const string UUID_BASE = "a2bfXXXX-7936-bc40-a3d9-1ea3b1cc51bc";

        protected IService UUID_WS2812B_SERVICE;
        protected const string UUID_WS2812B_SERVICE_UID = "a2bf1001-7936-bc40-a3d9-1ea3b1cc51bc";

        protected ICharacteristic UUID_WS2812B_CMD_CHAR;
        protected const string UUID_WS2812B_CMD_CHAR_UID = "a2bf1002-7936-bc40-a3d9-1ea3b1cc51bc";

        protected ICharacteristic UUID_WS2812B_ROW_CHAR;
        protected const string UUID_WS2812B_ROW_CHAR_UID = "a2bf1003-7936-bc40-a3d9-1ea3b1cc51bc";

        protected ICharacteristic UUID_WS2812B_COL_CHAR;
        protected const string UUID_WS2812B_COL_CHAR_UID = "a2bf1004-7936-bc40-a3d9-1ea3b1cc51bc";

        protected ICharacteristic UUID_WS2812B_PIXEL_CHAR;
        protected const string UUID_WS2812B_PIXEL_CHAR_UID = "a2bf1005-7936-bc40-a3d9-1ea3b1cc51bc";

        protected ICharacteristic UUID_WS2812B_PICTURE_CHAR;
        protected const string UUID_WS2812B_PICTURE_CHAR_UID = "a2bf1006-7936-bc40-a3d9-1ea3b1cc51bc";

        protected const int PICTURE_HEADER_OFFSET = 5;
        protected const int MAX_BLE_SIZE = 506;
        protected const int MAX_PICTURE_PAYLOAD = MAX_BLE_SIZE - PICTURE_HEADER_OFFSET; // payload X size needs to be X%3=0!

        protected const int CMD_BIT_REFRESH = 0x01;
        protected const int CMD_BIT_CLEARALL = 0x02;

        protected void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }
    }
}
