using HNApi.Models;

namespace HNApi.Dtos
{
    public static class Mapping
    {
        public static Story ToStoryDto(this StoryItem storyItem)
        {
            return new Story
            {
                Title = storyItem.Title,
                PostedBy = storyItem.PostedBy,
                Time = storyItem.Time,
                Score = storyItem.Score,
                CommentCount = storyItem.Descendants,
                Uri = storyItem.Url
            };
        }
    }
}
