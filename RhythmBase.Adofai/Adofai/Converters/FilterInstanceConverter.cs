using RhythmBase.Adofai.Components.Filters;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Adofai.Converters;

internal abstract class FilterMemberConverterBase
{
	public abstract IFilter ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options);
	public abstract void WriteProperties(Utf8JsonWriter writer, IFilter value, MetadataJsonSerializerOptions options);
}
internal abstract class FilterMemberConverter<TFilter> : FilterMemberConverterBase where TFilter : IFilter, new()
{
	public sealed override IFilter ReadProperties(ref Utf8JsonReader reader, MetadataJsonSerializerOptions options)
	{
		TFilter value = new();
		while (reader.TokenType is JsonTokenType.PropertyName)
		{
			string pn = reader.GetString() ?? "";
			if (!Read(ref reader, ref value, options))
			{
#if DEBUG
				if (!(false
					))
					Console.WriteLine($"The key {pn} of {value.GetType().Name} not found.");
#endif
				reader.Read();
				reader.Skip();
			}
		}
		return value;
	}
	public sealed override void WriteProperties(Utf8JsonWriter writer, IFilter value, MetadataJsonSerializerOptions options)
	{
		TFilter f = (TFilter)value;
		Write(writer, ref f, options);
	}
	protected virtual bool Read(ref Utf8JsonReader reader, ref TFilter value, MetadataJsonSerializerOptions options) { return false; }
	protected virtual void Write(Utf8JsonWriter writer, ref TFilter value, MetadataJsonSerializerOptions options) { }
}
