using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KafkaFlow.Serializer;

/// <summary>
/// A message serializer using System.Text.Json library
/// </summary>
public class JsonCoreSerializer : ISerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
    /// </summary>
    /// <param name="options">Json serializer options</param>
    public JsonCoreSerializer(JsonSerializerOptions options)
    {
        _serializerOptions = options;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCoreSerializer"/> class.
    /// </summary>
    public JsonCoreSerializer()
        : this(new JsonSerializerOptions())
    {
    }

    /// <inheritdoc/>
    public async Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        await JsonSerializer.SerializeAsync(output, message, _serializerOptions).ConfigureAwait(false);
    }
}
