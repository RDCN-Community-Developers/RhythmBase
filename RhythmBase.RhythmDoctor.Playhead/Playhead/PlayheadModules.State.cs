using System.Text.Json;
using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Lightweight conductor state of a playhead.
/// </summary>
public readonly struct PlayheadState
{
	/// <summary>Gets the current time.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the current bar.</summary>
	public int Bar { get; init; }
	/// <summary>Gets the current beat within the bar.</summary>
	public float Beat { get; init; }
	/// <summary>Gets the current play style.</summary>
	public PlayStyleType PlayStyle { get; init; }
}

/// <summary>
/// A scalar that tweens between two values over a number of ticks (crotchets).
/// </summary>
internal struct FloatTween
{
	private float _from;
	private float _to;
	private float _start;
	private float _duration;
	private EaseType _ease;

	internal bool Active;

	internal static FloatTween Start(float from, float to, float startTick, float duration, EaseType ease)
	{
		return new FloatTween
		{
			_from = from,
			_to = to,
			_start = startTick,
			_duration = duration,
			_ease = ease,
			Active = true,
		};
	}

	internal static FloatTween Constant(float value) => new() { _from = value, _to = value, _ease = EaseType.Linear, Active = false };

	internal readonly float At(float tick)
	{
		if (!Active || _duration <= 0f)
			return _to;
		float t = (tick - _start) / _duration;
		t = t < 0f ? 0f : t > 1f ? 1f : t;
		float eased = _ease == EaseType.Unset ? t : (float)_ease.Calculate(t);
		return _from + ((_to - _from) * eased);
	}
}

/// <summary>
/// A decoration's transform at a point in time. Positions, sizes and origins are percentages.
/// </summary>
public readonly struct DecorationTransform
{
	/// <summary>Gets the X position (percent).</summary>
	public float X { get; init; }
	/// <summary>Gets the Y position (percent).</summary>
	public float Y { get; init; }
	/// <summary>Gets the width (percent).</summary>
	public float Width { get; init; }
	/// <summary>Gets the height (percent).</summary>
	public float Height { get; init; }
	/// <summary>Gets the origin X (percent).</summary>
	public float OriginX { get; init; }
	/// <summary>Gets the origin Y (percent).</summary>
	public float OriginY { get; init; }
	/// <summary>Gets the angle in degrees.</summary>
	public float Angle { get; init; }
}

/// <summary>
/// The state of a decoration at a point in time.
/// </summary>
public readonly struct DecorationSnapshot
{
	/// <summary>Gets the decoration id.</summary>
	public string Id { get; init; }
	/// <summary>Gets the decoration room.</summary>
	public SingleRoom Room { get; init; }
	/// <summary>Gets the decoration room index.</summary>
	public int RoomIndex { get; init; }
	/// <summary>Gets the decoration type.</summary>
	public DecorationType Type { get; init; }
	/// <summary>Gets the sorting layer. Sprites use <see cref="LayerType.Default"/>.</summary>
	public LayerType Layer { get; init; }
	/// <summary>Gets the depth (sorting order) within the layer.</summary>
	public int Depth { get; init; }
	/// <summary>Gets whether the decoration is visible.</summary>
	public bool Visible { get; init; }
	/// <summary>Gets the transform.</summary>
	public DecorationTransform Transform { get; init; }
	/// <summary>Gets the tint color, when a tint has been applied.</summary>
	public PaletteColorWithAlpha? TintColor { get; init; }
	/// <summary>Gets the opacity (0-100).</summary>
	public int Opacity { get; init; }
	/// <summary>Gets whether tinting is enabled.</summary>
	public bool IsTint { get; init; }
	/// <summary>Gets whether text content has been set.</summary>
	public bool HasText { get; init; }
	/// <summary>Gets the text content.</summary>
	public string Text { get; init; }
	/// <summary>Gets the text size.</summary>
	public int TextSize { get; init; }
	/// <summary>Gets the text color.</summary>
	public PaletteColorWithAlpha TextColor { get; init; }
	/// <summary>Gets the text outline color.</summary>
	public PaletteColorWithAlpha OutlineColor { get; init; }
	/// <summary>Gets the font.</summary>
	public string Font { get; init; }
	/// <summary>Gets how many times the text has been advanced.</summary>
	public int AdvanceCount { get; init; }
	/// <summary>Gets whether an animation expression has been played.</summary>
	public bool HasExpression { get; init; }
	/// <summary>Gets the current animation expression name.</summary>
	public string Expression { get; init; }
	/// <summary>Gets how many ticks (beats) have elapsed since the expression last changed.</summary>
	public float ExpressionElapsedTicks { get; init; }
}

/// <summary>
/// The state of every decoration at a point in time.
/// </summary>
public readonly struct DecorationsSnapshot
{
	/// <summary>Gets the decoration snapshots.</summary>
	public IReadOnlyList<DecorationSnapshot> Items { get; init; }
}

