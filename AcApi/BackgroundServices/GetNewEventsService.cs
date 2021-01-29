using AcApi.Controllers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AcApi.BackgroundServices
{
    public class GetNewEventsService : BackgroundService
    {
        private readonly ILogger<GetNewEventsService> _logger;
        private readonly SmartCardRepository Configuration;
        public GetNewEventsService(ILogger<GetNewEventsService> logger, SmartCardRepository  smartCard)
        {
            _logger = logger;
            Configuration = smartCard;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("Starting service.");
            stoppingToken.Register(() => _logger.LogDebug("Servicee is stoping."));
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("Getting new events in background");

                var result = Configuration.GetLastCard();
                Debug.WriteLine($"Resultado da task: {0}", result);
                await Countdown();
            }
            _logger.LogDebug("New Event found");
        }

        private async Task Countdown()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}
