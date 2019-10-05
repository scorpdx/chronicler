﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chronicler.Converters
{
    public class ChronicleCollectionConverter : JsonConverter<ChronicleCollection>
    {
        public override ChronicleCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var chronicles = new List<Chronicle>();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName when reader.ValueTextEquals("chronicle"):
                        var chronicle = JsonSerializer.Deserialize<Chronicle>(ref reader, options);
                        chronicles.Add(chronicle);
                        break;
                }
            }

            return new ChronicleCollection
            {
                Chronicles = chronicles
            };
        }

        public override void Write(Utf8JsonWriter writer, ChronicleCollection value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
