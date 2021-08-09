using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace framecontroller.Models
{
    public class DeviceConcrete:IDevice
    {
        public IDevice Device { get; set; }

        public Guid Id => Device.Id;

        public string Name => Device.Name;

        public int Rssi => Device.Rssi;

        public object NativeDevice => Device.NativeDevice;

        public DeviceState State => Device.State;

        public IList<AdvertisementRecord> AdvertisementRecords => Device.AdvertisementRecords;

        public void Dispose()
        {
            Device.Dispose();
        }

        public Task<IService> GetServiceAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Device.GetServiceAsync(id, cancellationToken);
        }

        public Task<IReadOnlyList<IService>> GetServicesAsync(CancellationToken cancellationToken = default)
        {
            return Device.GetServicesAsync(cancellationToken);
        }

        public Task<int> RequestMtuAsync(int requestValue)
        {
            return Device.RequestMtuAsync(requestValue);
        }

        public bool UpdateConnectionInterval(ConnectionInterval interval)
        {
            return Device.UpdateConnectionInterval(interval);
        }

        public Task<bool> UpdateRssiAsync()
        {
            return Device.UpdateRssiAsync();
        }
    }
}
