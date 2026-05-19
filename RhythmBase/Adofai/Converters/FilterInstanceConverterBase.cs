using RhythmBase.Adofai.Components.Filters;
using System.Text;
using System.Text.Json;

namespace RhythmBase.Adofai.Converters;

internal abstract class FilterInstanceConverterBase
{
	public abstract IFilter ReadProperties(ref Utf8JsonReader reader, RDJsonSerializerOptions options);
	public abstract void WriteProperties(Utf8JsonWriter writer, IFilter value, RDJsonSerializerOptions options);
}
internal abstract class FilterInstanceConverterBase<TFilter> : FilterInstanceConverterBase where TFilter : struct, IFilter
{
	public sealed override IFilter ReadProperties(ref Utf8JsonReader reader, RDJsonSerializerOptions options)
	{
		TFilter value = new();
		while (reader.TokenType is JsonTokenType.PropertyName)
		{
			ReadOnlySpan<byte> propertyName = reader.ValueSpan;
			if (propertyName.IsEmpty)
				throw new JsonException("Property name cannot be null");
			reader.Read();
			if (!Read(ref reader, propertyName, ref value, options))
			{
#if DEBUG
				if (!(false
					))
					Console.WriteLine($"The key {Encoding.UTF8.GetString([.. propertyName])} of {value.GetType().Name} not found.");
#endif
			}
		}
		return value;
	}
	public sealed override void WriteProperties(Utf8JsonWriter writer, IFilter value, RDJsonSerializerOptions options)
	{
		TFilter f = (TFilter)value;
		Write(writer, ref f, options);
	}
	protected virtual bool Read(ref Utf8JsonReader reader, ReadOnlySpan<byte> propertyName, ref TFilter value, RDJsonSerializerOptions options) { return false; }
	protected virtual void Write(Utf8JsonWriter writer, ref TFilter value, RDJsonSerializerOptions options) { }
}
