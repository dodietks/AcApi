using AcApi.Controllers;
using AcApi.Infrastructure.Http;
using AcApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AcApi.BackgroundServices
{
    public class GetNewEventsService : BackgroundService
    {
        private readonly ILogger<GetNewEventsService> _logger;
        private readonly SmartCardRepository smartCardRepository;
        public IHttpRequest HttpRequest { get; set; }
        private string uri = "http://localhost:61387/api/SmartCard/";

        private readonly ConfigurationOptions _configuration;


        Token token = Token.Bearer("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6IlVzdcOhcmlvIFBhZHLDo28iLCJuYW1laWQiOiIwIiwiREVGQVVMVF9VU0VSX0FVVEgiOiJ0cnVlIiwicm9sZSI6WyIzNTAiLCIxMDAiLCIxNTAiLCIyMDAiLCIyNTAiXSwibmJmIjoxNjA4NjY5MDQyLCJleHAiOjE5MjQwMjkwNDIsImlhdCI6MTYwODY2OTA0Mn0.3B71wvTArrsm4kBIJAb_J5fZUo1tnehg1XXsR5ArcOE");
        public GetNewEventsService(ILogger<GetNewEventsService> logger,
                                   SmartCardRepository smartCard,
                                   IHttpRequest httpRequest,
                                   ConfigurationOptions _configuration)
        {
            _logger = logger;
            smartCardRepository = smartCard;
            this.HttpRequest = httpRequest;
            this._configuration = _configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting service.");
            stoppingToken.Register(() => _logger.LogDebug("Service is stoping."));
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Getting new events in background.");
                var results = _configuration.Gates.Select(x => checkGate(x)).ToList();

                _logger.LogDebug("Waiting all tasks will be done.");
                Task.WhenAll(results).Wait();

                await Countdown();
            }
        }

        private async Task<Task> checkGate(Gate gate)
        {
            var result = smartCardRepository.GetLastCard(gate);
            if (result != null)
            {
                _logger.LogDebug("New Event found");

                await SendEvent(uri, result, token);
                _logger.LogInformation($"Post content: {result}.");
            }
            return Task.CompletedTask;
        }

        private async Task Countdown()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        private async Task SendEvent(string uri, SmartCardDTO smartCard, Token token)
        {
            var unitMessage = HttpRequest.Post<SmartCardDTO>(uri, smartCard, token, TimeSpan.FromSeconds(2));
            await Task.FromResult(unitMessage);
        }
    }
}
