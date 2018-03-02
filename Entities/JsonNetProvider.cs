using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mtn.Library.Extensions;
using Mtn.Library.Interface;
using Newtonsoft.Json;

namespace Mtn.Library.JsonNetProvider
{
    public class JSON:IJsonProvider
    {
        #region IJsonProvider Members

        public string Serialize(object value, bool ignoreNullData, bool indented)
        {
            var newtonSerializationSet = new Newtonsoft.Json.JsonSerializerSettings
            {
                NullValueHandling = ignoreNullData == true
                    ? Newtonsoft.Json.NullValueHandling.Ignore
                    : Newtonsoft.Json.NullValueHandling.Include
            };
            var referenceLoop = Configuration.Config.GetNullableEnum<ReferenceLoopHandling>("Mtn.Library.JsonNetProvider.ReferenceLoopHandling");
            if (referenceLoop != null)
                newtonSerializationSet.ReferenceLoopHandling = referenceLoop.Value;
            else
                newtonSerializationSet.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                var oldPreserveReferencesHandling =
                Configuration.Config.GetNullableEnum<PreserveReferencesHandling>(
                    "Mtn.Library.JsonNetProvide.PreserveReferencesHandling");
            var preserveReferencesHandling = Configuration.Config.GetNullableEnum<PreserveReferencesHandling>("Mtn.Library.JsonNetProvider.PreserveReferencesHandling");
            var useJavaScriptDateTimeConverter = Configuration.Config.GetNullableBoolean("Mtn.Library.JsonNetProvide.UseJavaScriptDateTimeConverter");
            
            if(useJavaScriptDateTimeConverter.HasValue && useJavaScriptDateTimeConverter.Value)
                newtonSerializationSet.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            else
                newtonSerializationSet.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
            
            newtonSerializationSet.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            newtonSerializationSet.Converters.Add(new EnumerableConverter());

            // compatibility
            if(oldPreserveReferencesHandling != null)
                newtonSerializationSet.PreserveReferencesHandling = oldPreserveReferencesHandling?? PreserveReferencesHandling.None;
            else
                newtonSerializationSet.PreserveReferencesHandling = preserveReferencesHandling ?? PreserveReferencesHandling.None;

            return Newtonsoft.Json.JsonConvert.SerializeObject(value, indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None, newtonSerializationSet);
        }

        public object DeSerialize(string value, Type type)
        {
            JsonSerializer json = new Newtonsoft.Json.JsonSerializer();

            json.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            json.Converters.Add(new EnumerableConverter());

            StringReader sr = new StringReader(value);
            Newtonsoft.Json.JsonTextReader reader = new Newtonsoft.Json.JsonTextReader(sr);
            object result = null;
            result = type == null ? Newtonsoft.Json.JsonConvert.DeserializeObject(value) : json.Deserialize(reader, type);

            reader.Close();

            return result;
        }

        
        public T DeSerialize<T>(string value)
        {
            return (T)DeSerialize(value, typeof(T));
        }

        #endregion

        
    }
}
