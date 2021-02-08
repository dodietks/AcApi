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
        private readonly ConfigurationOptions _configuration;
        public IHttpRequest HttpRequest { get; set; }

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

                await SendEvent(result);
                _logger.LogInformation($"Post content: {result}.");
            }
            return Task.CompletedTask;
        }

        private async Task Countdown()
        {
            await Task.Delay(TimeSpan.FromSeconds(_configuration.TaskDelay));
        }

        private async Task SendEvent(SmartCardDTO smartCard)
        {
            Token token = Token.Bearer(_configuration.Token);

            var unitMessage = HttpRequest.Post<SmartCardDTO>(_configuration.EndPointSCG, smartCard, token, TimeSpan.FromSeconds(_configuration.PostTimeOut));
            await Task.FromResult(unitMessage);
        }
    }
}
