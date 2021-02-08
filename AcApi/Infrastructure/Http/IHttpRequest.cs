using System;

namespace AcApi.Infrastructure.Http
{
    public interface IHttpRequest
    {
        T Get<T>(string url, Token token);
        T Get<T>(string url, Token token, TimeSpan timeout);
        String GetText(string url, Token token, TimeSpan timeout);
        T Post<T>(string url, object postBody, Token token, TimeSpan timeout);
    }
}
