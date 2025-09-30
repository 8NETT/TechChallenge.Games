using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace TechChallenge.Games.Command.Infrastructure.Persistence
{
    public class NewtonsoftJsonCosmosSerializer : CosmosSerializer
    {
        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonCosmosSerializer(JsonSerializerSettings settings)
        {
            _serializer = JsonSerializer.Create(settings);
        }

        public override T FromStream<T>(Stream stream)
        {
            using var sr = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(sr);
            return _serializer.Deserialize<T>(jsonTextReader);
        }

        public override Stream ToStream<T>(T input)
        {
            var stream = new MemoryStream();
            using var sw = new StreamWriter(stream, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(sw);
            _serializer.Serialize(jsonWriter, input);
            jsonWriter.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
