using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryTweeter;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TwitterWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            TwitterService twitterService = new TwitterService();

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Factory.StartNew(() => { twitterService.StartCycle(); });
            //}
        }
    }
}
