using HNApi.JsonConverters;
using System.Text;
using System.Text.Json;

namespace HNApiTest
{
    public class UnixToNullableDateTimeConverterTest
    {

        [Test]
        public void Write_NotSupportedException()
        {
            // Assign
            using MemoryStream memStream = new (100);
            
            var writer = new Utf8JsonWriter(memStream);

            var sut = new UnixToNullableDateTimeConverter();

            // Act/Assert
            Assert.Throws< NotSupportedException>(() => sut.Write(writer, DateTime.Now, new JsonSerializerOptions { }));
        }

        [Test]
        public void Read_UnixEpoch_ReturnDateTime()
        {

            // Assign
            var json = "1699615456";

            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), new JsonReaderOptions { });
            while (reader.TokenType == JsonTokenType.None)
            {
                if (!reader.Read())
                {
                    break;
                }
            }

            var sut = new UnixToNullableDateTimeConverter();

            // Act
            var actual = sut.Read(ref reader, typeof(DateTime?), new JsonSerializerOptions { });


            Console.WriteLine(actual);

            // Assert
            Assert.That(actual, Is.EqualTo(new DateTime(2023, 11, 10, 11, 24, 16)));
        }

        [Test]
        public void Read_InvalidUnixEpoch_ReturnDateTime()
        {

            // Assign
            var json = "xyz";

            var sut = new UnixToNullableDateTimeConverter();

            // Act/Assert
            // System.Text.Json.JsonReaderException - MS has this currently scoped to internal
            Assert.Throws(Is.InstanceOf<JsonException>(), () =>
            {
                var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), new JsonReaderOptions { });
                while (reader.TokenType == JsonTokenType.None)
                {
                    if (!reader.Read())
                    {
                        break;
                    }
                }

                sut.Read(ref reader, typeof(DateTime?), new JsonSerializerOptions { });
            });
        }
    }
}