using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mtn.Library.Enums;
using Mtn.Library.ExtensionsEntity;
using Mtn.Library.Extensions;

namespace Mtn.Library.Utils
{
    /// <summary>
    /// Just simple utilities to send and receive data
    /// </summary>
    public static class HttpUtils
    {
        #region Post

        /// <summary>
        /// Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="uri"></param>
        /// <param name="formatType">in Json, attachments are ignored</param>
        /// <param name="attachments"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string Post<T>(T model, Uri uri, Enums.HttpFormatType formatType = HttpFormatType.Json, List<MtnFile> attachments = null, CookieContainer cookies = null)
        {
            var result = "";
            var data = new Dictionary<string, string>();
            switch (formatType)
            {
                case HttpFormatType.Json:
                    result = SendJsonPost(uri, model, cookies);
                    break;
                case HttpFormatType.FormData:

                    if (model is Dictionary<string, string>)
                        data = (model as Dictionary<string, string>);
                    else
                    {
                        data = model.ToDictionaryMtn();
                    }
                    result = SendMultiformDataToServer(uri, data, attachments, cookies);
                    break;
                case HttpFormatType.UrlEncoded:

                    if (model is Dictionary<string, string>)
                        data = (model as Dictionary<string, string>);
                    else
                    {
                        data = model.ToDictionaryMtn();
                    }
                    result = SendUrlEncoded(uri, data, cookies, HttpMethodType.Post, attachments);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("formatType");
            }


            return result;
        }
        #endregion

        #region Get

        /// <summary>
        /// Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="uri"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string SendGet<T>(T model, Uri uri, CookieContainer cookies = null)
        {
            var result = "";
            Dictionary<string, string> data;

            if (model is Dictionary<string, string>)
                data = (model as Dictionary<string, string>);
            else
            {
                data = model.ToDictionaryMtn();
            }
            result = SendUrlEncoded(uri, data, cookies, HttpMethodType.Get);

            return result;
        }
        #endregion

        #region SendJson
        private static string SendJsonPost<T>(Uri uri, T model, CookieContainer cookies = null)
        {
            var json = model.ToJsonMtn();
            // create a request
            var request = (HttpWebRequest)WebRequest.Create(uri); request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            // turn our request string into a byte stream
            var postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "text/json";
            request.Accept = "application/json,text/html,application/xhtml+xml,text/plain,application/xml;q=0.9,*/*;q=0.8";
            request.ContentLength = postBytes.Length;
            if (cookies != null)
                request.CookieContainer = cookies;

            var requestStream = request.GetRequestStream();

            // now send it
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();


            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();

            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);

                var responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            else
            {
                return "";
            }
        }
        #endregion


        #region Stream utils
        //reference from http://stackoverflow.com/questions/19954287/how-to-upload-file-to-server-with-http-post-multipart-form-data thanks to Darin Rousseau and Xyroid
        /// <summary>
        /// Creates HTTP POST request and uploads database to server. Author : Farhan Ghumra
        /// </summary>
        private static string SendMultiformDataToServer(Uri uri, Dictionary<string, string> data, List<MtnFile> attachments = null, CookieContainer cookies = null)
        {
            var boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.KeepAlive = false;
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Accept = "application/json,text/html,application/xhtml+xml,text/plain,application/xml;q=0.9,*/*;q=0.8";
            request.Method = "POST";
            if (cookies != null)
                request.CookieContainer = cookies;

            var requestStream = request.GetRequestStream();
            WriteMultipartForm(requestStream, boundary, data, attachments);
            requestStream.Close();
            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();

            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);

