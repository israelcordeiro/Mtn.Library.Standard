using System;


namespace Mtn.Library.Interface
{
    /// <summary>
    /// interface for Json Provider 
    /// </summary>
    public interface IJsonProvider
    {
        /// <summary>
        /// Serialize method
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ignoreNullData"></param>
        /// <param name="indented"></param>
        /// <returns></returns>
         String Serialize(Object value,Boolean ignoreNullData,Boolean indented = true);
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
         Object DeSerialize(String value, Type type = null);         
        
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
         T DeSerialize<T>(String value);         
    }

    /// <summary>
    /// Provider for JSOn using JavaScriptSerializer
    /// </summary>
    //public class MicrosoftJsonProvider : IJsonProvider
    //{

    //    #region IJsonProvider Members
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public string Serialize(object value, bool ignoreNullData, bool indented = true)
    //    {
    //        var js = new JavaScriptSerializer();
    //        var data = js.Serialize(value);
    //        return data;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public object DeSerialize(string value, Type type = null)
    //    {
    //        var js = new JavaScriptSerializer();
    //        var data = js.DeserializeObject(value);
    //        return data;
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public T DeSerialize<T>(string value)
    //    {
    //        return (T)DeSerialize(value, typeof(T));
    //    }

    //    #endregion
        
    //}

}
