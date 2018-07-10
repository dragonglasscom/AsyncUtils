using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.AsyncAwaitUtil.Source
{
    public class UnityHttpClient
    {
        public Uri BaseAddress
        {
            get
            {
                return _baseAddress;
            }
            set
            {
                CheckBaseAddress(value, nameof(BaseAddress));
                _baseAddress = value;
            }
        }

        private Uri _baseAddress;
        
        public Task<HttpResponseMessage> CreateAndSendWebrequest(WebrequestCreationInfo webrequestCreationInfo,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Uri absUri;
            if (string.IsNullOrWhiteSpace(webrequestCreationInfo.Url))
            {
                if(BaseAddress == null)
                    throw new Exception("Call address empty with no base address provided");
                else
                {
                    absUri = BaseAddress;
                }
            }
            else
            {
                var requestUri = new Uri(webrequestCreationInfo.Url);
                absUri = new Uri(BaseAddress, requestUri);
                
                if(BaseAddress == null && !requestUri.IsAbsoluteUri)
                    throw new Exception("No base address for relative Uri provided");
            }
            
            
            var callInput = new WebrequestCreationInfo
            {
                Data = webrequestCreationInfo.Data,
                Headers = webrequestCreationInfo.Headers,
                Method = webrequestCreationInfo.Method,
                Url = absUri.AbsoluteUri
            };

            return WebRequestUtils.CreateAndSendWebrequest(callInput, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string uri = "",
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

        public Task<HttpResponseMessage> PostAsync(string uri = "", string postData = "",
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var data = Encoding.UTF8.GetBytes(postData);
            return PostAsync(uri, data, contentType, cancellationToken);
        }
        
        public Task<HttpResponseMessage> PostAsync(string uri = "", byte[] postData = default(byte[]),
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
        
        public Task<HttpResponseMessage> PutAsync(string uri = "", string postData = "",
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var data = Encoding.UTF8.GetBytes(postData);
            return PutAsync(uri, data, contentType, cancellationToken);
        }

        public Task<HttpResponseMessage> PutAsync(string uri = "", byte[] postData = default(byte[]),
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
        
        public Task<HttpResponseMessage> DeleteAsync(string uri = "", string postData = "",
            string contentType = "application/json",
            CancellationToken cancellationToken = new CancellationToken())
        {
            var data = Encoding.UTF8.GetBytes(postData);
            return DeleteAsync(uri, data, contentType, cancellationToken);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri = "", byte[] postData = null,
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


        private void CheckBaseAddress(Uri baseAddress, string parameterName)
        {
            if(baseAddress == null)
                return;
            
            if (!baseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException("Base adress should be an absolute Uri", parameterName);
            }
        }
    }
}