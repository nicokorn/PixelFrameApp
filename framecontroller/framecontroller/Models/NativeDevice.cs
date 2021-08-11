using Newtonsoft.Json;
using Plugin.BLE.Abstractions.Contracts;

namespace framecontroller.Models
{
    public class NativeDevice
    {
        public IDevice Device;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Rssi { get; set; }

        public bool IDeviceNull { get; set; }

        public override string ToString()
        {
            return "Name: " + this.Name + ", Id: " + this.Id + ", RSSI: " + this.Rssi;
        }

        [JsonConstructor]
        public NativeDevice(IDevice device)
        {
            Device = device;

            Id = device.Id.ToString();
            if (Id == null)
            {
                Id = "No id";
            }

            Name = device.Name;
            if (Name == null)
            {
                Name = "No Name";
            }

            Rssi = device.Rssi.ToString();
            if (Rssi == null)
            {
                Rssi = "Error";
            }

            IDeviceNull = false;
        }

        public NativeDevice(string deviceId, string deviceName)
        {
            Id = deviceId;
            if (Id == null)
            {
                Id = "No id";
            }

            Name = deviceName;
            if (Name == null)
            {
                Name = "No Name";
            }

            Rssi = "0";

            IDeviceNull = true;
        }
    }
}
