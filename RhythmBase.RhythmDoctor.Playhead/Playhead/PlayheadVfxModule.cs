using System.Text.Json;
using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Tracks the layered VFX presets of every room.
/// </summary>
internal sealed class VfxModule : IPlayheadObject
{
	private readonly Dictionary<(int Room, VfxPreset Preset), VfxLayer> _layers = [];

	internal VfxModule(Playhead playhead)
	{
		Playhead = playhead;
	}

	private VfxModule(VfxModule source)
	{
		Playhead = source.Playhead;
		foreach (KeyValuePair<(int Room, VfxPreset Preset), VfxLayer> pair in source._layers)
			_layers[pair.Key] = pair.Value;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Vfx;

	public string Id => "vfx";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Vfx;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		Dictionary<int, List<VfxPresetState>> byRoom = [];
		foreach (KeyValuePair<(int Room, VfxPreset Preset), VfxLayer> pair in _layers)
		{
			int room = pair.Key.Room;
			if (!byRoom.TryGetValue(room, out List<VfxPresetState>? list))
			{
				list = [];
				byRoom[room] = list;
			}
			list.Add(new VfxPresetState
			{
				Room = room,
				Preset = pair.Key.Preset,
				Enabled = pair.Value.Enabled,
				Intensity = pair.Value.Intensity.At(tick),
				Threshold = pair.Value.Threshold.At(tick),
			});
		}
		List<PlayheadObjectSnapshot> objects = [];
		foreach (KeyValuePair<int, List<VfxPresetState>> pair in byRoom)
			objects.Add(new PlayheadObjectSnapshot { Id = "vfx.room" + pair.Key, Kind = PlayheadObjectKind.Vfx, State = new VfxRoomSnapshot { Room = pair.Key, Presets = pair.Value } });
		return objects;
	}

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } = [EventType.SetVfxPreset];

	public void MoveNext(IBaseEvent e)
	{
		if (e is not SetVfxPreset set)
			return;
		float tick = e.TickTime.Tick;
		foreach (byte room in set.Rooms.Rooms)
		{
			(int Room, VfxPreset Preset) key = (room, set.Preset);
			if (!_layers.TryGetValue(key, out VfxLayer layer))
				layer = new VfxLayer { Intensity = FloatTween.Constant(set.Intensity ?? 100f), Threshold = FloatTween.Constant(set.Threshold ?? 0f) };
			layer.Enabled = set.Enable;
			if (set.Intensity is { } intensity)
				layer.Intensity = FloatTween.Start(layer.Intensity.At(tick), intensity, tick, set.Duration, set.Ease);
			if (set.Threshold is { } threshold)
				layer.Threshold = FloatTween.Start(layer.Threshold.At(tick), threshold, tick, set.Duration, set.Ease);
			_layers[key] = layer;
		}
	}

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<VfxPresetState> states = [];
		foreach (KeyValuePair<(int Room, VfxPreset Preset), VfxLayer> pair in _layers)
		{
			(int room, VfxPreset preset) = pair.Key;
			VfxLayer layer = pair.Value;
			states.Add(new VfxPresetState
			{
				Room = room,
				Preset = preset,
				Enabled = layer.Enabled,
				Intensity = layer.Intensity.At(tick),
				Threshold = layer.Threshold.At(tick),
			});
		}
		return new VfxSnapshot { Presets = states };
	}

	public IPlayheadObject Clone(Playhead playhead) => new VfxModule(this) { Playhead = playhead };

	private struct VfxLayer
	{
		public bool Enabled;
		public FloatTween Intensity;
		public FloatTween Threshold;
	}
}
