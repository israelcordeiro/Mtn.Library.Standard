using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections;


namespace Mtn.Library.Extensions
{
    /// <summary>    
    /// <para>This class solves some problems for conversion into JSON Newtonsoft with Enumerable that have property's.</para>
    /// </summary>
    public class EnumerableConverter : CustomCreationConverter<IEnumerable>
    {
        /// <summary>    
        /// <para>Not implemented.</para>
        /// <param name="objectType">
        /// <para>Not implemented.</para>
        /// </param>
        /// </summary>
        /// <returns>
        /// <para>Not implemented.</para>
        /// </returns>
        public override IEnumerable Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        /// <summary>    
        /// <para>Writes the JSON representation of the object.</para>
        /// </summary>
        /// <param name="writer">
        /// <para>The Newtonsoft.Json.JsonWriter to write to.</para>
        /// </param>
        /// <param name="value">
        /// <para>The value.</para>
        /// </param>
        /// <param name="serializer">
        /// <para>The calling serializer.</para>
        /// </param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string resp = "";
            if(value is IEnumerable<string> || UseSerializer(value) == false)
            {
                if(UseSerializer(value))
                {
                    resp = JsonConvert.SerializeObject(value);
                    writer.WriteRawValue(resp);
                }
                else
                    writer.WriteValue(value);

                return;
            }
            var dataToConv = value.GetType().GetProperties();
                       

            writer.WriteStartObject();
            foreach(var elementToSerialize in dataToConv)
            {
                writer.WritePropertyName(elementToSerialize.Name);
                //elementToSerialize.

                var objType = elementToSerialize.GetValue(value, null);
                if(UseSerializer(objType))
                {
                    var newtonSerializationSet = new JsonSerializerSettings
                                                     {NullValueHandling = serializer.NullValueHandling};
                    
                    newtonSerializationSet.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
                    newtonSerializationSet.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                    resp = JsonConvert.SerializeObject(elementToSerialize.GetValue(value, null), writer.Formatting, newtonSerializationSet);
                    /*if(resp.StartsWith("\""))
                    {
                        resp = resp.Remove(0);
                        resp = resp.Remove(resp.Length - 1);
                    }*/
                    writer.WriteRawValue(resp);
                }
                else
                    writer.WriteValue(objType);
                //writer.WriteValue("teste");
            }

            //finalizo a conversao
            writer.WriteEndObject();


        }
        private bool UseSerializer(object objToSerialize)
        {
            string name = objToSerialize.GetType().Name.ToLower();

            switch(name)
            {
                case "bool":
                case "bool?":
                case "byte":
                case "byte?":
                case "byte[]":
                case "char":
                case "char?":
                case "DateTime":
                case "DateTime?":
                case "DateTimeOffset":
                case "DateTimeOffset?":
                case "decimal":
                case "decimal?":
                case "double":
                case "double?":
                case "float":
                case "float?":
                case "int":
                case "int16":
                case "int32":
                case "int64":
                case "int?":
                case "long":
                case "long?":
                case "sbyte":
                case "sbyte?":
                case "short":
                case "short?":
                case "string":
                case "uint":
                case "uint16":
                case "uint32":
                case "uint64":
                case "uint?":
                case "ulong":
                case "ulong?":
                case "ushort":
                case "ushort?":
                    return false;
            }

            return true;
        }
        /// <summary>
        /// <para>Determines whether this instance can convert the specified object type.</para>
        /// </summary>
        /// <param name="objectType">
        /// <para>Type of the object.</para>
        /// </param>
        /// <returns>
        /// <para>true if can convert the type informed.</para>
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return (objectType.GetInterface("IEnumerable") != null && objectType.GetProperties().Count() > 3 && !objectType.Name.StartsWith("List") && !objectType.Name.StartsWith("Dictionary") && !objectType.Name.ToLower().StartsWith("string") && objectType.Name.ToLower().Equals("expandoobject") == false);
        }
        /// <summary>
        /// <para> Gets a value indicating whether this Newtonsoft.Json.JsonConverter can write Json.</para>
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }
    }
}
