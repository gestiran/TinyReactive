// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.
    
#if EXTERNAL_DEPENDENCIES
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyReactive.Fields;

namespace TinyReactive.JsonConverters {
    public sealed class ObservedJsonConverter<T> : JsonConverter<Observed<T>> {
        public override Observed<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return new Observed<T>(JsonSerializer.Deserialize<T>(ref reader, options));
        }
        
        public override void Write(Utf8JsonWriter writer, Observed<T> value, JsonSerializerOptions options) {
            if (value == null) {
                writer.WriteNullValue();
                return;
            }
            
            JsonSerializer.Serialize(writer, value.value, typeof(T), options);
        }
    }
    
    public sealed class ObservedJsonConverter : JsonConverterFactory {
        public override bool CanConvert(Type typeToConvert) {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Observed<>);
        }
        
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
            Type innerType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(ObservedJsonConverter<>).MakeGenericType(innerType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
#endif