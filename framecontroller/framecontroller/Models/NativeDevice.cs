using Newtonsoft.Json;
using Plugin.BLE.Abstractions.Contracts;

namespace framecontroller.Models
{
    public class NativeDevice
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string Rssi { get; set; }

        public IDevice Device;

        public override string ToString()
        {
            return "Name: " + this.Name + ", Id: " + this.Id + ", RSSI: " + this.Rssi;
        }

        [JsonConstructor]
        public NativeDevice()
        {
        }

        public NativeDevice(IDevice device)
        {
            Id = device.Id.ToString();
            if(Id == null)
            {
                Id = "No id";
            }

            Name = device.Name;
            if(Name == null)
            {
                Name = "No Name";
            }

            Rssi = device.Rssi.ToString();
            if(Rssi == null)
            {
                Rssi = "Error";
            }

            this.Device = device;
        }
    }
}
