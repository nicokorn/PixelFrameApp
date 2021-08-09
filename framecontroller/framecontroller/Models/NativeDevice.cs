using framecontroller.Helper;
using Newtonsoft.Json;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace framecontroller.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class NativeDevice:IDevice
    {
        //[JsonIgnoreAttribute]
        [JsonConverter(typeof(NativeDeviceConverter))]
        public IDevice Device;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Rssi { get; set; }

        Guid IDevice.Id => Device.Id;

        int IDevice.Rssi => Device.Rssi;

        object IDevice.NativeDevice => Device.NativeDevice;

        public DeviceState State => Device.State;

        [JsonIgnore]
        public IList<AdvertisementRecord> AdvertisementRecords => Device.AdvertisementRecords;

        public override string ToString()
        {
            return "Name: " + this.Name + ", Id: " + this.Id + ", RSSI: " + this.Rssi;
        }

        public Task<IReadOnlyList<IService>> GetServicesAsync(CancellationToken cancellationToken = default)
        {
            return Device.GetServicesAsync(cancellationToken);
        }

        public Task<IService> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Device.GetServiceAsync(id, cancellationToken);
        }

        public Task<bool> UpdateRssiAsync()
        {
            return Device.UpdateRssiAsync();
        }

        public Task<int> RequestMtuAsync(int requestValue)
        {
            return Device.RequestMtuAsync(requestValue);
        }

        public bool UpdateConnectionInterval(ConnectionInterval interval)
        {
            return Device.UpdateConnectionInterval(interval);
        }

        public void Dispose()
        {
            Device.Dispose();
        }

        public NativeDevice()
        {
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
        }
    }
}
