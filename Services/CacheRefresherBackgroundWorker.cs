using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repos.Entities;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CacheRefresherBackgroundWorker : BackgroundService
    {
        private readonly ILogger<CacheRefresherBackgroundWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(5);

        public CacheRefresherBackgroundWorker(ILogger<CacheRefresherBackgroundWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BACKGROUND WORKER: Cache refresher start running");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cacheEntryOptions = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    };

                    using var scope = _serviceProvider.CreateScope();

                    var redisCacheProductService = scope.ServiceProvider.GetRequiredService<IRedisCacheService<List<Product>>>();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                    // Attempt to retrieve products from cache
                    var cachedProducts = await redisCacheProductService.GetCacheValueAsync("[products]");

                    if (cachedProducts is null)
                    {
                        _logger.LogInformation("BACKGROUND WORKER: Cache miss. Fetching products from db...");

                        // Fetch products from the source (e.g., database)
                        var productsFromSource = await productService.GetProducts();

                        // Set the fetched products in the cache
                        await redisCacheProductService.SetCacheValueAsync("[products]", productsFromSource);

                        _logger.LogInformation("BACKGROUND WORKER: Products fetched from db and set in cache.");
                    }
                    else
                    {
                        _logger.LogInformation("BACKGROUND WORKER: Products successfully retrieved from cache.");
                    }


                    await Task.Delay(_refreshInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "BACKGROUND WORKER: An error occurred while refreshing cache");
                }
            }

            _logger.LogInformation("BACKGROUND WORKER: Cache refresher is stopping");
        }


    }
}
