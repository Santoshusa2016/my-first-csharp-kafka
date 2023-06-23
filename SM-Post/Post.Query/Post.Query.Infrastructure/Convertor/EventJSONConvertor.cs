using CQRS.Core.Events;
using Post.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Convertor
{
    public class EventJSONConverter : JsonConverter<BaseEvent>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        }

        public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader,out var doc))
            {
                throw new JsonException($"failed to parse {nameof(JsonDocument)}");
            }

            if (doc.RootElement.TryGetProperty("Type", out var jsonElement))
            {
                //this is the string type in baseEvent class
                throw new JsonException($"could not detect concrete type");
            }


            var typeDiscriminator = jsonElement.GetString();
            var json = doc.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json,options),
                nameof(MessageUpdateEvent) => JsonSerializer.Deserialize<MessageUpdateEvent>(json, options),
                nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
                nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
                nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json, options),
                nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
                nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
                _ => throw new JsonException($"{typeDiscriminator} not supported")
            };
        }






        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {

            


        }
    }
}
