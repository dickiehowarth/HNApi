using HNApi.Models;
using HNApi.Services;
using System.Collections.Concurrent;

namespace HNApi.Repositories
{
    public class HNRepository
    {
        private readonly HNHttpClient _hnClient;
        private readonly ILogger<HNRepository> _logger;

        // change to an IMemoryCache
        private readonly ConcurrentDictionary<int, StoryItem> _storyItems = new();

        public HNRepository(HNHttpClient hnClient, ILogger<HNRepository> logger)
        {
            _hnClient = hnClient;
            _logger = logger;
        }

        private async Task<StoryItem?> GetStoryItem(int id)
        {
            if (!_storyItems.TryGetValue(id, out var item))
            {
                item = await _hnClient.GetStory(id);
                _logger.LogInformation("Cache miss, Get Story Item from Client API {id}", id);

                if (item != null)
                {
                    _storyItems.TryAdd(id, item);
                }
            }
            return item;
        }

        public async Task<List<StoryItem>> GetBestStories(int? count)
        {
            var ids = await _hnClient.GetBestStoryIds();

            var tasks = ids.Select(id => GetStoryItem(id)).ToArray();

            var storyItems = await Task.WhenAll(tasks);

            var orderedStoriesQuery = storyItems.Where(si => si != null).Select(si => si!).OrderByDescending(si => si!.Score).AsQueryable();

            if (count != null)
            {
                orderedStoriesQuery = orderedStoriesQuery.Take(count.Value);
            }

            return orderedStoriesQuery.ToList();
        }
    }
}
