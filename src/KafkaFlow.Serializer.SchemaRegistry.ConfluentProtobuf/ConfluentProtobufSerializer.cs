using System;
using System.IO;
using System.Threading.Tasks;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace KafkaFlow.Serializer.SchemaRegistry;

/// <summary>
/// A protobuf message serializer integrated with the confluent schema registry
/// </summary>
public class ConfluentProtobufSerializer : ISerializer
{
    private readonly ISchemaRegistryClient _schemaRegistryClient;
    private readonly ProtobufSerializerConfig _serializerConfig;
    private readonly RuleRegistry _ruleRegistry;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfluentProtobufSerializer"/> class.
    /// </summary>
    /// <param name="resolver">An instance of <see cref="IDependencyResolver"/></param>
    /// <param name="serializerConfig">An instance of <see cref="ProtobufSerializerConfig"/></param>
    /// <param name="ruleRegistry">An instance of <see cref="RuleRegistry"/>.</param>
    public ConfluentProtobufSerializer(IDependencyResolver resolver, ProtobufSerializerConfig serializerConfig = null, RuleRegistry ruleRegistry = null)
    {
        _schemaRegistryClient =
            resolver.Resolve<ISchemaRegistryClient>() ??
            throw new InvalidOperationException(
                $"No schema registry configuration was found. Set it using {nameof(ClusterConfigurationBuilderExtensions.WithSchemaRegistry)} on cluster configuration");

        _serializerConfig = serializerConfig;
        _ruleRegistry = ruleRegistry;
    }

    /// <inheritdoc/>
    public Task SerializeAsync(object message, Stream output, ISerializerContext context)
    {
        return ConfluentSerializerWrapper
            .GetOrCreateSerializer(
                message.GetType(),
                () => Activator.CreateInstance(
                    typeof(ProtobufSerializer<>).MakeGenericType(message.GetType()),
                    _schemaRegistryClient,
                    _serializerConfig,
                    _ruleRegistry))
            .SerializeAsync(message, output, context);
    }
}
