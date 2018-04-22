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
        private static async Task<HttpResponseMessage> unsafe_ProcessWebrequest(UnityWebRequest request,
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

        public static async Task<HttpResponseMessage> Get(string uri,
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (SyncContextUtil.UnitySynchronizationContext == SynchronizationContext.Current)
            {
                //We are on unity thread, all good
                using (var request = UnityWebRequest.Get(uri))
                {
                    return await unsafe_ProcessWebrequest(request, cancellationToken);
                }
            }

            //We were called outside of the Unity main thread
            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            //Push the async method onto Unity's main thread
            SyncContextUtil.UnitySynchronizationContext.Post(async _ =>
            {
                try
                {
                    using (var request = UnityWebRequest.Get(uri))
                    {
                        var task = unsafe_ProcessWebrequest(request, cancellationToken);
                        var result = await task;
                        tcs.SetResult(result);
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);

            return await tcs.Task;
        }

        public static async Task<HttpResponseMessage> Post(string uri, string postData,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (SyncContextUtil.UnitySynchronizationContext == SynchronizationContext.Current)
            {
                //We are on unity thread, all good
                using (var request = new UnityWebRequest(uri))
                {
                    //Why aren't we using UnityWebRequest.Post you ask?
                    //well, it ignores content type and encoding no matter what
                    //Thanks, Unity!
                    request.method = "POST";
                    var data = Encoding.UTF8.GetBytes(postData);
                    var uploadHandler = new UploadHandlerRaw(data);
                    request.uploadHandler = uploadHandler;
                    request.downloadHandler = new DownloadHandlerBuffer();

                    request.SetRequestHeader("Content-Type", contentType);

                    return await unsafe_ProcessWebrequest(request, cancellationToken);
                }
            }

            //We were called outside of the Unity main thread
            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            //Push the async method onto Unity's main thread
            SyncContextUtil.UnitySynchronizationContext.Post(async _ =>
            {
                try
                {
                    using (var request = new UnityWebRequest(uri))
                    {
                        //Why aren't we using UnityWebRequest.Post you ask?
                        //well, it ignores content type and encoding no matter what
                        //Thanks, Unity!
                        request.method = "POST";
                        var data = Encoding.UTF8.GetBytes(postData);
                        var uploadHandler = new UploadHandlerRaw(data);
                        request.uploadHandler = uploadHandler;
                        request.downloadHandler = new DownloadHandlerBuffer();

                        request.SetRequestHeader("Content-Type", contentType);
                        var task = unsafe_ProcessWebrequest(request, cancellationToken);
                        var result = await task;
                        tcs.SetResult(result);
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);

            return await tcs.Task;
        }

        public static async Task<HttpResponseMessage> Put(string uri, string postData,
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (SyncContextUtil.UnitySynchronizationContext == SynchronizationContext.Current)
            {
                //We are on unity thread, all good
                using (var request = new UnityWebRequest(uri))
                {
                    //Why aren't we using UnityWebRequest.Put you ask?
                    //well, it ignores content type and encoding no matter what
                    //Thanks, Unity!
                    request.method = "PUT";
                    var data = Encoding.UTF8.GetBytes(postData);
                    var uploadHandler = new UploadHandlerRaw(data);
                    request.uploadHandler = uploadHandler;
                    request.downloadHandler = new DownloadHandlerBuffer();

                    request.SetRequestHeader("Content-Type", contentType);

                    return await unsafe_ProcessWebrequest(request, cancellationToken);
                }
            }

            //We were called outside of the Unity main thread
            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            //Push the async method onto Unity's main thread
            SyncContextUtil.UnitySynchronizationContext.Post(async _ =>
            {
                try
                {
                    using (var request = new UnityWebRequest(uri))
                    {
                        //Why aren't we using UnityWebRequest.Put you ask?
                        //well, it ignores content type and encoding no matter what
                        //Thanks, Unity!
                        request.method = "PUT";
                        var data = Encoding.UTF8.GetBytes(postData);
                        var uploadHandler = new UploadHandlerRaw(data);
                        request.uploadHandler = uploadHandler;
                        request.downloadHandler = new DownloadHandlerBuffer();

                        request.SetRequestHeader("Content-Type", contentType);
                        var task = unsafe_ProcessWebrequest(request, cancellationToken);
                        var result = await task;
                        tcs.SetResult(result);
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);

            return await tcs.Task;
        }

        public static async Task<HttpResponseMessage> Delete(string uri, string postData = "",
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            if (SyncContextUtil.UnitySynchronizationContext == SynchronizationContext.Current)
            {
                //We are on unity thread, all good
                using (var request = new UnityWebRequest(uri))
                {
                    //Why aren't we using UnityWebRequest.Delete you ask?
                    //well, it ignores content type and encoding no matter what
                    //Thanks, Unity!
                    request.method = "DELETE";
                    var data = Encoding.UTF8.GetBytes(postData);
                    var uploadHandler = new UploadHandlerRaw(data);
                    request.uploadHandler = uploadHandler;
                    request.downloadHandler = new DownloadHandlerBuffer();

                    request.SetRequestHeader("Content-Type", contentType);

                    return await unsafe_ProcessWebrequest(request, cancellationToken);
                }
            }

            //We were called outside of the Unity main thread
            var tcs = new TaskCompletionSource<HttpResponseMessage>();

            //Push the async method onto Unity's main thread
            SyncContextUtil.UnitySynchronizationContext.Post(async _ =>
            {
                try
                {
                    using (var request = new UnityWebRequest(uri))
                    {
                        //Why aren't we using UnityWebRequest.Delete you ask?
                        //well, it ignores content type and encoding no matter what
                        //Thanks, Unity!
                        request.method = "DELETE";
                        var data = Encoding.UTF8.GetBytes(postData);
                        var uploadHandler = new UploadHandlerRaw(data);
                        request.uploadHandler = uploadHandler;
                        request.downloadHandler = new DownloadHandlerBuffer();

                        request.SetRequestHeader("Content-Type", contentType);
                        var task = unsafe_ProcessWebrequest(request, cancellationToken);
                        var result = await task;
                        tcs.SetResult(result);
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);

            return await tcs.Task;
        }
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