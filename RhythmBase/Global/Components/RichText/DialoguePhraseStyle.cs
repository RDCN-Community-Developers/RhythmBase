using static RhythmBase.Global.Extensions.Extensions;

namespace RhythmBase.Global.Components.RichText;

/// <summary>
/// Represents the style of a rich string.
/// </summary>
public record struct DialoguePhraseStyle : IRichStringStyle<DialoguePhraseStyle>
{
	/// <summary>
	/// Gets or sets the color of the text.
	/// </summary>
	public Color? Color { get; set; }
	/// <summary>
	/// Gets or sets the speed of the text animation.
	/// </summary>
	public float? Speed { get; set; }
	/// <summary>
	/// Gets or sets the volume of the text.
	/// </summary>
	public float? Volume { get; set; }
	/// <summary>
	/// Gets or sets the pitch of the text.
	/// </summary>
	public float? Pitch { get; set; }
	/// <summary>
	/// Gets or sets the pitch range of the text.
	/// </summary>
	public float? PitchRange { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should shake.
	/// </summary>
	public bool Shake { get; set; }
	/// <summary>
	/// Gets or sets the radius of the shake effect.
	/// </summary>
	public float? ShakeRadius { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should have a wave effect.
	/// </summary>
	public bool Wave { get; set; }
	/// <summary>
	/// Gets or sets the height of the wave effect.
	/// </summary>
	public float? WaveHeight { get; set; }
	/// <summary>
	/// Gets or sets the speed of the wave effect.
	/// </summary>
	public float? WaveSpeed { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should have a swirl effect.
	/// </summary>
	public bool Swirl { get; set; }
	/// <summary>
	/// Gets or sets the radius of the swirl effect.
	/// </summary>
	public float? SwirlRadius { get; set; }
	/// <summary>
	/// Gets or sets the speed of the swirl effect.
	/// </summary>
	public float? SwirlSpeed { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should be sticky.
	/// </summary>
	public bool Sticky { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should be loud.
	/// </summary>
	public bool Loud { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should be bold.
	/// </summary>
	public bool Bold { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the text should be whispered.
	/// </summary>
	public bool Whisper { get; set; }
	/// <inheritdoc/>
	public
#if !NETSTANDARD
		static
#else
	readonly
#endif
		bool HasPhrase => true;
	/// <summary>
	/// Sets the property of the rich string style based on the provided name and value.
	/// </summary>
	/// <param name="name">The name of the property to set.</param>
	/// <param name="value">The value to set for the property.</param>
	/// <returns>True if the property was successfully set; otherwise, false.</returns>
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
			case "speed":
				if (!float.TryParse(value.ToString(), out float speed))
					return false;
				Speed = speed;
				break;
			case "volume":
				if(!float.TryParse(value.ToString(), out float volume))
					return false;
				Volume = volume;
				break;
			case "pitch":
				if(!float.TryParse(value.ToString(), out float pitch))
					return false;
				Pitch = pitch;
				break;
			case "pitchRange":
				if(!float.TryParse(value.ToString(), out float pitchRange))
					return false;
				PitchRange = pitchRange;
				break;
			case "shake":
				Shake = true;
				break;
			case "shakeRadius":
				if(!float.TryParse(value.ToString(), out float shakeRadius))
					return false;
				ShakeRadius = shakeRadius;
				break;
			case "wave":
				Wave = true;
				break;
			case "waveHeight":
				if(!float.TryParse(value.ToString(), out float waveHeight))
					return false;
				WaveHeight = waveHeight;
				break;
			case "waveSpeed":
				if(!float.TryParse(value.ToString(), out float waveSpeed))
					return false;
				WaveSpeed = waveSpeed;
				break;
			case "swirl":
				Swirl = true;
				break;
			case "swirlRadius":
				if(!float.TryParse(value.ToString(), out float swirlRadius))
					return false;
				SwirlRadius = swirlRadius;
				break;
			case "swirlSpeed":
				if(!float.TryParse(value.ToString(), out float swirlSpeed))
					return false;
				SwirlSpeed = swirlSpeed;
				break;
			case "sticky":
				Sticky = true;
				break;
			case "loud":
				Loud = true;
				break;
			case "bold":
				Bold = true;
				break;
			case "whisper":
				Whisper = true;
				break;
			default:
				return false;
		}
		return true;
	}
	/// <summary>
	/// Removes the property of the rich string style based on the provided name.
	/// </summary>
	/// <param name="name">The name of the property to remove.</param>
	/// <returns>True if the property was successfully removed; otherwise, false.</returns>
	public bool ResetProperty(ReadOnlySpan<char> name)
	{
		switch (name)
		{
			case "color":
				Color = null;
				break;
			case "speed":
				Speed = null;
				break;
			case "volume":
				Volume = null;
				break;
			case "pitch":
				Pitch = null;
				break;
			case "pitchRange":
				PitchRange = null;
				break;
			case "shake":
				Shake = false;
				break;
			case "shakeRadius":
				ShakeRadius = null;
				break;
			case "wave":
				Wave = false;
				break;
			case "waveHeight":
				WaveHeight = null;
				break;
			case "waveSpeed":
				WaveSpeed = null;
				break;
			case "swirl":
				Swirl = false;
				break;
			case "swirlRadius":
				SwirlRadius = null;
				break;
			case "swirlSpeed":
				SwirlSpeed = null;
				break;
			case "sticky":
				Sticky = false;
				break;
			case "loud":
				Loud = false;
				break;
			case "bold":
				Bold = false;
				break;
			case "whisper":
				Whisper = false;
				break;
			default:
				return false;
		}
		return true;
	}
	/// <inheritdoc/>
	public
#if NETSTANDARD
		readonly
#else
		static
#endif
		string GetXmlTag(DialoguePhraseStyle before, DialoguePhraseStyle after)
	{
		string tag = "";
		TryAddTag(ref tag, "color",
			before.Color?.TryGetName(out string[] namesbefore) == true
			? namesbefore[0].ToLower()
			: before.Color?.ToString(before.Color?.A == 255 ? "#RRGGBB" : "#RRGGBBAA"),
			after.Color?.TryGetName(out string[] namesafter) == true
			? namesafter[0].ToLower()
			: after.Color?.ToString(after.Color?.A == 255 ? "#RRGGBB" : "#RRGGBBAA"));
		TryAddTag(ref tag, "speed", before.Speed?.ToString(), after.Speed?.ToString());
		TryAddTag(ref tag, "volume", before.Volume?.ToString(), after.Volume?.ToString());
		TryAddTag(ref tag, "pitch", before.Pitch?.ToString(), after.Pitch?.ToString());
		TryAddTag(ref tag, "pitchRange", before.PitchRange?.ToString(), after.PitchRange?.ToString());
		TryAddTag(ref tag, "shake", before.Shake, after.Shake);
		TryAddTag(ref tag, "shakeRadius", before.ShakeRadius?.ToString(), after.ShakeRadius?.ToString());
		TryAddTag(ref tag, "wave", before.Wave, after.Wave);
		TryAddTag(ref tag, "waveHeight", before.WaveHeight?.ToString(), after.WaveHeight?.ToString());
		TryAddTag(ref tag, "waveSpeed", before.WaveSpeed?.ToString(), after.WaveSpeed?.ToString());
		TryAddTag(ref tag, "swirl", before.Swirl, after.Swirl);
		TryAddTag(ref tag, "swirlRadius", before.SwirlRadius?.ToString(), after.SwirlRadius?.ToString());
		TryAddTag(ref tag, "swirlSpeed", before.SwirlSpeed?.ToString(), after.SwirlSpeed?.ToString());
		TryAddTag(ref tag, "sticky", before.Sticky, after.Sticky);
		TryAddTag(ref tag, "loud", before.Loud, after.Loud);
		TryAddTag(ref tag, "bold", before.Bold, after.Bold);
		TryAddTag(ref tag, "whisper", before.Whisper, after.Whisper);
		return tag;
	}
	/// <inheritdoc/>
	public readonly override int GetHashCode()
	{
		return base.GetHashCode();
	}

}
