using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.BackgroundServices
{
    public class EmailTask : IHostedService
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IHttpContextAccessor _httpContextAccessor;




        public EmailTask(IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // throw new NotImplementedException();
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // throw new NotImplementedException();
            return Task.CompletedTask;
        }

        protected virtual async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //stoppingToken.Register(() =>
            // _logger.LogDebug($" GracePeriod background task is stopping."));

            do
            {
                await Process();
                //await Task.Delay(60000, stoppingToken); //60 seconds delay
                await Task.Delay(43200000, stoppingToken); //12 hours delay

            }
            while (!stoppingToken.IsCancellationRequested);
        }




        protected async Task Process()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {


                await Task.Delay(2500); //0.25 seconds delay

            }




        }



    }

}
