namespace AcApi.Infrastructure.Http
{
    public interface IHttpConfiguration
    {
        public int TimeoutInSeconds { get; }
        
        public string Token { get; }

        public string ServerURL { get; }

        public int MaxRetry { get; }

        public int TimeInSecondsBetweenRetry { get; }

        public string Endpoint { get; }
    }
}
