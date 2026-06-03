using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Converters
{
	internal class MemberConverterBaseNote : MemberConverter<BaseNote>
	{
		protected override bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref BaseNote value, MetadataJsonSerializerOptions options)
		{
			JsonException.ThrowIfNotMatch(reader, [JsonTokenType.StartObject]);
			Utf8JsonReader checkpoint = reader;
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.PropertyName && reader.ValueTextEquals(propertyName))
				{
					return true;
				}
			}
		}
		protected override void Write(Utf8JsonWriter writer, ref BaseNote value, MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
		}
	}
}
