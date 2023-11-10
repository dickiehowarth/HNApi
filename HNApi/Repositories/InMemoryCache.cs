using HNApi.Models;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace HNApi.Repositories
{
    public interface IInMemoryCache
    {
        bool TryAdd(int key, StoryItem value);
        bool TryGetValue(int key, [MaybeNullWhen(false)] out StoryItem value);
    }

    public class InMemoryCache : IInMemoryCache
    {
        private readonly ConcurrentDictionary<int, StoryItem> _cache = new();
        private readonly ILogger<InMemoryCache> _logger;
        private readonly int _refreshInterval;
        private DateTime _lastCleared = DateTime.Now;

        public InMemoryCache(ILogger<InMemoryCache> logger, int refreshInterval)
        {
            _logger = logger;
            _refreshInterval = refreshInterval;
        }

        public bool TryAdd(int key, StoryItem value)
            => _cache.TryAdd(key, value);

        public bool TryGetValue(int key, [MaybeNullWhen(false)] out StoryItem value)
        {
            if (_lastCleared.AddSeconds(_refreshInterval) < DateTime.Now)
            {
                _cache.Clear();
                _lastCleared = DateTime.Now;
                _logger.LogInformation("Cache refreshed, lifetime: {_refreshInterval}secs", _refreshInterval);
            }

            var found = _cache.TryGetValue(key, out value);

            if (!found)
                _logger.LogInformation("Cache miss: {key}", key);

            return found;
        }
    }
}