                var responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            else
            {
                return "";
            }            
        }

        /// <summary>
        /// Writes multi part HTTP POST request. Author : Farhan Ghumra
        /// </summary>
        private static void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, List<MtnFile> attachments = null)
        {
            // The first boundary
            var boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            // the last boundary.
            var trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "–-\r\n");
            // the form data, properly formatted
            const string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            // the form-data file upload, properly formatted
            const string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            // Added to track if we need a CRLF or not.
            var bNeedsCrlf = false;

            if (data != null)
            {
                foreach (var key in data.Keys)
                {
                    // if we need to drop a CRLF, do that.
                    if (bNeedsCrlf)
                        WriteToStream(s, "\r\n");

                    // Write the boundary.
                    WriteToStream(s, boundarybytes);

                    // Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCrlf = true;
                }
            }

            // If we don't have keys, we don't need a crlf.
            if (bNeedsCrlf)
                WriteToStream(s, "\r\n");

            if (attachments == null || attachments.Count <= 0) return;
            foreach (var attachment in attachments)
            {
                WriteToStream(s, boundarybytes);
                WriteToStream(s, string.Format(fileheaderTemplate, "file", attachment.FileName, attachment.MediaType));
                // Write the file data to the stream.
                var bytesData = attachment.LoadFile ? attachment.FilePath.ReadBinaryFileMtn() : attachment.MemoryStreamInfo.ToByteArrayMtn();
                WriteToStream(s, bytesData);
                WriteToStream(s, trailer);
            }
        }

        /// <summary>
        /// Writes string to stream. Author : Farhan Ghumra
        /// </summary>
        private static void WriteToStream(Stream s, string txt)
        {
            var bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes byte array to stream. Author : Farhan Ghumra
        /// </summary>
        private static void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        #endregion

        #region UrlEncoded
        private static string SendUrlEncoded(Uri uri, Dictionary<string, string> data, CookieContainer cookies = null, Enums.HttpMethodType methodType = HttpMethodType.Post, List<MtnFile> attachments = null)
        {
            // create a request
            var isFirst = true;

            var strData = "";
            if (data != null)
            {
                foreach (var pair in data)
                {
                    strData += 
                              "{0}{1}={2}".FormatMtn((isFirst ? "" : "&"), pair.Key, HttpUtility.UrlEncode(pair.Value));
                    isFirst = false;
                }
            }

            if (methodType == HttpMethodType.Get && !strData.IsNullOrWhiteSpaceMtn())
            {
                strData = (!uri.Query.IsNullOrWhiteSpaceMtn()? (uri.Query + "&"):"?") + strData;
                var url = "{0}{1}".FormatMtn(uri.Query.IsNullOrWhiteSpaceMtn()?uri.AbsoluteUri: uri.AbsoluteUri.Replace(uri.Query, ""), strData);
                uri = new Uri(url);
            }

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = methodType.GetEnumDescriptionMtn();
            request.Accept = "application/json,text/html,application/xhtml+xml,text/plain,application/xml;q=0.9,*/*;q=0.8";

            if (cookies != null)
                request.CookieContainer = cookies;


            if (methodType == HttpMethodType.Post)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                // turn our request string into a byte stream
                var postBytes = Encoding.UTF8.GetBytes(strData);
                request.ContentLength = postBytes.Length;
                var requestStream = request.GetRequestStream();

                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
                if (attachments != null)
                {
                    for (var index = 0; index < attachments.Count; index++)
                    {
                        var strInitFile = "";
                        if (strData.IsNullOrWhiteSpaceMtn() && index == 0)
                            strInitFile = "";
                        else
                            strInitFile = "&";

                        var attachment = attachments[index];


                        var bytesData = attachment.LoadFile
                            ? attachment.FilePath.ReadBinaryFileMtn()
                            : attachment.MemoryStreamInfo.ToByteArrayMtn();
                        strInitFile += attachment.FileName.ToString() + "=" + bytesData.ToStringMtn();
                        strInitFile = HttpUtility.UrlEncode(strInitFile);
                        var bytesPost = strInitFile.ToByteArrayMtn();
                        requestStream.Write(bytesPost, 0, bytesPost.Length);
                    }
                }
                requestStream.Close();
            }

            var response = request.GetResponse();
            var dataStream = response.GetResponseStream();

            if (dataStream != null)
            {
                var reader = new StreamReader(dataStream);

                var responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            else
            {
                return "";
            }
        }
        #endregion

    }
}