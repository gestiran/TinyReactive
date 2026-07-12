// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.
    
#if EXTERNAL_DEPENDENCIES
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TinyReactive.Fields;

namespace TinyReactive.JsonConverters {
    public sealed class ObservedListJsonConverter<T> : JsonConverter<ObservedList<T>> {
        public override ObservedList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            List<T> values = JsonSerializer.Deserialize<List<T>>(ref reader, options);
            
            if (values == null) {
                return new ObservedList<T>();
            }
            
            return new ObservedList<T>(values);
        }
        
        public override void Write(Utf8JsonWriter writer, ObservedList<T> value, JsonSerializerOptions options) {
            if (value == null) {
                writer.WriteNullValue();
                return;
            }
            
            JsonSerializer.Serialize(writer, value.list, options);
        }
    }
    
    public sealed class ObservedListJsonConverter : JsonConverterFactory {
        public override bool CanConvert(Type typeToConvert) {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ObservedList<>);
        }
        
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
            Type innerType = typeToConvert.GetGenericArguments()[0];
            Type converterType = typeof(ObservedListJsonConverter<>).MakeGenericType(innerType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }
}
#endif