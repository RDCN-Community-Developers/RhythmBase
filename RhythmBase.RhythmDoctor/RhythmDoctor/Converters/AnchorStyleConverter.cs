namespace RhythmBase.RhythmDoctor.Converters;

//[RDJsonConverterFor(typeof(FloatingTextAnchorStyle))]
//internal class AnchorStyleConverter : JsonConverter<FloatingTextAnchorStyle>
//{
//	public override void Write(Utf8JsonWriter writer, FloatingTextAnchorStyle value, JsonSerializerOptions serializer)
//	{
//		FloatingTextAnchorStyle horizontal = value & (FloatingTextAnchorStyle.Left | FloatingTextAnchorStyle.Right);
//		FloatingTextAnchorStyle vertical = value & (FloatingTextAnchorStyle.Upper | FloatingTextAnchorStyle.Lower);
//		writer.WriteStringValue(
//			(vertical == 0 ?
//				"Middle"
//				: vertical.ToString())
//			+ horizontal.ToString()
//		);
//	}
//	public override FloatingTextAnchorStyle Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions serializer)
//	{
//		ReadOnlySpan<byte> value = reader.ValueSpan;
//		FloatingTextAnchorStyle result = FloatingTextAnchorStyle.Center;
//		switch (value)
//		{
//			case var v when v.StartsWith("Upper"u8):
//				result |= FloatingTextAnchorStyle.Upper;
//				value = value[5..];
//				break;
//			case var v when v.StartsWith("Lower"u8):
//				result |= FloatingTextAnchorStyle.Lower;
//				value = value[5..];
//				break;
//			case var v when v.StartsWith("Middle"u8):
//				value = value[6..];
//				break;
//			default:
//				throw new ConvertingException("Cannot read the anchor.");
//		}
//		switch (value)
//		{
//			case var v when v.SequenceEqual("Left"u8):
//				result |= FloatingTextAnchorStyle.Left;
//				break;
//			case var v when v.SequenceEqual("Right"u8):
//				result |= FloatingTextAnchorStyle.Right;
//				break;
//			case var v when v.SequenceEqual("Center"u8):
//				break;
//			default:
//				throw new ConvertingException("Cannot read the anchor.");
//		}
//		return result;
//	}
//}
