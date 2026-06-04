using RhythmBase.Global.Components.Easing;
using RhythmBase.RhythmDoctor.Components;
namespace RhythmBase.RhythmDoctor.Events;

/// <summary>  
/// Represents an event to set a theme in a room.  
/// </summary>  
[JsonObjectSerializable]
public record class SetTheme : BaseEvent, IRoomEvent, IEaseEvent
{
	/// <summary>  
	/// Gets or sets the theme preset.  
	/// </summary>  
	public Theme Preset { get; set; } = Theme.None;
	/// <summary>  
	/// Gets or sets the variant of the theme.  
	/// </summary>  
	[JsonCondition($"""
		$&.{nameof(Preset)}
		is RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.Intimate)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.InsomniacDay)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.CrossesStraight)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.CubesFalling)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.Rooftop)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.Sky)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.CoffeeShop)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.Garden)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.TrainDay)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.DesertDay)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.HospitalWard)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.ColeWardNight)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.Skyline)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.FloatingHeart)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.FloatingHeartBroken)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.Stadium)}
		or RhythmBase.RhythmDoctor.{nameof(Theme)}.{nameof(Theme.AthleteWard)}
		""")]
	public byte Variant { get; set; }
	/// <summary>
	/// Gets or sets a value indicating whether the theme's horizontal position is enabled.
	/// </summary>
	[JsonCondition($"RhythmBase.RhythmDoctor.Events.{nameof(SetTheme)}.{nameof(ThemesHasPosition)}.{nameof(ReadOnlyEnumCollection<>.Contains)}($&.{nameof(Preset)})")]
	public bool EnablePosition { get; set; }
	/// <summary>
	/// Gets or sets the horizontal position offset (X) for themes that support positioning.
	/// </summary>
	[JsonCondition($"""
		$&.{nameof(EnablePosition)} &&
		RhythmBase.RhythmDoctor.Events.{nameof(SetTheme)}.{nameof(ThemesHasPosition)}.{nameof(ReadOnlyEnumCollection<>.Contains)}($&.{nameof(Preset)})
		""")]
	public float PositionX { get; set; }
	///<inheritdoc/>
	public override EventType Type => EventType.SetTheme;
	///<inheritdoc/>
	public override Tab Tab => Tab.Actions;
	///<inheritdoc/>
	public Room Rooms { get; set; } = new Room([0]);
	/// <summary>
	/// Gets or sets a value indicating whether to skip paint effects.
	/// </summary>
	public bool SkipPaintEffects { get; set; } = false;
	/// <summary>
	/// Gets or sets a value indicating whether the first row is positioned on the floor.
	/// </summary>
	[JsonCondition($"$&.{nameof(FirstRowOnFloor)}")]
	public bool FirstRowOnFloor { get; set; } = false;
	///<inheritdoc/>
	[JsonAlias("positionEase")]
	[JsonCondition($"""
		$&.{nameof(EnablePosition)} &&
		RhythmBase.RhythmDoctor.Events.{nameof(SetTheme)}.{nameof(ThemesHasPosition)}.{nameof(ReadOnlyEnumCollection<>.Contains)}($&.{nameof(Preset)})
		""")]
	public EaseType Ease { get; set; }
	///<inheritdoc/>
	[JsonAlias("positionDuration")]
	[JsonCondition($"""
		$&.{nameof(EnablePosition)} &&
		RhythmBase.RhythmDoctor.Events.{nameof(SetTheme)}.{nameof(ThemesHasPosition)}.{nameof(ReadOnlyEnumCollection<>.Contains)}($&.{nameof(Preset)})
		""")]
	public float Duration { get; set; }
	///<inheritdoc/>
	public override string ToString() => base.ToString() + $" {Preset}";
	internal static readonly ReadOnlyEnumCollection<Theme> ThemesHasPosition = [
		Theme.TrainDay,
		Theme.TrainNight,
		Theme.DesertDay,
		Theme.DesertNight,
		Theme.HospitalWard,
		Theme.PaigeOffice,
		Theme.Basement,
		Theme.RollerDisco,
		Theme.AthleteWard,
		Theme.AthleteWardNight,
		Theme.Airport,
		Theme.RecordsRoom,
		Theme.AbandonedWard];
}