using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.RhythmDoctor.Events;

/// <summary>
/// Represents an event to hide or show a row.
/// </summary>
[JsonObjectSerializable]
public record class HideRow : BaseRowAction
{
	internal sealed class HideRowShowPropertyConverter : JsonConverter<ShowTargetType>
	{
		public override ShowTargetType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			JsonException.ThrowIfNotMatch(ref reader, JsonTokenType.String, JsonTokenType.True, JsonTokenType.False);
			return reader.TokenType switch
			{
				JsonTokenType.String => TryParse(ref reader, out ShowTargetType result) ? result : ShowTargetType.Hidden,
				JsonTokenType.True => ShowTargetType.Visible,
				JsonTokenType.False => ShowTargetType.Hidden,
				_ => ShowTargetType.Hidden,
			};
		}
		public override void Write(Utf8JsonWriter writer, ShowTargetType value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToEnumUtf8String());
		}
	}
	/// <summary>
	/// Gets or sets the transition type for hiding the row.
	/// </summary>
	public Transition Transition { get; set; } = Transition.Smooth;
	/// <summary>
	/// Gets or sets the visibility state of the row.
	/// </summary>
	[JsonConverter(typeof(HideRowShowPropertyConverter))]
	public ShowTargetType Show { get; set; } = ShowTargetType.Hidden;
	/// <summary>
	/// Gets the type of the event.
	/// </summary>
	public override EventType Type => EventType.HideRow;
	/// <summary>
	/// Gets the tab category of the event.
	/// </summary>
	public override Tab Tab => Tab.Actions;
}