using RhythmBase.Global.Components.Easing;
namespace RhythmBase.Adofai.Events;

/// <summary>  
/// Represents an event that sets a filter effect in the Adofai level.  
/// </summary>  
[JsonObjectSerializable]
public class SetFilter : BaseTaggedTileEvent, IEaseEvent, IBeginningEvent
{
	/// <inheritdoc/>
	public override EventType Type => EventType.SetFilter;
	/// <summary>  
	/// Gets or sets the filter to be applied.  
	/// </summary>  
	public Filter Filter { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether the filter is enabled.  
	/// </summary>  
	public bool Enabled { get; set; } = true;
	/// <summary>  
	/// Gets or sets the intensity of the filter effect.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(Enabled)} &&
		$&.{nameof(Filter)} is
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.VHS)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.LED)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Drawing)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Compression)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Waves)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Pixelate)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Rain)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Blizzard)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.PixelSnow)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Static)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Grain)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.MotionBlur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Fisheye)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Aberration)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Sepia)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Grayscale)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.HexagonBlack)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Posterize)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Sharpen)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Contrast)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.OilPaint)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Blur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.BlurFocus)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.GaussianBlur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.WaterDrop)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.LightWater)}
		""")]
	public float Intensity { get; set; } = 100f;
	/// <summary>  
	/// Gets or sets the duration of the filter effect.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(Enabled)} &&
		$&.{nameof(Filter)} is
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.VHS)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.LED)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Drawing)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Compression)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Waves)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Pixelate)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Rain)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Blizzard)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.PixelSnow)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Static)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Grain)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.MotionBlur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Fisheye)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Aberration)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Sepia)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Grayscale)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.HexagonBlack)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Posterize)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Sharpen)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Contrast)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.OilPaint)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Blur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.BlurFocus)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.GaussianBlur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.WaterDrop)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.LightWater)}
		""")]
	public float Duration { get; set; }
	/// <summary>  
	/// Gets or sets the easing type for the filter effect.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(Enabled)} &&
		$&.{nameof(Filter)} is
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.VHS)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.LED)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Drawing)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Compression)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Waves)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Pixelate)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Rain)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Blizzard)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.PixelSnow)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Static)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Grain)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.MotionBlur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Fisheye)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Aberration)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Sepia)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Grayscale)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.HexagonBlack)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Posterize)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Sharpen)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Contrast)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.OilPaint)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.Blur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.BlurFocus)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.GaussianBlur)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.WaterDrop)} or
			RhythmBase.Adofai.{nameof(Adofai.Filter)}.{nameof(Filter.LightWater)}
		""")]
	public EaseType Ease { get; set; }
	/// <summary>  
	/// Gets or sets a value indicating whether other filters should be disabled.  
	/// </summary>  
	public bool DisableOthers { get; set; }
}