using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using RhythmBase.Rizline.Events;

namespace RhythmBase.Rizline.Converters
{
	internal class MemberConverterBaseNote<T> : MemberConverter<T> where T : BaseNote, new()
	{
		protected override bool Read(ref Utf8JsonReader reader, ref T value, MetadataJsonSerializerOptions options)
		{
			return base.Read(ref reader, ref value, options);
		}
		protected override void Write(Utf8JsonWriter writer, ref T value, MetadataJsonSerializerOptions options)
		{
			writer.WriteNumber("type"u8, (int)value.Type);
			base.Write(writer, ref value, options);
		}
	}
}
