using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace TelemetryStash.Functions.Tests;
public class CacheProvider : IDisposable
{
    private readonly ServiceProvider _provider;

    public CacheProvider()
    {
        var collection = new ServiceCollection();

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        collection.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromDays(1),
                LocalCacheExpiration = TimeSpan.FromDays(1)
            };
        });
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        _provider = collection.BuildServiceProvider();
    }

    public HybridCache HybridCache => _provider.GetService<HybridCache>()!;

    public void Dispose()
    {
        _provider.Dispose();
    }
}
