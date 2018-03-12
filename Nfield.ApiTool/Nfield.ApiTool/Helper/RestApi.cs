using System;
using System.Net;
using Nfield.ApiTool.Models;


namespace Nfield.ApiTool.Helper
{
    public class RestApi
    {
        public WebRequest Get(string url, AccessToken token)
        {
            var request = WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {token.AuthenticationToken}";
            
            return request;
        }

        public WebRequest GetText(string url, AccessToken token)
        {
            var request = WebRequest.Create(new Uri(url));
            request.ContentType = "text/plain";
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {token.AuthenticationToken}";

            return request;
        }

        public WebRequest Post(string url)
        {
            var request = WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "POST";
            

            return request;
        }

        public WebRequest Post(string url, AccessToken token)
        {
            var request = WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "POST";            
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {token.AuthenticationToken}";

            return request;
        }

        public WebRequest PostStream(string url, AccessToken token)
        {
            var request = WebRequest.Create(new Uri(url));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {token.AuthenticationToken}";

            return request;
        }

        public WebRequest Delete(string url, AccessToken token)
        {
            var request = WebRequest.Create(new Uri(url));
            request.Method = "DELETE";
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {token.AuthenticationToken}";

            return request;
        }
    }
}
