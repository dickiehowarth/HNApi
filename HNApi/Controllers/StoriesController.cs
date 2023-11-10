using HNApi.Dtos;
using HNApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HNApi.Controllers
{
    [ApiController]
    [Route("api/hackernews")]
    public class StoriesController : ControllerBase
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly HNRepository _hnRepository;

        public StoriesController(ILogger<StoriesController> logger, HNRepository hnRepository)
        {
            _logger = logger;
            _hnRepository = hnRepository;
        }

        [HttpGet("beststories", Name = "GetBestStories")]
        public async Task<ActionResult<List<Story>>> GetBestStories(int? count = null)
        {
            _logger.LogInformation("Best Stories: {count}", count);

            var storyItems = await _hnRepository.GetBestStories(count);

            return storyItems.Select(si => si.ToStoryDto()).ToList();
        }
    }
}