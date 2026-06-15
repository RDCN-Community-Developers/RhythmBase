using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RhythmBase.Rizline.Converters
{
	internal class HoldConverter : MemberConverterBaseNote<RhythmBase.Rizline.Events.Hold>
	{
		protected override bool Read(ref Utf8JsonReader reader, ref RhythmBase.Rizline.Events.Hold value, global::RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
		{
			if (base.Read(ref reader, ref value, options))
				return true;
			if (reader.ValueTextEquals("otherInformations"u8) && reader.Read())
			{
				JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.StartArray);
				reader.Read();
				value.EndCanvasindex = reader.GetSingle();
				reader.Read();
				value.EndTime = reader.GetSingle();
				reader.Read();
				value.Height = reader.GetSingle();
				reader.Read();
			}
			//if (propertyName.SequenceEqual("endCanvasindex"u8))
			//	value.EndCanvasindex = reader.GetSingle();
			//else if (propertyName.SequenceEqual("endTime"u8))
			//	value.EndTime = reader.GetSingle();
			//else if (propertyName.SequenceEqual("height"u8))
			//	value.Height = reader.GetSingle();
			else return false;
			return true;
		}
		protected override void Write(Utf8JsonWriter writer, ref RhythmBase.Rizline.Events.Hold value, global::RhythmBase.Global.Converters.MetadataJsonSerializerOptions options)
		{
			base.Write(writer, ref value, options);
			{ writer.WriteNumber("endCanvasindex"u8, value.EndCanvasindex); }
			{ writer.WriteNumber("endTime"u8, value.EndTime); }
			{ writer.WriteNumber("height"u8, value.Height); }
		}
	}
}
