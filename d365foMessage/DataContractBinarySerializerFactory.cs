using MassTransit;
using MassTransit.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace d365foMessage;

public class DataContractBinarySerializerFactory : ISerializerFactory
{
    private string _contentType = "application/vnd.ms-internal";
    public ContentType ContentType => new ContentType(_contentType);

    public IMessageDeserializer CreateDeserializer()
    {
        return new DataContractBinaryMessageDeserializer();
    }

    public IMessageSerializer CreateSerializer()
    {
        return new DataContractBinaryMessageDeserializer();
    }
}
public class DataContractBinaryMessageDeserializer :
        RawMessageSerializer,
        IMessageDeserializer,
        IMessageSerializer
{
    private RawSerializerOptions _rawSerializerOptions = RawSerializerOptions.Default;
    public ContentType ContentType => new ContentType("application/vnd.ms-internal");

    public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
    {
        var stream = body.GetStream();
        var datacontractSerializer = new DataContractBinarySerializer(typeof(string));
        var json = (string)datacontractSerializer.ReadObject(stream);

        JsonDocument document = JsonDocument.Parse(json);
        var jsonElement = document.RootElement;

        var messageTypes = headers.GetMessageTypes();

        var messageContext = new RawMessageContext(headers, destinationAddress, _rawSerializerOptions);

        var serializerContext = new SystemTextJsonRawSerializerContext(SystemTextJsonMessageSerializer.Instance,
            SystemTextJsonMessageSerializer.Options, ContentType, messageContext, messageTypes, _rawSerializerOptions, jsonElement);

        return serializerContext;
    }

    public ConsumeContext Deserialize(ReceiveContext receiveContext)
    {
        return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
    }

    public MessageBody GetMessageBody(string text)
    {
        return new StringMessageBody(text);
    }

    public MessageBody GetMessageBody<T>(SendContext<T> context) where T : class
    {
        if (_rawSerializerOptions.HasFlag(RawSerializerOptions.AddTransportHeaders))
            SetRawMessageHeaders<T>(context);

        return new SystemTextJsonRawMessageBody<T>(context, SystemTextJsonMessageSerializer.Options);
    }

    public void Probe(ProbeContext context)
    {
    }
}
