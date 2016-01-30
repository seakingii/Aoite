using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Serialization;
using System.Web;

namespace Newtonsoft.Json
{
    //internal class JsonEnumConverter : JsonConverter
    //{
    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        if(value == null) writer.WriteNull();
    //        else
    //        {
    //            string s = value.ToString();
    //            var resolver = WebConfig.DefaultJsonSetting.ContractResolver as CamelCasePropertyNamesContractResolver;
    //            if(resolver != null) s = resolver.GetResolvedPropertyName(s);
    //            writer.WriteValue(s);
    //        }
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        return Enum.Parse(objectType, reader.Value.ToString(), true);
    //    }

    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType.IsEnum;
    //    }
    //}
}
