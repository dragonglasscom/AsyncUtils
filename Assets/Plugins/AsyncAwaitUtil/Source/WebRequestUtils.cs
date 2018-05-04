using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityAsyncAwaitUtil;
using UnityEngine;
using UnityEngine.Networking;

namespace Plugins.AsyncAwaitUtil.Source
{
    public static class WebRequestUtils
    {
        private static async Task<HttpResponseMessage> unsafe_send_webrequest(UnityWebRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //Prepare cancelation
            using (cancellationToken.Register(() =>
            {
                //Unity API is not thread safe, thus request abort must happen on Unity Main thread
                SyncContextUtil.UnitySynchronizationContext.Post(_ =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    request.Abort();
                }, null);
            }))
            {

                await request.SendWebRequest();

                //The web request will be completed early with "Aborted error",
                //Will need to throw here, to indicate that the task was aborted
                cancellationToken.ThrowIfCancellationRequested();
                
                return new HttpResponseMessage((HttpStatusCode) request.responseCode, request.downloadHandler.data,
                    request.GetResponseHeaders());
            }
        }
        
        public static async Task<HttpResponseMessage> CreateAndSendWebrequest(WebrequestCreationInfo webrequestCreationInfo,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await new WaitForUpdate();

            using (var request = create_web_request(webrequestCreationInfo))
            {
                return await unsafe_send_webrequest(request, cancellationToken).ConfigureAwait(false);
            }
        }

        private static UnityWebRequest create_web_request(WebrequestCreationInfo requestCreationInfo)
        {
            var webrequest = new UnityWebRequest(requestCreationInfo.Url);

            webrequest.method = requestCreationInfo.Method.ToString();
            
            webrequest.SetRequestHeader("Content-Type", requestCreationInfo.ContentType);

            if (requestCreationInfo.Data != null && requestCreationInfo.Data.Length != 0)
                webrequest.uploadHandler = new UploadHandlerRaw(requestCreationInfo.Data);

            webrequest.downloadHandler = new DownloadHandlerBuffer();

            return webrequest;
        }

        public static Task<HttpResponseMessage> GetAsync(string uri,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var creationInfo = new WebrequestCreationInfo()
            {
                Method = WebRequestMethod.GET,
                Data = new byte[0],
                Url = uri
            };
            return CreateAndSendWebrequest(creationInfo, cancellationToken);
        }

        public static Task<HttpResponseMessage> PostAsync(string uri, string postData,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var data = Encoding.UTF8.GetBytes(postData);
            return PostAsync(uri, data, contentType, cancellationToken);
        }
        
        public static Task<HttpResponseMessage> PostAsync(string uri, byte[] postData,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var creationInfo = new WebrequestCreationInfo()
            {
                Method = WebRequestMethod.POST,
                Data = postData,
                Url = uri,
                ContentType = contentType
            };
            return CreateAndSendWebrequest(creationInfo, cancellationToken);
        }
        
        public static Task<HttpResponseMessage> PutAsync(string uri, string postData,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var data = Encoding.UTF8.GetBytes(postData);
            return PutAsync(uri, data, contentType, cancellationToken);
        }

        public static Task<HttpResponseMessage> PutAsync(string uri, byte[] postData,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var creationInfo = new WebrequestCreationInfo()
            {
                Method = WebRequestMethod.PUT,
                Data = postData,
                Url = uri,
                ContentType = contentType
            };
            return CreateAndSendWebrequest(creationInfo, cancellationToken);
        }
        
        public static Task<HttpResponseMessage> DeleteAsync(string uri, string postData = "",
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var data = Encoding.UTF8.GetBytes(postData);
            return DeleteAsync(uri, data, contentType, cancellationToken);
        }

        public static Task<HttpResponseMessage> DeleteAsync(string uri, byte[] postData = null,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var creationInfo = new WebrequestCreationInfo()
            {
                Method = WebRequestMethod.DELETE,
                Data = postData,
                Url = uri,
                ContentType = contentType
            };
            return CreateAndSendWebrequest(creationInfo, cancellationToken);
        }
    }

    public class WebrequestCreationInfo
    {
        private string _contentType;

        public WebrequestCreationInfo()
        {
            _contentType = "application/json";
        }

        public string Url { get; set; }
        
        public WebRequestMethod Method { get; set; }

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public byte[] Data { get; set; }
    }

    public enum WebRequestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpResponseMessage
    {
        public byte[] Data { get; }
        
        public HttpStatusCode StatusCode { get; }
        
        public Dictionary<string, string> ResponseHeaders { get; }

        public HttpResponseMessage(HttpStatusCode statusCode, byte[] data, 
            Dictionary<string, string> responseHeaders = null)
        {
            StatusCode = statusCode;
            Data = data;
            ResponseHeaders = responseHeaders;
        }
        
        public bool IsSuccessStatusCode
        {
            get { return ((int)StatusCode >= 200) && ((int)StatusCode <= 299); }
        }

        public Stream ToStream()
        {
            return new MemoryStream(Data);
        }

        public string ReadToEnd()
        {
            using (var str = ToStream())
            {
                using (var sr = new StreamReader(str))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        public T DeserializeAsJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(ReadToEnd());
        }
    }
}