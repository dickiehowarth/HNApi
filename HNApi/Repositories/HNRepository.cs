using HNApi.Models;
using HNApi.Services;

namespace HNApi.Repositories
{
    public class HNRepository
    {
        private const int RefreshInterval = 300;
        private readonly IHNHttpClient _hnClient;
        private readonly IInMemoryCache _cache;

        public HNRepository(IHNHttpClient hnClient, ILogger<InMemoryCache> logger)
        {
            _hnClient = hnClient;
            _cache = new InMemoryCache(logger, RefreshInterval);
        }

        private async Task<StoryItem?> GetStoryItem(int id)
        {
            if (!_cache.TryGetValue(id, out var item))
            {
                item = await _hnClient.GetStory(id);

                if (item != null)
                {
                    _cache.TryAdd(id, item);
                }
            }
            return item;
        }

        public async Task<List<StoryItem>> GetBestStories(int? count = null)
        {
            var ids = await _hnClient.GetBestStoryIds();

            var tasks = ids.Select(id => GetStoryItem(id)).ToArray();

            var storyItems = await Task.WhenAll(tasks);

            var orderedStoriesQuery = storyItems.Where(si => si != null)
                .Select(si => si!).OrderByDescending(si => si!.Score).AsQueryable();

            if (count != null)
            {
                orderedStoriesQuery = orderedStoriesQuery.Take(count.Value);
            }

            return orderedStoriesQuery.ToList();
        }
    }
}
