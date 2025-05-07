
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace My
{

    public class AsIntConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType.CanAsJsonInt(acceptString: true);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
            case JsonToken.Integer:
            case JsonToken.Float: // Accepts numbers like 4.00
            case JsonToken.Null:
                return serializer.Deserialize(reader, objectType);
            case JsonToken.String:
                if (objectType == typeof(Int32))
                    return Int32.Parse((string)reader.Value!);
                if (objectType == typeof(Int64))
                    return Int64.Parse((string)reader.Value!);
                if (objectType == typeof(UInt32))
                    return UInt32.Parse((string)reader.Value!);
                if (objectType == typeof(UInt64))
                    return UInt64.Parse((string)reader.Value!);
                throw new JsonSerializationException(string.Format("Field {0} of type {1} is not a JSON integer", reader.Path, objectType));
            default:
                    throw new JsonSerializationException(string.Format("Token \"{0}\" of type {1} was not a JSON integer", reader.Value, reader.TokenType));
            }
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) => throw new NotImplementedException();
    }

}