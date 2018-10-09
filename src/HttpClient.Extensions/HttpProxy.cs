using System;
using System.Net;

namespace HttpClient.Extensions
{
    public class HttpProxy : IWebProxy
    {
        public HttpProxy(string proxyUri) : this(new Uri(proxyUri))
        {

        }

        public HttpProxy(Uri proxUri)
        {
            ProxyUri = proxUri;
        }

        public Uri ProxyUri { get; set; }

        public Uri GetProxy(Uri destination)
        {
            return ProxyUri;
        }

        public bool IsBypassed(Uri host)
        {
            return false;
        }

        public ICredentials Credentials { get; set; }
    }
}