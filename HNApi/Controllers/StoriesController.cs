using HNApi.Dtos;
using HNApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HNApi.Controllers
{
    [ApiController]
    [Route("api/Stories")]
    public class StoriesController : ControllerBase
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly HNRepository _hnRepository;

        public StoriesController(ILogger<StoriesController> logger, HNRepository hnRepository)
        {
            _logger = logger;
            _hnRepository = hnRepository;
        }

        [HttpGet(Name = "GetBestStories{%d}")]
        public async Task<ActionResult<List<Story>>> GetBestStories(int? count)
        {
            _logger.LogInformation("Stories: {count}", count);

            var storyItems = await _hnRepository.GetBestStories(count);

            return storyItems.Select(si => si.ToStoryDto()).ToList();
        }
    }
}