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
            //request.Headers.Add("Authorization", $"Basic {token.AuthenticationToken}");
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

        public WebRequest Post(string url, AccessToken token, string contentType)
        {
            var d = WebUtility.UrlEncode(contentType);
            var request = WebRequest.Create(new Uri(url));
            request.ContentType = "application/octet-stream";
            
            request.Method = "POST";
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {token.AuthenticationToken}";

            return request;
        }
    }
}
