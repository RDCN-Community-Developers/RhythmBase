using static RhythmBase.Global.Extensions.Extensions;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Represents a rich string style.
/// </summary>
public record struct RichStringStyle : IRichStringStyle<RichStringStyle>
{
	/// <summary>
	/// 获取或设置文本的颜色。
	/// </summary>
	public Color? Color { get; set; }
	/// <inheritdoc/>
#if NET7_0_OR_GREATER
	static
#else
	readonly
#endif
	public bool HasPhrase => false;
	/// <inheritdoc/>
#if NET7_0_OR_GREATER
	static
#else
	readonly
#endif
	public string GetXmlTag(RichStringStyle before, RichStringStyle after)
	{
		string tag = "";
		TryAddTag(ref tag, "color",
			before.Color?.TryGetName(out string[] namesbefore) is true
			? namesbefore[0].ToLower()
			: before.Color?.ToString(before.Color?.A == 255 ? "#RRGGBB" : "#RRGGBBAA"),
			after.Color?.TryGetName(out string[] namesafter) is true
			? namesafter[0].ToLower()
			: after.Color?.ToString(after.Color?.A == 255 ? "#RRGGBB" : "#RRGGBBAA"));
		return tag;
	}
	/// <inheritdoc/>
	public bool ResetProperty(ReadOnlySpan<char> name)
	{
		switch (name)
		{
			case "color":
				Color = null;
				break;
			default:
				return false;
		}
		return true;
	}
	/// <inheritdoc/>
	public bool SetProperty(ReadOnlySpan<char> name, ReadOnlySpan<char> value)
	{
		switch (name)
		{
			case "color":
				if (Components.Color.TryFromName(value, out Color color))
                    Color = color;
				else if (Components.Color.TryFromRgba(value.ToString(), out color))
                    Color = color;
				else
					return false;
				break;
			default:
				return false;
		}
		return true;
	}
	/// <inheritdoc/>
	public readonly override int GetHashCode() => Color.GetHashCode();
}
