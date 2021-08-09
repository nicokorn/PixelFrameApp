using framecontroller.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace framecontroller.Helper
{
    public class NativeDeviceConverter //: JsonConverter
    {
        //public override bool CanWrite => false;
        //public override bool CanRead => true;
        //public override bool CanConvert(Type objectType)
        //{
        //    return objectType == typeof(IDevice);
        //}
        //public override void WriteJson(JsonWriter writer,
        //    object value, JsonSerializer serializer)
        //{
        //    throw new InvalidOperationException("Use default serialization.");
        //}
        //
        //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //{
        //    var jsonObject = JObject.Load(reader);
        //
        //    var device = default(IDevice);
        //
        //    device = new NativeDevice();
        //    serializer.Populate(jsonObject.CreateReader(), device);  // triggers exceptiom
        //    return device;
        //}



        //public override bool CanConvert(Type objectType)
        //{
        //    //assume we can convert to anything for now
        //    return true;
        //}
        //
        //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //{
        //    //explicitly specify the concrete type we want to create
        //    return serializer.Deserialize<NativeDevice>(reader);
        //}
        //
        //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    //use the default serialization - it works fine
        //    serializer.Serialize(writer, value);
        //}
    }
}
