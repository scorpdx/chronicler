﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Chronicler
{
    /// <summary>
    /// JsonElement serialization that avoids string allocations
    /// </summary>
    /// <remarks>
    /// <para>
    /// Taken from https://github.com/dotnet/corefx/issues/37564#issuecomment-530361412
    /// </para>
    /// <para>
    /// Can be removed when System.Text.Json >= 5.0 drops
    /// </para>
    /// </remarks>
    internal readonly ref struct JsonElementSerializer
    {
        private static readonly FieldInfo JsonDocumentField = typeof(JsonElement).GetField("_parent", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo JsonDocumentUtf8JsonField = typeof(JsonDocument).GetField("_utf8Json", BindingFlags.NonPublic | BindingFlags.Instance);

        ReadOnlyMemory<byte> Value { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JsonElementSerializer(JsonElement jsonElement)
        {
            if (JsonDocumentField == null) throw new ArgumentNullException(nameof(JsonDocumentField));
            if (JsonDocumentUtf8JsonField == null) throw new ArgumentNullException(nameof(JsonDocumentUtf8JsonField));
            var jsonDocument = JsonDocumentField.GetValue(jsonElement);
            Value = (ReadOnlyMemory<byte>)JsonDocumentUtf8JsonField.GetValue(jsonDocument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ToObject<T>(JsonSerializerOptions jsonSerializerOptions = null)
        {
            return (T)ToObject(typeof(T), jsonSerializerOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object ToObject(Type type, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return JsonSerializer.Deserialize(Value.Span, type, jsonSerializerOptions);
        }
    }
}
