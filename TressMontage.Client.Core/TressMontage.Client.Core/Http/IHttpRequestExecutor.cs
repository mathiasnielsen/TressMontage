using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Http
{
    public interface IHttpRequestExecutor
    {
        Task<TResponse> Put<TResponse>(string url, IEnumerable<KeyValuePair<string, string>> bodyParameter) where TResponse : class;

        Task<TResponse> Post<TContent, TResponse>(string url, TContent bodyParameter) where TResponse : class where TContent : class;

        Task<TResponse> Post<TResponse>(string url, IEnumerable<KeyValuePair<string, string>> bodyParameter) where TResponse : class;

        Task<TResponse> Post<TResponse>(string url, byte[] bodyParameter) where TResponse : class;

        Task<TResponse> Get<TResponse>(string url) where TResponse : class;

        Task<string> GetContent(string url);
    }
}
