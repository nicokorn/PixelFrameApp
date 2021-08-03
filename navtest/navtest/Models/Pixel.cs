using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Forms;

namespace navtest.Models
{
    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Button Property { get; set; }

        public override string ToString()
        {
            return Property.Text;
        }

        public Pixel(int x, int y)
        {
            X = x;
            Y = y;
            Property = new Button();
        }
    }
}
