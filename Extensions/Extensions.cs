using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Reflection;
using System.ComponentModel;

using Mtn.Library.Configuration;
using Mtn.Library.Entities;
using Mtn.Library.Interface;


namespace Mtn.Library.Extensions
{
    /// <summary>
    /// Extensions for System common objects, like String and Object
    /// </summary>
    public static class SystemExtensions
    {
        #region String

        #region Parse

        /// <summary>
        /// <para>Convert the string to boolean.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Boolean ToBooleanMtn(this string value, Boolean defaultValue = false, Boolean throwException = false)
        {
            Boolean result;
            if (throwException)
                return Boolean.Parse(value);

            Boolean.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// <para>Convert the string to Int32.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Int32 ToInt32Mtn(this string value, Int32 defaultValue = 0, Boolean throwException = false)
        {
            Int32 result;
            if (throwException)
                return Int32.Parse(value);

            Int32.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// <para>Convert the string to Int16.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Int16 ToInt16Mtn(this string value, Int16 defaultValue = 0, Boolean throwException = false)
        {
            Int16 result;
            if (throwException)
                return Int16.Parse(value);

            Int16.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// <para>Convert the string to Int64.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Int64 ToInt64Mtn(this string value, Int64 defaultValue = 0, Boolean throwException = false)
        {
            Int64 result;
            if (throwException)
                return Int64.Parse(value);

            Int64.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// <para>Convert the string to Decimal.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Decimal ToDecimalMtn(this string value, Decimal defaultValue = 0, Boolean throwException = false)
        {
            Decimal result;
            if (throwException)
                return Decimal.Parse(value);

            Decimal.TryParse(value, out result);
            return result;
        }

        /// <summary>
        /// <para>Convert the string to Single.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Single ToSingleMtn(this string value, Single defaultValue = 0, Boolean throwException = false)
        {
            Single result;
            if (throwException)
                return Single.Parse(value);

            Single.TryParse(value, out result);
            return result;
        }
        /// <summary>
        /// <para>Convert the string to Double.</para>
        /// </summary>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <param name="defaultValue">default value for result </param>
        /// <param name="throwException">Indicates if will throw exception when don't cast </param>
        /// <returns>
        /// <para>Returns boolean converto or default result .</para>
        /// </returns>
        public static Double ToDoubleMtn(this string value, Double defaultValue = 0, Boolean throwException = false)
        {
            Double result;
            if (throwException)
                return Double.Parse(value);

            Double.TryParse(value, out result);
            return result;
        }
        #endregion

        private static readonly Regex PvHtmlTagMtn = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Trim String, if null return "".
        /// </summary>
        /// <param name="value">
        /// Object to be converted to String.
        /// </param>
        /// <returns>
        /// Returns the String with trim, if null return "".
        /// </returns>
        public static String ToTrimStringMtn(this Object value)
        {
            if (value == null)
                return "";
            return value.ToString().Trim();
        }

        /// <summary>
        /// Returns MD5 hash from String.
        /// </summary>
        /// <param name="value">
        /// String to generate the hash.
        /// </param>
        /// <param name="base64">
        /// force returns in base 64 mode
        /// </param>
        /// <returns>
        /// Returns MD5 hash from String.
        /// </returns>
        public static String Md5Mtn(this String value, Boolean base64 = false)
        {
            return !base64
                       ? Encoding.UTF8.GetString(
                           System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value)))
                       : HttpUtility.UrlEncode(
                           System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        /// <summary>
        /// Returns MD5 hash from object (transform in JSON first).
        /// </summary>
        /// <param name="value">Object to generate the hash.</param>
        /// <param name="base64">force returns in base 64 mode instead UTF8</param>
        /// <param name="jsonProvider">The json provider.</param>
        /// <returns>
        /// Returns MD5 hash from object (transform in JSON first).
        /// </returns>
        public static String Md5JsonMtn(this Object value, Boolean base64 = false,
                                        IJsonProvider jsonProvider = null)
        {
            String data = ToJsonMtn(value, jsonProvider);

            Byte[] arrByte = Encoding.UTF8.GetBytes(data);
            Byte[] arrByteHash = System.Security.Cryptography.MD5.Create().ComputeHash(arrByte);
            data = base64 ? HttpUtility.UrlEncode(arrByteHash) : Encoding.UTF8.GetString(arrByteHash);

            return data;
        }

        #region Encrypt AES
        /// <summary>
        /// Encrypt AES using cipherMode and keys
        /// </summary>
        /// <param name="value">string value</param>
        /// <param name="cipherMode">CipherMode mode </param>
        /// <param name="ivKey">IV key, must be at least 8 bytes</param>
        /// <param name="cipherKey">Key to cipher, must be at least 1 byte</param>
        /// /// <param name="encoding">Default is UTF8</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static String EncryptAesMtn(this String value, CipherMode cipherMode = CipherMode.CBC, String ivKey = null, String cipherKey = null, Encoding encoding = null)
        {

            try
            {
                if (value.IsNullOrWhiteSpaceMtn())
                    return value;

                if ((!ivKey.IsNullOrWhiteSpaceMtn() && ivKey.Length < 8) | (!cipherKey.IsNullOrWhiteSpaceMtn() && cipherKey.Length < 1))
                    throw new ArgumentOutOfRangeException("ivKey must have at least 8 bytes and cipherkey must have at least 1 byte");


                var aesMan = new AesManaged {Mode = cipherMode};

                
                if (!ivKey.IsNullOrWhiteSpaceMtn() && !cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var p = new Rfc2898DeriveBytes(cipherKey, ivKey.ToByteArrayMtn());
                    aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                    aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                }
                else if (!ivKey.IsNullOrWhiteSpaceMtn() && cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var keyFromConfig = Config.GetString("Mtn.Library.Crypt.Key");
                    if (!keyFromConfig.IsNullOrWhiteSpaceMtn())
                    {
                        cipherKey = keyFromConfig;
                        var p = new Rfc2898DeriveBytes(cipherKey, ivKey.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }
                }
                else if (ivKey.IsNullOrWhiteSpaceMtn() && !cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var ivkeyFromConfig = Config.GetString("Mtn.Library.Crypt.IV");
                    if (!ivkeyFromConfig.IsNullOrWhiteSpaceMtn())
                    {
                        var p = new Rfc2898DeriveBytes(cipherKey, ivkeyFromConfig.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }
                }
                else if(ivKey.IsNullOrWhiteSpaceMtn() && cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var ivkeyFromConfig = Config.GetString("Mtn.Library.Crypt.IV");
                    var keyFromConfig = Config.GetString("Mtn.Library.Crypt.Key");
                    if (!ivkeyFromConfig.IsNullOrWhiteSpaceMtn() && !keyFromConfig.IsNullOrWhiteSpaceMtn())
                    {
                        var p = new Rfc2898DeriveBytes(keyFromConfig, ivkeyFromConfig.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize/8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize/8);
                    }
                    else
                    {
                        var p = new Rfc2898DeriveBytes(cipherKey, ivKey.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }

                }

                var arrByte = value.ToByteArrayMtn(encoding);
                Byte[] arrByteHash;


                ICryptoTransform encryptor = aesMan.CreateEncryptor(aesMan.Key, aesMan.IV);

                // Create the streams used for encryption. 
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(value);
                            //csEncrypt.FlushFinalBlock();
                        }
                        arrByteHash = msEncrypt.ToArray();
                    }
                }
                var data = HttpUtility.UrlEncode(arrByteHash);

                return data;
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
                throw ex;
            }
        }

        /// <summary>
        /// Decript AES using cipherMode and keys
        /// </summary>
        /// <param name="value">string value</param>
        /// <param name="cipherMode">CipherMode mode </param>
        /// <param name="ivKey">IV key, must be at least 8 bytes</param>
        /// <param name="cipherKey">Key to cipher, must be at least 1 byte</param>
        /// <param name="encoding">Default is UTF8</param>
        /// <returns></returns>
        public static String DecryptAesMtn(this String value, CipherMode cipherMode = CipherMode.CBC, String ivKey = null, String cipherKey = null, Encoding encoding = null)
        {
            try
            {
                if (value.IsNullOrWhiteSpaceMtn())
                    return value;

                if ((!ivKey.IsNullOrWhiteSpaceMtn() && ivKey.Length < 8) | (!cipherKey.IsNullOrWhiteSpaceMtn() && cipherKey.Length < 1))
                    throw new ArgumentOutOfRangeException("ivKey must have at least 8 bytes and cipherkey must have at least 1 byte");

                var aesMan = new AesManaged();
                aesMan.Padding = PaddingMode.PKCS7;
                aesMan.Mode = cipherMode;
                if (!ivKey.IsNullOrWhiteSpaceMtn() && !cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var p = new Rfc2898DeriveBytes(cipherKey, ivKey.ToByteArrayMtn());
                    aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                    aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                }
                else if (!ivKey.IsNullOrWhiteSpaceMtn() && cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var keyFromConfig = Config.GetString("Mtn.Library.Crypt.Key");
                    if (!keyFromConfig.IsNullOrWhiteSpaceMtn())
                    {
                        cipherKey = keyFromConfig;
                        var p = new Rfc2898DeriveBytes(cipherKey, ivKey.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }
                }
                else if (ivKey.IsNullOrWhiteSpaceMtn() && !cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var ivkeyFromConfig = Config.GetString("Mtn.Library.Crypt.IV");
                    if (!ivkeyFromConfig.IsNullOrWhiteSpaceMtn())
                    {
                        var p = new Rfc2898DeriveBytes(cipherKey, ivkeyFromConfig.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }

                }
                else if (ivKey.IsNullOrWhiteSpaceMtn() && cipherKey.IsNullOrWhiteSpaceMtn())
                {
                    var ivkeyFromConfig = Config.GetString("Mtn.Library.Crypt.IV");
                    var keyFromConfig = Config.GetString("Mtn.Library.Crypt.Key");
                    if (!ivkeyFromConfig.IsNullOrWhiteSpaceMtn() && !keyFromConfig.IsNullOrWhiteSpaceMtn())
                    {
                        var p = new Rfc2898DeriveBytes(keyFromConfig, ivkeyFromConfig.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }
                    else
                    {
                        var p = new Rfc2898DeriveBytes(cipherKey, ivKey.ToByteArrayMtn());
                        aesMan.IV = p.GetBytes(aesMan.BlockSize / 8);
                        aesMan.Key = p.GetBytes(aesMan.KeySize / 8);
                    }
                }

                var arrByte = HttpUtility.UrlEncode(value).ToByteArrayMtn();
                string retValue;

                // Create a decrytor to perform the stream transform.
                var decryptor = aesMan.CreateDecryptor(aesMan.Key, aesMan.IV);

                // Create the streams used for decryption. 
                using (var msDecrypt = new MemoryStream(arrByte))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            retValue = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return retValue;
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
                throw ex;
            }
        }
        #endregion

        #region Bytes and base 64
        /// <summary>
        /// Convert string to byte array 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns>Empty array if null or empty, otherwise convert to bytes using encodig</returns>
        public static Byte[] ToByteArrayMtn(this String value, Encoding encoding = null)
        {
            if (value.IsNullOrWhiteSpaceMtn()) return new byte[] { };

            if (encoding == null)
                encoding = Encoding.UTF8;

            return encoding.GetBytes(value);
        }

        /// <summary>
        /// Convert byte array to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns>Empty array if null or empty, otherwise convert to bytes using encodig</returns>
        public static String ToStringMtn(this Byte[] value, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return (value == null || value.Length == 0) ? String.Empty : encoding.GetString(value);
        }

        /// <summary>
        /// Convert to base 64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToBase64Mtn(this String value)
        {
            return Convert.ToBase64String(value.ToByteArrayMtn());
        }

        /// <summary>
        /// Convert from base 64
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String FromBase64Mtn(this String value)
        {

            return Convert.FromBase64String(value).ToStringMtn();
        }

        #endregion

        /// <summary>
        /// Converts a object to Json string.
        /// </summary>
        /// <param name="value">Object to be serialized to Json.</param>
        /// <param name="jsonProvider">The json provider.</param>
        /// <param name="ignoreNullData">
        /// <para>Identify if ignore null data.(only for JsonNet provider)</para>
        /// </param>
        /// <param name="indented">
        /// <para>Identify if ident the data.(only for JsonNet provider)</para>
        /// </param>
        /// <returns>
        /// <para>Returns the Json string.</para>
        /// </returns>
        public static String ToJsonMtn(this Object value, IJsonProvider jsonProvider = null, Boolean ignoreNullData = false, Boolean indented = true)
        {

            if (jsonProvider == null)
            {
                jsonProvider = Core.Parameter.JsonProvider;
            }

            var data = jsonProvider.Serialize(value: value, ignoreNullData: ignoreNullData, indented: indented);
            return data;
        }


        /// <summary>
        /// Generates a object based on Json String.
        /// </summary>
        /// <typeparam name="T">Represents the object type for return.</typeparam>
        /// <param name="value">Json String.</param>
        /// <param name="jsonProvider">The json provider.</param>
        /// <returns>
        /// Object based on Json String.
        /// </returns>
        public static T ToObjectFromJsonMtn<T>(this String value, IJsonProvider jsonProvider = null)
        {
            return (T)ToObjectFromJsonMtn(value, typeof(T), jsonProvider);
        }


        /// <summary>
        /// Generates a typed object based on Json String.
        /// </summary>
        /// <param name="value">Json String.</param>
        /// <param name="type">Type of object to be converted.</param>
        /// <param name="jsonProvider">The json provider.</param>
        /// <returns>
        /// Object based on Json String.
        /// </returns>
        public static Object ToObjectFromJsonMtn(this String value, Type type = null, IJsonProvider jsonProvider = null)
        {
            if (jsonProvider == null)
            {
                jsonProvider = Mtn.Library.Core.Parameter.JsonProvider;
            }

            var data = jsonProvider.DeSerialize(value, type);
            return data;
        }
        /// <summary>
        /// Generates a typed object based on Json String.
        /// </summary>
        /// <param name="value">Json String.</param>
        /// <param name="jsonProvider">The json provider.</param>
        /// <returns>
        /// Object based on Json String.
        /// </returns>
        public static T ToTypedFromJsonMtn<T>(this String value, IJsonProvider jsonProvider = null)
        {
            if (jsonProvider == null)
            {
                jsonProvider = Mtn.Library.Core.Parameter.JsonProvider;
            }

            var data = jsonProvider.DeSerialize<T>(value);
            return data;
        }



        /// <summary>
        /// Check if String is null or empty.
        /// </summary>
        /// <param name="value">
        /// String to be checked.
        /// </param>
        /// <param name="trimString">
        /// Indicates if ignore spaces.
        /// </param>
        /// <returns>
        /// Returns true if is null or empty.
        /// </returns>
        public static Boolean IsNullOrEmptyMtn(this String value, Boolean trimString = true)
        {
            if (String.IsNullOrEmpty(value))
                return true;

            if (trimString == false)
                return false;

            return (value.Trim().Length == 0);
        }

        /// <summary>
        /// Check if String is null or empty.
        /// </summary>
        /// <param name="value">
        /// String to be checked.
        /// </param>
        /// <returns>
        /// Returns true if is null or empty.
        /// </returns>
        public static Boolean IsNullOrWhiteSpaceMtn(this String value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return true;
            return false;
        }

        /// <summary>
        /// Returns a String array that contains the subStrings in this String that are delimited by delimiter.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="delimiter">
        /// Delimiter.
        /// </param>
        /// <param name="options">
        /// Options .
        /// </param>
        /// <returns>
        /// Returns a String array that contains the subStrings in this String that are delimited by delimiter.
        /// </returns>
        public static String[] SplitMtn(this String value, Char delimiter, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            if (value.IsNullOrEmptyMtn(true))
                return new String[] { };

            return value.Split(new Char[] { delimiter }, options);
        }

        /// <summary>
        /// Returns a String array that contains the subStrings in this String that are delimited by delimiter.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="delimiter">
        /// Delimiter.
        /// </param>
        /// <param name="options">
        /// Options .
        /// </param>
        /// <returns>
        /// Returns a String array that contains the subStrings in this String that are delimited by delimiter.
        /// </returns>
        public static String[] SplitMtn(this String value, String delimiter, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            if (value.IsNullOrEmptyMtn(true))
                return new String[] { };

            return value.Split(new String[] { delimiter }, options);
        }

        /// <summary>
        /// Converts a String that has been HTML-encoded for HTTP transmission into a decoded String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns a decoded String.
        /// </returns>
        public static String HtmlDecodeMtn(this String value)
        {
            return HttpUtility.HtmlDecode(value);
        }

        /// <summary>
        /// Converts a String to an HTML-encoded String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns an HTML-encoded String.
        /// </returns>
        public static String HtmlEncodeMtn(this String value)
        {
            return HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Remove all html tags in String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns a String without html tags.
        /// </returns>
        public static String HtmlRemoveMtn(this String value)
        {
            return PvHtmlTagMtn.Replace(value, String.Empty);
        }

        /// <summary>
        /// Removes spaces and tabs of String around the String are indicated by subString.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="subString">
        /// Indicates the subString to remove white space surround it.
        /// </param>
        /// <returns>
        /// Returns the String without spaces and tabs around the String are indicated by subString.
        /// </returns>
        public static String RemoveSurroundingWhitespaceMtn(this String value, String subString)
        {
            String retVal = value;

            if (retVal.Contains(subString.ConcatMtn(" ")) || retVal.Contains(subString.ConcatInverseMtn(" ")))
            {
                retVal = retVal.Replace(subString.ConcatMtn(" "), subString);
                retVal = retVal.Replace(subString.ConcatInverseMtn(" "), subString);
                retVal = RemoveSurroundingWhitespaceMtn(retVal, subString);
            }

            if (retVal.Contains(subString.ConcatMtn("\t")) || retVal.Contains(subString.ConcatInverseMtn("\t")))
            {
                retVal = retVal.Replace(subString.ConcatMtn("\t"), subString);
                retVal = retVal.Replace(subString.ConcatInverseMtn("\t"), subString);
                retVal = RemoveSurroundingWhitespaceMtn(retVal, subString);
            }

            return retVal;
        }

        /// <summary>
        /// Returns the number (size) of characters on the left.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="size">
        /// Number of characters to return.
        /// </param>
        /// <returns>
        /// Returns the number (size) of characters on the left.
        /// </returns>
        public static String LeftMtn(this String value, Int32 size)
        {
            if (value.IsNullOrWhiteSpaceMtn())
                return value;
            var l = value.Length;
            l = l < size ? l : size;
            return value.Substring(0, l);
        }


        /// <summary>
        /// Concatenates the value at the beginning of the String.
        /// </summary>
        /// <param name="source">
        /// String.
        /// </param>
        /// <param name="value">
        /// String to concatenate.
        /// </param>
        /// <returns>
        /// Returns String with the value at the beginning of the String.
        /// </returns>
        public static String ConcatInverseMtn(this String source, String value)
        {
            return String.Concat(value, source);
        }


        /// <summary>
        /// Concatenates the value at the end of the String.
        /// </summary>
        /// <param name="source">
        /// String.
        /// </param>
        /// <param name="value">
        /// String to concatenate.
        /// </param>
        /// <returns>
        /// Returns String with the value at the end of the String.
        /// </returns>
        public static String ConcatMtn(this String source, String value)
        {
            return String.Concat(source, value);
        }

        /// <summary>
        /// Unescapes any escaped characters in the input String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Unescaped String.
        /// </returns>
        public static String RegexUnescapeMtn(this String value)
        {
            return Regex.Unescape(value);
        }

        /// <summary>
        /// Escapes a minimal set of metacharacters (\, *, +, ?, |, {, [, (,), ^, $,.,#, and white space) by replacing them with their escape codes.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Escaped String.
        /// </returns>
        public static String RegexEscapeMtn(this String value)
        {
            return Regex.Escape(value);
        }

        /// <summary>
        /// Returns the number (size) of characters on the right.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="size">
        /// Number of characters to return.
        /// </param>
        /// <returns>
        /// Returns the number (size) of characters on the right.
        /// </returns>
        public static String RightMtn(this String value, Int32 size)
        {
            if (value.IsNullOrWhiteSpaceMtn())
                return value;
            var l = value.Length;
            l = l < size ? l : size;

            return value.Substring(value.Length - l, l);
        }

        /// <summary>
        /// Save the String contents in file.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="filePath">
        /// File path.
        /// </param>
        /// <param name="rethrow">
        /// Indicates if must rethrow exception.
        /// </param>
        /// <param name="fileMode"> </param>
        /// <param name="writeline"> </param>
        /// <param name="encoding"></param>
        /// <returns>
        /// Returns true on sucess.
        /// </returns>
        public static Boolean ToFileMtn(this String value, String filePath, Boolean rethrow = true, FileMode fileMode = FileMode.OpenOrCreate, Boolean writeline = false, Encoding encoding = null)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;

            try
            {
                if (encoding == null)
                    encoding = new UTF8Encoding();

                Debug.Assert(filePath != null, "filePath != null");
                if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
                {
                    Directory.CreateDirectory(filePath);
                }

                fileStream = new FileStream(filePath, fileMode, FileAccess.Write);
                streamWriter = new StreamWriter(fileStream, encoding);
                if (writeline)
                    streamWriter.WriteLine(value);
                else
                    streamWriter.Write(value);

                streamWriter.Flush();
                return true;
            }
            catch
            {
                if (rethrow)
                    throw;
                return false;
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        public static string ReadFileMtn(this String filePath, Encoding encoding = null, Boolean rethrow = true)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            const FileMode fileMode = FileMode.Open;
            try
            {
                if (encoding == null)
                    encoding = new UTF8Encoding();

                Debug.Assert(filePath != null, "filePath != null");

                fileStream = new FileStream(filePath, fileMode, FileAccess.Read);
                streamReader = new StreamReader(fileStream, encoding);

                return streamReader.ReadToEnd();
            }
            catch
            {
                if (rethrow)
                    throw;
                return null;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rethrow"></param>
        /// <returns></returns>
        public static byte[] ReadBinaryFileMtn(this String filePath, Boolean rethrow = true)
        {
            FileStream fileStream = null;
            MemoryStream ms = new MemoryStream();
            const FileMode fileMode = FileMode.Open;
            try
            {
                Debug.Assert(filePath != null, "filePath != null");

                fileStream = new FileStream(filePath, fileMode, FileAccess.Read);
                fileStream.CopyTo(ms);

                return ms.ToArray();
            }
            catch
            {
                if (rethrow)
                    throw;
                return null;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }


        /// <summary>
        /// Save the String contents in file as binary file.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="filePath">
        /// File path.
        /// </param>
        /// <param name="rethrow">
        /// Indicates if must rethrow exception.
        /// </param>
        /// <returns>
        /// Returns true on sucess.
        /// </returns>
        public static Boolean ToFileBinaryMtn(this String value, String filePath, Boolean rethrow = true)
        {
            var asen = new ASCIIEncoding();
            byte[] ba = asen.GetBytes(value);
            ba.ToFileBinaryMtn(filePath: filePath, rethrow: rethrow);
            return true;
        }

        /// <summary>
        /// Save the bytes in file as binary file.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <param name="filePath">
        /// File path.
        /// </param>
        /// <param name="rethrow">
        /// Indicates if must rethrow exception.
        /// </param>
        /// <returns>
        /// Returns true on sucess.
        /// </returns>
        public static Boolean ToFileBinaryMtn(this Byte[] value, String filePath, Boolean rethrow = true)
        {
            FileStream fileStream = null;
            BinaryWriter binaryWriter = null;

            try
            {
                Debug.Assert(filePath != null, "filePath != null");
                if (Directory.Exists(Path.GetDirectoryName(filePath)) == false)
                {
                    Directory.CreateDirectory(filePath);
                }
                var asen = new ASCIIEncoding();

                fileStream = new FileStream(filePath, FileMode.Create);
                binaryWriter = new BinaryWriter(fileStream);
                binaryWriter.Write(value);
                binaryWriter.Flush();
                return true;
            }
            catch
            {
                if (rethrow)
                    throw;
                return false;
            }
            finally
            {
                if (binaryWriter != null)
                {
                    binaryWriter.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
        /// <summary>
        /// Return a string returning a empty string if is null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToSafeStringMtn(this String value)
        {
            return value == null ? String.Empty : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static String FormatMtn(this String value, params object[] args)
        {
            return string.Format(value, args);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="arg"></param>
        /// <param name="comparison"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        public static String FormatByPropertyMtn(this String value, object arg, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase, String startTag = "{", String endTag = "}")
        {
            var ret = value;
            var retL = value.ToLowerInvariant();
            if (arg == null || value.IsNullOrWhiteSpaceMtn())
                return value;

            foreach (var prop in arg.GetType().GetProperties())
            {
                var name = prop.Name;
                var fmtName = startTag + name + endTag;

                if (!retL.Contains(fmtName.ToLowerInvariant()))
                    continue;

                var valProp = arg.GetType().GetProperty(prop.Name).GetValue(arg, null);
                var safeVal = valProp == null ? "" : valProp.ToString();
                ret = ret.ReplaceFastMtn(fmtName, safeVal, comparison);

            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        public static String FormatByStringMtn<T>(this T arg, String value, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase, String startTag = "{", String endTag = "}")
        {
            return value.FormatByPropertyMtn(arg: arg, comparison: comparison, startTag: startTag, endTag: endTag);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="regexOptions"></param>
        /// <param name="escape"></param>
        /// <returns></returns>
        public static String RegexReplaceMtn(this String value, String oldValue, String newValue, RegexOptions regexOptions = RegexOptions.IgnoreCase, bool escape = false)
        {
            return Regex.Replace(value, escape ? oldValue.RegexEscapeMtn() : oldValue, escape ? newValue.RegexEscapeMtn() : newValue, regexOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string ReplaceFastMtn(this String value, String oldValue, String newValue, StringComparison comparison = StringComparison.CurrentCultureIgnoreCase)
        {
            var sb = new StringBuilder();
            var posInit = 0;
            var pos = value.IndexOf(oldValue, comparison);
            while (pos != -1)
            {
                sb.Append(value.Substring(posInit, pos - posInit));
                sb.Append(newValue);
                pos += oldValue.Length;

                posInit = pos;
                pos = value.IndexOf(oldValue, pos, comparison);
            }
            sb.Append(value.Substring(posInit));

            return sb.ToString();
        }
        /// <summary>
        /// Return a enumerable using the characters "\r\n" as delimiter.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Return a enumerable using the characters "\r\n" as delimiter.
        /// </returns>
        public static IEnumerable<String> ToLinesMtn(this String value)
        {
            if (value.IsNullOrEmptyMtn(true) == false)
            {
                return value.Split(new String[] { "\r\n", Environment.NewLine }, StringSplitOptions.None);
            }
            else
            {
                return new String[0];
            }
        }

        /// <summary>
        /// Encodes a URL String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns a encoded String.
        /// </returns>
        public static String UrlEncodeMtn(this String value)
        {
            return HttpUtility.UrlEncode(value);
        }

        /// <summary>
        /// Converts a String that has been encoded for transmission in a URL into a decoded String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns a decoded String.
        /// </returns>
        public static String UrlDecodeMtn(this String value)
        {
            return HttpUtility.UrlDecode(value);
        }


        /// <summary>
        /// Remove double space from String.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns the String without double space.
        /// </returns>
        public static String RemoveDoubleSpaceMtn(this String value)
        {
            String retVal = value;

            while (retVal.Contains("  "))
            {
                retVal = retVal.Replace("  ", " ");
            }
            return retVal;
        }


        /// <summary>
        /// Counts the total number of words separated by space.
        /// </summary>
        /// <param name="value">
        /// String.
        /// </param>
        /// <returns>
        /// Returns the total number of words separated by space.
        /// </returns>
        public static Int32 TotalWordMtn(this String value)
        {
            return value.RemoveDoubleSpaceMtn().SplitMtn(' ').Count();
        }

        #endregion

        #region isPrimitive
        /// <summary>
        /// Return true if is a primitive type.
        /// </summary>
        /// <param name="value">object to be checked.</param>
        /// <returns>
        /// Return true if is a primitive type.
        /// </returns>
        public static Boolean IsPrimitiveMtn(this Object value)
        {
            if (value == null)
                return false;

            return IsPrimitiveMtn(value.GetType());
        }
        /// <summary>
        ///  Return true if is a primitive type.(Mtn)
        /// </summary>
        /// <param name="type">
        /// Type to be checked.
        ///</param>
        /// <returns>
        ///  Return true if is a primitive type.
        /// </returns>
        public static Boolean IsPrimitiveMtn(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///  Return true if is a primitive type.(Mtn)
        /// </summary>
        /// <param name="typeName">
        /// Name of type to be checked.
        ///</param>
        /// <returns>
        ///  Return true if is a primitive type.
        /// </returns>
        public static Boolean IsPrimitiveMtn(String typeName)
        {
            switch (typeName.ToLower())
            {
                case "byte":
                case "int16":
                case "int32":
                case "int64":
                case "uint16":
                case "uint32":
                case "uint64":
                case "decimal":
                case "single":
                case "double":
                case "boolean":
                case "char":
                case "datetime":
                case "String":
                    return true;
                default:
                    return false;
            }

        }
        #endregion

        #region Enum

        /// <summary>
        /// <para>Convert the string to enum.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="value">
        /// <para>The string to be checked.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the enum object .</para>
        /// </returns>
        public static TEntity ToEnumMtn<TEntity>(this string value) where TEntity : struct
        {
            return (TEntity)Enum.Parse(typeof(TEntity), value, true);
        }

        /// <summary>
        /// <para>Return enum value.</para>
        /// </summary>
        /// <param name="value">
        /// <para>Enum object.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the enum value .</para>
        /// </returns>
        [ObsoleteAttribute("This property is obsolete. Use GetValueMtn instead.", false)] 
        public static TEntity GetValue<TEntity>(this Enum value)
        {
            return GetValueMtn<TEntity>(value);
        }

        /// <summary>
        /// <para>Return enum value.</para>
        /// </summary>
        /// <param name="value">
        /// <para>Enum object.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the enum value .</para>
        /// </returns>
        public static TEntity GetValueMtn<TEntity>(this Enum value)
        {
            return (TEntity)Convert.ChangeType(value, typeof(TEntity));
        }

        /// <summary>
        /// <para>Return enum description using System.ComponentModel.DescriptionAttribute .</para>
        /// </summary>
        /// <param name="value">
        /// <para>Enum object.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the enum description .</para>
        /// </returns>
        [ObsoleteAttribute("This property is obsolete. Use GetEnumDescriptionMtn instead.", false)] 
        public static String GetEnumDescription(this Enum value)
        {
            return GetEnumDescriptionMtn(value);
        }

        /// <summary>
        /// <para>Return enum description using System.ComponentModel.DescriptionAttribute .</para>
        /// </summary>
        /// <param name="value">
        /// <para>Enum object.</para>
        /// </param>
        /// <returns>
        /// <para>Returns the enum description .</para>
        /// </returns>
        public static String GetEnumDescriptionMtn(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(name: value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }


        #endregion

        #region Guid

        /// <summary>
        /// <para>Check if Guid is null or empty.</para>
        /// </summary>
        /// <param name="guid">
        /// <para>The guid to be checked.</para>
        /// </param>
        /// <returns>
        /// <para>Returns true if is null or empty.</para>
        /// </returns>
        public static Boolean IsNullOrEmptyMtn(this Guid guid)
        {
            return guid == null || guid == Guid.Empty;
        }
        #endregion

        #region Enumerable
        /// <summary>
        /// <para>Check if IEnumerable is null or empty.</para>
        /// </summary>
        /// <typeparam name="TEntity">
        /// <para>Entity class.</para>
        /// </typeparam>
        /// <param name="value">
        /// <para>Enumerable to be checked.</para>
        /// </param>
        /// <returns>
        /// <para>Returns true if is null or empty.</para>
        /// </returns>
        public static bool IsNullOrEmptyMtn<TEntity>(this IEnumerable<TEntity> value)
        {
            return value == null || !value.Any();
        }
        #endregion

        #region Exception
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="stackTrace"></param>
        /// <param name="breakCode"></param>
        /// <param name="ignoreUnhandledException"></param>
        /// <returns></returns>
        [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", false)]
        public static string GetAllMessages(this Exception exception, StackTrace stackTrace = null,
            string breakCode = "\r\n", bool ignoreUnhandledException = true)
        {
            return GetAllMessagesMtn(exception: exception, stackTrace: stackTrace, breakCode:breakCode,ignoreUnhandledException:ignoreUnhandledException);
        }

        /// <summary>
        /// Get inner excemption and other data to debug like domain, browser, etc
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="stackTrace"></param>
        /// <param name="breakCode"></param>
        /// <param name="ignoreUnhandledException"></param>
        /// <returns></returns>
        public static string GetAllMessagesMtn(this Exception exception, StackTrace stackTrace = null, string breakCode = "\r\n", bool ignoreUnhandledException = true)
        {

            if (exception == null)
                return string.Empty;

            var strExp = new StringBuilder();
            var auxiliarException = exception;


            //if (System.Web.Hosting.HostingEnvironment.IsHosted)
            //{
            //    var current = HttpContext.Current;
            //    if (current != null)
            //    {
            //        var request = current.Request;
            //        strExp.AppendFormat("{6}URL: {0}{6}Referer: {1}{6}Domain: {2}{6}Browser: {3}{6}Server Address: {4}{6}Client Address: {5}{6}", request.RawUrl, request.UrlReferrer, request.Url.DnsSafeHost, request.UserAgent, request.ServerVariables["LOCAL_ADDR"], (object)request.UserHostAddress, breakCode);
            //    }
            //}

            strExp.AppendFormat("\rType: {0}", (object)auxiliarException.GetType().Name);
            if (ignoreUnhandledException)
            {

                while (auxiliarException != null)
                    auxiliarException = auxiliarException.InnerException;

            }
            for (; auxiliarException != null; auxiliarException = auxiliarException.InnerException)
            {
                strExp.Append(auxiliarException.Message);
                strExp.Append(breakCode);
                if (stackTrace != null)
                {
                    strExp.Append(((object)stackTrace));
                    stackTrace = null;
                }
                else if (auxiliarException.StackTrace != null)
                    strExp.Append(auxiliarException.StackTrace);
                strExp.Append(breakCode + breakCode);
            }
            return (strExp).ToString();
        }

        #endregion

        #region DateTime

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static bool IsLaterOrEqualThanCurrentMtn(this DateTime value, DoubleDateInfo.FormatType formatType)
        {
            var dateInfo = new DoubleDateInfo(formatType);

            return (DateTime.Compare(value, dateInfo.CurrentChanged) >= 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static bool IsEarlierOrEqualThanCurrentMtn(this DateTime value, DoubleDateInfo.FormatType formatType)
        {
            var dateInfo = new DoubleDateInfo(formatType);

            return (DateTime.Compare(value, dateInfo.CurrentChanged) <= 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static bool IsLaterThanCurrentMtn(this DateTime value, DoubleDateInfo.FormatType formatType)
        {
            var dateInfo = new DoubleDateInfo(formatType);

            return (DateTime.Compare(value, dateInfo.CurrentChanged) > 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="formatType"></param>
        /// <returns></returns>
        public static bool IsEarlierThanCurrentMtn(this DateTime value, DoubleDateInfo.FormatType formatType)
        {
            var dateInfo = new DoubleDateInfo(formatType);

            return (DateTime.Compare(value, dateInfo.CurrentChanged) < 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOnMonthMtn(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOnYearMtn(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }

        /// <summary>
        /// Get number of weeks
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int GetNumberOfWeeksMtn(this DateTime start, DateTime end)
        {
            var diff = end.Subtract(start);

            if (diff.Days <= 7)
            {
                return start.DayOfWeek > end.DayOfWeek ? 2 : 1;
            }

            var totalDays = diff.Days - 7 + (int)start.DayOfWeek;
            var weekCount = 1;
            var dayCount = 0;

            for (weekCount = 1; dayCount < totalDays; weekCount++)
            {
                dayCount += 7;
            }

            return weekCount;
        }

        /// <summary>
        /// Get number of Months
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="absolute"></param>
        /// <returns></returns>
        public static int GetNumberOfMonthsMtn(this DateTime start, DateTime end, bool absolute=false)
        {   
            
            if ( start.Year == end.Year && start.Month == end.Month)
            {
                return 1;
            }
            if(absolute)
                return Math.Abs((start.Month - end.Month) + 12 * (start.Year - end.Year));
            else
                return (start.Month - end.Month) + 12 * (start.Year - end.Year);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="forceFirstDayOfWeek"></param>
        /// <param name="initDay"></param>
        /// <param name="initSecond"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOnWeekMtn(this DateTime value, DayOfWeek forceFirstDayOfWeek = DayOfWeek.Sunday, bool initDay = true, bool initSecond = true)
        {
            var curWeekDay = ((int) value.DayOfWeek) + 1;
            var calcFirstDay = ((int) forceFirstDayOfWeek) + 1;
            var calcDay = curWeekDay - calcFirstDay;
            if (value.DayOfWeek != forceFirstDayOfWeek && calcDay > 0)
            {
                var dtWeek = value.AddDays(-calcDay);
                return initDay ? dtWeek.Date : dtWeek.Date.AddHours(value.Hour).AddMinutes(value.Minute).AddSeconds(initSecond ? 0 : value.Second);
            }
            else  return initDay? new DateTime(value.Year, value.Month, value.Day):value;
        }

        #endregion

        #region Stream 

        /// <summary>
        /// Return a byte array from Stream
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToByteArrayMtn(this Stream value)
        {
            if (value == Stream.Null || value.Length <= 0) return new byte[] {};

            if (value is MemoryStream)
            {
                return ((MemoryStream)value).ToArray();
            }

            using (var memoryStream = new MemoryStream())
            {
                value.CopyTo(memoryStream);
                
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Convert a stream to base64 string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64StringMtn(this Stream value)
        {
            return value != Stream.Null && value.Length > 0 ? Convert.ToBase64String(value.ToByteArrayMtn()) : "";
        }

        #endregion

        #region object

        /// <summary>
        /// Return a dictionary from object, don't work with a enumerable inside (yet)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="fieldName"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionaryMtn<T>(this T value, string fieldName=null, bool ignoreNull=true)
        {
            var result = new Dictionary<string, string>();
            
            if (value is Dictionary<string, string>)
                return (value as Dictionary<string, string>);

            if (value.IsPrimitiveMtn())
            {
                result.Add(fieldName??value.GetType().FullName, value.ToString());
                return result;
            }
            
            foreach (var prop in value.GetType().GetProperties())
            {
                var name = prop.Name;

                var valProp = value.GetType().GetProperty(prop.Name).GetValue(value, null);
                if(!ignoreNull && valProp == null)
                    continue;
                var safeVal = valProp == null ? "" : valProp.ToString();
                
                result.Add(name,safeVal);
            }

            return result;
        }
        #endregion
    }
}
