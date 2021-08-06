using navtest.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Color = Xamarin.Forms.Color;

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

        private ObservableCollection<Pixel> _pixel;
        public ObservableCollection<Pixel> Pixel
        {
            get
            {
                return _pixel;
            }
            set
            {
                _pixel = value;
                RaisePropertyChanged();
            }
        }

        public Pixel SelectedPixel
        {
            get => null;
            set
            {
                HandleSelectedPixel(value);
                RaisePropertyChanged();
            }
        }

        private Color _selectedColor;
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                HandleSelectedColor(value);
                RaisePropertyChanged();
            }
        }

        public Command PixelCommand { get; set; }
        public Command EraseCommand { get; set; }
        public Command PictureCommand { get; set; }

        private async void sendPixelRandom()
        {
            Random rnd = new Random();
            Debug.WriteLine("Send pixel random");
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
                Debug.WriteLine(ex.Message, "Error while sending random pixel characteristics");
                //await App.Current.MainPage.DisplayAlert("Error", "Error while sending random pixel characteristics", "OK");
            }
        }

        private async void sendPixel(Pixel pixel, int col, int row, int r, int g, int b)
        {
            Debug.WriteLine("Send pixel");
            byte[] data = { (byte)CMD_BIT_REFRESH, (byte)row, 0x00, (byte)col, 0x00, (byte)r, (byte)g, (byte)b };
            Color tempColor = pixel.Property.BackgroundColor;

            try
            {
                pixel.Property.BackgroundColor = _selectedColor;
                UUID_WS2812B_PIXEL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PIXEL_CHAR_UID));
                await UUID_WS2812B_PIXEL_CHAR.WriteAsync(data);
            }
            catch (Exception ex)
            {
                pixel.Property.BackgroundColor = tempColor;
                Debug.WriteLine(ex.Message, "Error while sending pixel characteristics");
                //await App.Current.MainPage.DisplayAlert("Error", "Error while sending pixel characteristics", "OK");
            }
        }

        public interface IPhotoPickerService
        {
            Task<Stream> GetImageStreamAsync();
        }

        async void selectPicture()
        {
            Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (stream != null)
            {
                SKBitmap picture = SKBitmap.Decode(stream);
                //ScalePixels(SkiaSharp.SKPixmap destination, SkiaSharp.SKFilterQuality quality);
                sendPicture( picture );
            }
        }

        private async void sendPicture(SKBitmap picture )
        {
            int col                 = int.Parse(_lblCol);
            int row                 = int.Parse(_lblRow);
            int pixelcount          = col * row;
            int bytecount           = pixelcount * 3; // because 1 pixel = 3*8bit
            int packetcount         = bytecount / MAX_PICTURE_PAYLOAD + 1;
            byte[] data             = new byte[MAX_BLE_SIZE];
            byte[] pictureParsed    = new byte[bytecount];
            int offset;
            int size;
            SKColor pixelSKColor;
            int counter;
            int xDelta = 0;
            int yDelta = 0;

            // parse picture into the frames resolution
            xDelta = picture.Width / col;
            yDelta = picture.Height / row;
            counter = 0;
            for ( int y=yDelta/2; y<picture.Height; y+=yDelta )
            {
                for ( int x=xDelta/2; x<picture.Width; x+=xDelta )
                {
                    if( x>=picture.Width || y>=picture.Height || counter>=_pixel.Count )
                    {
                        break;
                    }
                    pixelSKColor = picture.GetPixel(x, y);
                    _pixel[counter].Property.BackgroundColor = Color.FromRgb(pixelSKColor.Red, pixelSKColor.Green, pixelSKColor.Blue);
                    pictureParsed[counter * 3] = pixelSKColor.Red;
                    pictureParsed[counter * 3 + 1] = pixelSKColor.Green;
                    pictureParsed[counter * 3 + 2] = pixelSKColor.Blue;
                    pictureParsed[counter * 3] /= 8;
                    pictureParsed[counter * 3 + 1] /= 8;
                    pictureParsed[counter * 3 + 2] /= 8;
                    counter++;
                }
            }

            try
            {
                UUID_WS2812B_PICTURE_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PICTURE_CHAR_UID));
                counter = 0;
                for (int packetnr = 0; packetnr < packetcount; packetnr++)
                {
                    // set header
                    offset = packetnr * MAX_PICTURE_PAYLOAD;
                    byte[] offsetB = BitConverter.GetBytes(offset);
                    data[1] = offsetB[0];
                    data[2] = offsetB[1];
            
                    if (packetnr == packetcount - 1)
                    {
                        // last packet
                        size = bytecount % MAX_PICTURE_PAYLOAD;
                        byte[] sizeB = BitConverter.GetBytes(size);
                        data[3] = sizeB[0];
                        data[4] = sizeB[1];
                        data[0] = CMD_BIT_REFRESH;
                    }
                    else
                    {
                        size = MAX_PICTURE_PAYLOAD;
                        byte[] sizeB = BitConverter.GetBytes(size);
                        data[3] = sizeB[0];
                        data[4] = sizeB[1];
                        data[0] = CMD_EMPTY;
                    }
            
                    // set picture data (3*8bits=1pixel)
                    for (int pixelbyte = 0; pixelbyte < size; pixelbyte++)
                    {
                        data[PICTURE_HEADER_OFFSET + pixelbyte] = pictureParsed[counter++];
                    }
            
                    // send the packet
                    await UUID_WS2812B_PICTURE_CHAR.WriteAsync(data);
                    Debug.WriteLine("Packet: " + (packetnr + 1) + "/" + packetcount + " sended, with " + (size + 4) + " bytes");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while sending picture");
                await App.Current.MainPage.DisplayAlert("Error", "Error while sending picture", "OK");
            }
        }

        private async void clearFrame()
        {
            Debug.WriteLine("Erase all pixel");

            try
            {
                UUID_WS2812B_PICTURE_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PICTURE_CHAR_UID));

                var data = new byte[PICTURE_HEADER_OFFSET];
                data[0] = CMD_BIT_CLEARALL | CMD_BIT_REFRESH; // clear frame cmd bit
                data[1] = 0x00;
                data[2] = 0x00;
                data[3] = 0x00;
                data[4] = 0x00;

                // send the packet
                await UUID_WS2812B_PICTURE_CHAR.WriteAsync(data);
                Debug.WriteLine("Send clear frame cmd");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while requesting to clear the frame");
                await App.Current.MainPage.DisplayAlert("Error", "Error while requesting to clear the frame", "OK");
            }

            clearFrameApp();
        }

        private void HandleSelectedPixel(Pixel pixel)
        {
            sendPixel(pixel, pixel.X, pixel.Y, (int)(_selectedColor.R * 255), (int)(_selectedColor.G * 255), (int)(_selectedColor.B * 255));
            Debug.WriteLine("Pushed Pixel x: " + pixel.X + ", y: " + pixel.Y);
        }

        private void HandleSelectedColor(Color color)
        {
            _selectedColor = color;
            Debug.WriteLine("Color selected");
        }

        private void clearFrameApp()
        {
            // clears pixel locally on the app
            for (int i = 0; i < _pixel.Count; i++)
            {
                _pixel[i].Property.BackgroundColor = Color.FromRgb(0, 0, 0);
            }
        }

        private void setFrameApp(byte[] data, int size)
        {
            if ((size / 3) >= _pixel.Count)
            {
                Debug.WriteLine("Error size is to big for the frame");
                return;
            }

            // for Version
            for (int i = 0; i < _pixel.Count; i++)
            {
                _pixel[i].Property.BackgroundColor = Color.FromRgb(data[3 * i], data[3 * i + 1], data[3 * i + 2]);
            }
        }

        public DeviceViewModel(INavigation navigation)
        {
            this.navigation = navigation;

            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            connectedDevice = BaseViewModel.connectedDevice;

            _pixel = new ObservableCollection<Pixel>();

            PictureCommand = new Command(() => selectPicture());
            PixelCommand = new Command(() => sendPixelRandom());
            EraseCommand = new Command(() => clearFrame());

            _deviceName = connectedDevice.Name;
            _lblRow = "na";
            _lblCol = "na";
            _selectedColor = Color.FromRgb(0, 0, 0);

            LoadServicesWS2812B();

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    Pixel pixel;
                    pixel = new Pixel(x, y);
                    pixel.Property.BackgroundColor = Color.FromRgb(0, 0, 0);
                    pixel.Property.Text = "I am a pixel :-)";
                    _pixel.Add(pixel);
                }
            }
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
                await App.Current.MainPage.DisplayAlert("Error", "Error while discovering services", "OK");
            }

            try
            {
                // get characteristics of the service
                UUID_WS2812B_COL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_COL_CHAR_UID));
                UUID_WS2812B_ROW_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_ROW_CHAR_UID));
                UUID_WS2812B_PIXEL_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PIXEL_CHAR_UID));
                UUID_WS2812B_PICTURE_CHAR = await UUID_WS2812B_SERVICE.GetCharacteristicAsync(Guid.Parse(UUID_WS2812B_PICTURE_CHAR_UID));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while discovering characteristics");
                await App.Current.MainPage.DisplayAlert("Error", "Error while discovering characteristics", "OK");
            }

            try
            {
                // set cols on the uid state notification
                var bytesCOL = await UUID_WS2812B_COL_CHAR.ReadAsync();
                if (Buffer.ByteLength(bytesCOL) > 0)
                {
                    short num = BitConverter.ToInt16(bytesCOL, 0);
                    _lblCol = num.ToString();
                    RaisePropertyChanged("LblCol");

                    //UUID_WS2812B_COL_CHAR.ValueUpdated += (o, args) =>
                    //{
                    //    _lblCol = BitConverter.ToInt16(args.Characteristic.Value, 0).ToString();
                    //    RaisePropertyChanged("LblCol");
                    //};
                    //await UUID_WS2812B_COL_CHAR.StartUpdatesAsync();
                }

                // set cols on the uid state notification
                var bytesROW = await UUID_WS2812B_ROW_CHAR.ReadAsync();
                if (Buffer.ByteLength(bytesCOL) > 0)
                {
                    short num = BitConverter.ToInt16(bytesROW, 0);
                    _lblRow = num.ToString();
                    RaisePropertyChanged("LblRow");

                    //UUID_WS2812B_ROW_CHAR.ValueUpdated += (o, args) =>
                    //{
                    //    _lblRow = BitConverter.ToInt16(args.Characteristic.Value, 0).ToString();
                    //    RaisePropertyChanged("LblRow");
                    //};
                    //await UUID_WS2812B_ROW_CHAR.StartUpdatesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "Error while reading characteristics");
                await App.Current.MainPage.DisplayAlert("Error", "Error while reading characteristics", "OK");
            }
        }
    }
}
