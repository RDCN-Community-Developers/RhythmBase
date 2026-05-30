namespace RhythmBase.RhythmDoctor.Converters;

//internal class TagActionTypeConverter : JsonConverter<ActionTagAction>
//{
//	public override ActionTagAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//	{
//		ReadOnlySpan<byte> value = reader.ValueSpan;
//		ActionTagAction result = 0;
//		switch(value)
//		{
//			case var v when v.StartsWith("Run"u8):
//				result |= ActionTagAction.Run;
//				value = value[3..];
//				break;
//				case var v when v.StartsWith("Enable"u8):
//				result |= ActionTagAction.Enable;
//				value = value[6..];
//				break;
//				case var v when v.StartsWith("Disable"u8):
//				result |= ActionTagAction.Disable;
//				value = value[7..];
//				break;
//			default:
//				throw new ConvertingException("Cannot read the TagAction.");
//		}
//		switch(value)
//		{
//			case var v when v.IsEmpty:
//				break;
//			case var v when v.SequenceEqual("All"u8):
//				result |= ActionTagAction.All;
//				break;
//			default:
//				throw new ConvertingException("Cannot read the TagAction.");
//		}
//		return result;
//	}

//	public override void Write(Utf8JsonWriter writer, ActionTagAction value, JsonSerializerOptions options)
//	{
//		bool isAll = value.HasFlag(ActionTagAction.RunAll);
//		ActionTagAction action = value & (ActionTagAction.Run | ActionTagAction.Enable | ActionTagAction.Disable);
//		writer.WriteStringValue(
//			isAll
//				? action.ToString() + "All"
//				: action.ToString()
//			);
//	}
//}