/// <summary>
/// The state of one VFX preset on one room.
/// </summary>
public readonly struct VfxPresetState
{
	/// <summary>Gets the room index.</summary>
	public int Room { get; init; }
	/// <summary>Gets the preset.</summary>
	public VfxPreset Preset { get; init; }
	/// <summary>Gets whether the preset is enabled.</summary>
	public bool Enabled { get; init; }
	/// <summary>Gets the intensity.</summary>
	public float Intensity { get; init; }
	/// <summary>Gets the threshold.</summary>
	public float Threshold { get; init; }
}

/// <summary>
/// The layered VFX preset state of every room.
/// </summary>
public readonly struct VfxSnapshot
{
	/// <summary>Gets the active preset layers.</summary>
	public IReadOnlyList<VfxPresetState> Presets { get; init; }
}

/// <summary>
/// The VFX preset layers of one room.
/// </summary>
public readonly struct VfxRoomSnapshot
{
	/// <summary>Gets the room index.</summary>
	public int Room { get; init; }
	/// <summary>Gets the active preset layers of the room.</summary>
	public IReadOnlyList<VfxPresetState> Presets { get; init; }
}

/// <summary>
/// A recorded audio command.
/// </summary>
public readonly struct AudioCommand
{
	/// <summary>Gets the time of the command.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the event type.</summary>
	public EventType Type { get; init; }
	/// <summary>Gets the audio file name.</summary>
	public string? Filename { get; init; }
	/// <summary>Gets the volume.</summary>
	public float Volume { get; init; }
	/// <summary>Gets the pitch.</summary>
	public float Pitch { get; init; }
	/// <summary>Gets the pan.</summary>
	public float Pan { get; init; }
	/// <summary>Gets the start offset into the audio file.</summary>
	public TimeSpan Offset { get; init; }
	/// <summary>Gets whether the audio should loop (songs).</summary>
	public bool Loop { get; init; }
	/// <summary>
	/// Gets the scheduled performance tick. It may be later than <see cref="TickTime"/> for beat
	/// sounds, whose pulses the module emits ahead of time.
	/// </summary>
	public float Tick { get; init; }
	/// <summary>Gets the official game sound type, when the command refers to one.</summary>
	public SoundType? SoundType { get; init; }
}

/// <summary>
/// The audio state at a point in time.
/// </summary>
public readonly struct AudioSnapshot
{
	/// <summary>Gets the current song file name.</summary>
	public string? CurrentSong { get; init; }
	/// <summary>Gets whether a song is playing.</summary>
	public bool IsPlaying { get; init; }
	/// <summary>Gets the song volume.</summary>
	public float SongVolume { get; init; }
	/// <summary>Gets the song pitch.</summary>
	public float SongPitch { get; init; }
	/// <summary>Gets the song pan.</summary>
	public float SongPan { get; init; }
	/// <summary>Gets the audio commands seen so far.</summary>
	public IReadOnlyList<AudioCommand> Commands { get; init; }
	/// <summary>Gets player 1's clap sound.</summary>
	public AudioStateSnapshot? ClapP1 { get; init; }
	/// <summary>Gets player 2's clap sound.</summary>
	public AudioStateSnapshot? ClapP2 { get; init; }
	/// <summary>Gets the CPU clap sound.</summary>
	public AudioStateSnapshot? ClapCpu { get; init; }
	/// <summary>Gets the clap row type.</summary>
	public RowType? ClapRowType { get; init; }
	/// <summary>Gets whether the counting sound is enabled.</summary>
	public bool? CountingEnabled { get; init; }
	/// <summary>Gets the counting sound voice source.</summary>
	public CountingSoundVoiceSource? CountingVoice { get; init; }
	/// <summary>Gets the counting sound volume.</summary>
	public int? CountingVolume { get; init; }
	/// <summary>Gets the beat sound.</summary>
	public AudioStateSnapshot? BeatSound { get; init; }
	/// <summary>Gets the last game sound type.</summary>
	public SoundType? GameSoundType { get; init; }
	/// <summary>Gets the last game sound.</summary>
	public AudioStateSnapshot? GameSound { get; init; }
}

/// <summary>
/// The playback parameters of one audio.
/// </summary>
public readonly struct AudioStateSnapshot
{
	/// <summary>Gets the file name.</summary>
	public string? Filename { get; init; }
	/// <summary>Gets the volume.</summary>
	public float Volume { get; init; }
	/// <summary>Gets the pitch.</summary>
	public float Pitch { get; init; }
	/// <summary>Gets the pan.</summary>
	public float Pan { get; init; }
}

/// <summary>
/// The conductor snapshot.
/// </summary>
public readonly struct ConductorSnapshot
{
	/// <summary>Gets the time.</summary>
	public TickTime TickTime { get; init; }
	/// <summary>Gets the beats per minute.</summary>
	public float BeatsPerMinute { get; init; }
	/// <summary>Gets the crotchets per bar.</summary>
	public int CrotchetsPerBar { get; init; }
	/// <summary>Gets the speed.</summary>
	public float Speed { get; init; }
	/// <summary>Gets the play style.</summary>
	public PlayStyleType PlayStyle { get; init; }
}
