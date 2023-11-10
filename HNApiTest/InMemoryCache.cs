
using HNApi.Models;
using HNApi.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace HNApiTest
{
    public class InMemeoryCacheTest
    {
        private readonly StoryItem storyItem1 = new() { 
            Id = 1, Title = "Title 1", Score = 200, Url = "1", Descendants = 11, By = "Me", Time = DateTime.Now 
        };
        private readonly StoryItem storyItem2 = new() { 
            Id = 2, Title = "Title 2", Score = 100, Url = "2", Descendants = 12, By = "You", Time = DateTime.Now 
        };

        [Test]
        public void Cache_AddAllAndGetAll_NoMissesLogs()
        {
            // Assign 
             var loggerMoq = new Mock<ILogger<InMemoryCache>>();

            var sut = new InMemoryCache(loggerMoq.Object, 300);


            // Act
            var actualAdd1 = sut.TryAdd(storyItem1.Id, storyItem1);
            var actualAdd2 = sut.TryAdd(storyItem2.Id, storyItem2);


            var actualGet1 = sut.TryGetValue(storyItem1.Id, out var actual1);
            var actualGet2 = sut.TryGetValue(storyItem2.Id, out var actual2);
            Assert.Multiple(() =>
            {


                // Assert
                Assert.That(actualAdd1, Is.True);
                Assert.That(actualAdd2, Is.True);
                Assert.That(actualGet1, Is.True);
                Assert.That(actualGet2, Is.True);

                Assert.That(actual1, Is.EqualTo(storyItem1));
                Assert.That(actual2, Is.EqualTo(storyItem2));
            });

            loggerMoq.VerifyNoOtherCalls();
        }

        [Test]
        public void Cache_GetItemFail_OneMissLog()
        {
            // Assign 
            var loggerMoq = new Mock<ILogger<InMemoryCache>>();

            var sut = new InMemoryCache(loggerMoq.Object, 300);


            // Act
            var actualGet1 = sut.TryGetValue(storyItem1.Id, out var actual1);


            // Assert
            Assert.That(actualGet1, Is.False);

            loggerMoq.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                0,
                It.Is<It.IsAnyType>((@o, @t) => @o.ToString()!.StartsWith("Cache miss")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            loggerMoq.VerifyNoOtherCalls();
        }

        [Test]
        public void Cache_ClearCacheAfterLifetime_MissesLogs()
        {
            // Assign 
            var loggerMoq = new Mock<ILogger<InMemoryCache>>();

            var sut = new InMemoryCache(loggerMoq.Object, 3);


            // Act
            var actualAdd1 = sut.TryAdd(storyItem1.Id, storyItem1);
            var actualAdd2 = sut.TryAdd(storyItem2.Id, storyItem2);


            var actualGet1 = sut.TryGetValue(storyItem1.Id, out var actual1);
            var actualGet2 = sut.TryGetValue(storyItem2.Id, out var actual2);

            Thread.Sleep(5000);

            var delay_actualGet1 = sut.TryGetValue(storyItem1.Id, out var delay_actual1);
            var delay_actualGet2 = sut.TryGetValue(storyItem2.Id, out var delay_actual2);

            Assert.Multiple(() =>
            {
                // Assert
                Assert.That(actualAdd1, Is.True);
                Assert.That(actualAdd2, Is.True);
                Assert.That(actualGet1, Is.True);
                Assert.That(actualGet2, Is.True);
                Assert.That(delay_actualGet1, Is.False);
                Assert.That(delay_actualGet2, Is.False);

                Assert.That(actual1, Is.EqualTo(storyItem1));
                Assert.That(actual2, Is.EqualTo(storyItem2));
            });

            loggerMoq.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                0,
                It.Is<It.IsAnyType>((@o, @t) => @o.ToString()!.StartsWith("Cache miss")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2));
            loggerMoq.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                0,
                It.Is<It.IsAnyType>((@o, @t) => @o.ToString()!.StartsWith("Cache refreshed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            loggerMoq.VerifyNoOtherCalls();
        }
    }
}