
using HNApi.Models;
using HNApi.Repositories;
using HNApi.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace HNApiTest
{
    public class HNRepositoryTest
    {
        private Mock<IHNHttpClient> hnHttpClientMoq;
        private Mock<ILogger<InMemoryCache>> loggerMoq;

        private readonly StoryItem storyItem1 = new() { 
            Id = 1, Title = "Title 1", Score = 200, Url = "1", Descendants = 11, By = "Me", Time = DateTime.Now 
        };
        private readonly StoryItem storyItem2 = new() { 
            Id = 2, Title = "Title 2", Score = 100, Url = "2", Descendants = 12, By = "You", Time = DateTime.Now 
        };
        private readonly StoryItem storyItem3 = new() { 
            Id = 3, Title = "Title 3", Score = 1000, Url = "3", Descendants = 13, By = "Them", Time = DateTime.Now 
        };

        [SetUp]
        public void Setup()
        {
            hnHttpClientMoq = new();
            loggerMoq = new();

            hnHttpClientMoq
                .Setup(x => x.GetStory(1))
                .ReturnsAsync(storyItem1);

            hnHttpClientMoq
                .Setup(x => x.GetStory(2))
                .ReturnsAsync(storyItem2);

            hnHttpClientMoq
                .Setup(x => x.GetStory(3))
                .ReturnsAsync(storyItem3);
        }

        [Test]
        public async Task GetAllBestStories()
        {
            // Assign
            hnHttpClientMoq
                .Setup(x => x.GetBestStoryIds())
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            var sut = new HNRepository(hnHttpClientMoq.Object, loggerMoq.Object);

            // Act
            var actual = await sut.GetBestStories();

            // Assert
            Assert.That(actual, Is.EqualTo(new StoryItem[] { storyItem3, storyItem1, storyItem2 }));

            loggerMoq.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                0,
                It.Is<It.IsAnyType>((@o, @t) => @o.ToString()!.StartsWith("Cache miss")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(3));
            loggerMoq.VerifyNoOtherCalls();
        }

        [Test]
        public async Task GetFirstTwoBestStories_TwoOnly_ReturnFirstTwoStories()
        {
            // Assign
            hnHttpClientMoq
                .Setup(x => x.GetBestStoryIds())
                .ReturnsAsync(new List<int> { 1, 2, 3 });
  

            var sut = new HNRepository(hnHttpClientMoq.Object, loggerMoq.Object);

            // Act
            var actual = await sut.GetBestStories(2);

            // Assert
            Assert.That(actual, Is.EqualTo(new StoryItem[] { storyItem3, storyItem1 }));
        }

        [Test]
        public async Task GetBestStories_MissingIds_ReturnOnlyValidStories()
        {
            // Assign
            hnHttpClientMoq
                .Setup(x => x.GetBestStoryIds())
                .ReturnsAsync(new List<int> { 1, 2, 3, 4, 5, 6 });


            var sut = new HNRepository(hnHttpClientMoq.Object, loggerMoq.Object);

            // Act
            var actual = await sut.GetBestStories();

            // Assert
            Assert.That(actual, Is.EqualTo(new StoryItem[] { storyItem3, storyItem1, storyItem2 }));
        }

        [Test]
        public async Task GetBestStories_CallTwice_CacheOnlyMissesFirstCall()
        {
            // Assign
            hnHttpClientMoq
                .Setup(x => x.GetBestStoryIds())
                .ReturnsAsync(new List<int> { 1, 2, 3 });

            var sut = new HNRepository(hnHttpClientMoq.Object, loggerMoq.Object);

            // Act
            var actual1 = await sut.GetBestStories();
            var actual2 = await sut.GetBestStories();

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(actual1, Is.EqualTo(new StoryItem[] { storyItem3, storyItem1, storyItem2 }));
                Assert.That(actual2, Is.EqualTo(new StoryItem[] { storyItem3, storyItem1, storyItem2 }));
            });

            loggerMoq.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                0,
                It.Is<It.IsAnyType>((@o, @t) => @o.ToString()!.StartsWith("Cache miss")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(3));

            loggerMoq.VerifyNoOtherCalls();
        }
    }
}