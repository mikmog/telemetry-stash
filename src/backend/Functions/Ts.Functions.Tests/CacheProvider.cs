using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace TelemetryStash.Functions.Tests;
public class CacheProvider : IDisposable
{
    private readonly ServiceProvider _provider;

    public CacheProvider()
    {
        var collection = new ServiceCollection();
        collection.AddHybridCache();
        _provider = collection.BuildServiceProvider();
    }

    public HybridCache HybridCache => _provider.GetService<HybridCache>()!;

    public void Dispose()
    {
        _provider.Dispose();
    }
}
