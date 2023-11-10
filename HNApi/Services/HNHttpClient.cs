using HNApi.Models;

namespace HNApi.Services
{
    public class HNHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HNHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<int>> GetBestStoryIds()
        {
            var itemIds = await _httpClient.GetFromJsonAsync<List<int>>(_config["HNServiceUrl"] + "/v0/beststories.json");

            return itemIds ?? new List<int>();
        }

        public async Task<StoryItem?> GetStory(int id)
        {
            var story = await _httpClient.GetFromJsonAsync<StoryItem>(_config["HNServiceUrl"] + $"/v0/item/{id}.json");

            return story;
        }
    }
}
