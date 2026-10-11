using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// The state of a room camera.
/// </summary>
public readonly struct CameraSnapshot
{
	/// <summary>Gets the camera X position (percent).</summary>
	public float X { get; init; }
	/// <summary>Gets the camera Y position (percent).</summary>
	public float Y { get; init; }
	/// <summary>Gets the camera zoom (percent).</summary>
	public float Zoom { get; init; }
	/// <summary>Gets the camera angle in degrees.</summary>
	public float Angle { get; init; }
	/// <summary>Gets the last pulse strength.</summary>
	public StrengthType PulseStrength { get; init; }
	/// <summary>Gets the last pulse count.</summary>
	public int PulseCount { get; init; }
	/// <summary>Gets the last pulse frequency.</summary>
	public float PulseFrequency { get; init; }
}

/// <summary>
/// The room perspective corners (percent, y up).
/// </summary>
public readonly struct PerspectiveSnapshot
{
	/// <summary>Gets the left-bottom X.</summary>
	public float LeftBottomX { get; init; }
	/// <summary>Gets the left-bottom Y.</summary>
	public float LeftBottomY { get; init; }
	/// <summary>Gets the right-bottom X.</summary>
	public float RightBottomX { get; init; }
	/// <summary>Gets the right-bottom Y.</summary>
	public float RightBottomY { get; init; }
	/// <summary>Gets the left-top X.</summary>
	public float LeftTopX { get; init; }
	/// <summary>Gets the left-top Y.</summary>
	public float LeftTopY { get; init; }
	/// <summary>Gets the right-top X.</summary>
	public float RightTopX { get; init; }
	/// <summary>Gets the right-top Y.</summary>
	public float RightTopY { get; init; }
}

/// <summary>
/// The state of one room at a point in time. Positions, sizes and origins are percentages.
/// </summary>
public readonly struct RoomSnapshot
{
	/// <summary>Gets the room index.</summary>
	public int Index { get; init; }
	/// <summary>Gets whether the room is visible.</summary>
	public bool Visible { get; init; }
	/// <summary>Gets the height percentage.</summary>
	public float Height { get; init; }
	/// <summary>Gets the X position (percent).</summary>
	public float X { get; init; }
	/// <summary>Gets the Y position (percent).</summary>
	public float Y { get; init; }
	/// <summary>Gets the width (percent).</summary>
	public float Width { get; init; }
	/// <summary>Gets the height scale (percent).</summary>
	public float HeightScale { get; init; }
	/// <summary>Gets the origin X (percent).</summary>
	public float OriginX { get; init; }
	/// <summary>Gets the origin Y (percent).</summary>
	public float OriginY { get; init; }
	/// <summary>Gets the angle in degrees.</summary>
	public float Angle { get; init; }
	/// <summary>Gets whether <see cref="X"/> was set by a MoveRoom event.</summary>
	public bool PositionXSet { get; init; }
	/// <summary>Gets whether <see cref="Y"/> was set by a MoveRoom event.</summary>
	public bool PositionYSet { get; init; }
	/// <summary>Gets whether <see cref="Width"/> was set by a MoveRoom event.</summary>
	public bool ScaleXSet { get; init; }
	/// <summary>Gets whether <see cref="HeightScale"/> was set by a MoveRoom event.</summary>
	public bool ScaleYSet { get; init; }
	/// <summary>Gets whether <see cref="Angle"/> was set by a MoveRoom event.</summary>
	public bool AngleSet { get; init; }
	/// <summary>Gets whether <see cref="OriginX"/>/<see cref="OriginY"/> were set by a MoveRoom event.</summary>
	public bool PivotSet { get; init; }
	/// <summary>Gets the content mode.</summary>
	public ContentMode ContentMode { get; init; }
	/// <summary>Gets the background color.</summary>
	public PaletteColorWithAlpha BackgroundColor { get; init; }
	/// <summary>Gets the background type.</summary>
	public BackgroundType BackgroundType { get; init; }
	/// <summary>Gets the background image files (<see cref="BackgroundType.Image"/>).</summary>
	public IReadOnlyList<FileReference> BackgroundImages { get; init; }
	/// <summary>Gets the background image content mode.</summary>
	public ContentMode BackgroundContentMode { get; init; }
	/// <summary>Gets the room camera state.</summary>
	public CameraSnapshot Camera { get; init; }
	/// <summary>Gets the room perspective corners.</summary>
	public PerspectiveSnapshot Perspective { get; init; }
	/// <summary>Gets the perspective corners composited with the room position and scale.</summary>
	public PerspectiveSnapshot EffectivePerspective { get; init; }
	/// <summary>Gets the room opacity (0-100).</summary>
	public float Opacity { get; init; }
	/// <summary>Gets whether a foreground is applied.</summary>
	public bool HasForeground { get; init; }
	/// <summary>Gets the foreground color.</summary>
	public PaletteColorWithAlpha ForegroundColor { get; init; }
	/// <summary>Gets the foreground image files.</summary>
	public IReadOnlyList<FileReference> ForegroundImages { get; init; }
	/// <summary>Gets the foreground image content mode.</summary>
	public ContentMode ForegroundContentMode { get; init; }
	/// <summary>Gets the current flash color.</summary>
	public PaletteColorWithAlpha FlashColor { get; init; }
	/// <summary>Gets the current flash opacity (0-100).</summary>
	public float FlashOpacity { get; init; }
	/// <summary>Gets whether the flash affects the background.</summary>
	public bool FlashBackground { get; init; }
}

/// <summary>The state of every room.</summary>
public readonly struct RoomsSnapshot
{
	/// <summary>Gets the room snapshots.</summary>
	public IReadOnlyList<RoomSnapshot> Items { get; init; }
	/// <summary>Gets the room order (slot -&gt; room index).</summary>
	public int[] Order { get; init; }
	/// <summary>Gets the top (HUD) camera state; <see cref="RoomIndex.RoomTop"/> targets this camera.</summary>
	public CameraSnapshot TopCamera { get; init; }
}

/// <summary>
/// The state of one row at a point in time. Positions, sizes and origins are percentages.
/// </summary>
public readonly struct RowSnapshot
{
	/// <summary>Gets the row index.</summary>
	public int Index { get; init; }
	/// <summary>Gets the room the row belongs to.</summary>
	public SingleRoom Room { get; init; }
	/// <summary>Gets whether the row is visible.</summary>
	public bool Visible { get; init; }
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
	/// <summary>Gets the tint color, when set.</summary>
	public PaletteColorWithAlpha? TintColor { get; init; }
	/// <summary>Gets the opacity (0-100).</summary>
	public int Opacity { get; init; }
	/// <summary>Gets the X pattern.</summary>
	public string? Pattern { get; init; }
	/// <summary>Gets the syncopation beat.</summary>
	public int SyncoBeat { get; init; }
	/// <summary>Gets the syncopation swing.</summary>
	public float SyncoSwing { get; init; }
	/// <summary>Gets the syncopation style.</summary>
	public SetRowXsSyncoStyle SyncoStyle { get; init; }
	/// <summary>Gets the syncopation volume.</summary>
	public int SyncoVolume { get; init; }
	/// <summary>Gets the syncopation pitch.</summary>
	public int SyncoPitch { get; init; }
	/// <summary>Gets the linked source row, when linked.</summary>
	public int? LinkedSourceRow { get; init; }
	/// <summary>Gets the link action, when linked.</summary>
	public LinkRowsAction? LinkAction { get; init; }
	/// <summary>Gets the sorting layer, when reordered.</summary>
	public LayerType? Layer { get; init; }
	/// <summary>Gets the sorting order, when reordered.</summary>
	public int? SortingOrder { get; init; }
	/// <summary>Gets the spin action, when spinning.</summary>
	public SpiningAction? SpinAction { get; init; }
	/// <summary>Gets the spin angle in degrees.</summary>
	public float SpinAngle { get; init; }
	/// <summary>Gets the spin target row.</summary>
	public int SpinToRow { get; init; }
	/// <summary>Gets the oneshot wave type, when set.</summary>
	public OneshotWaveShapeType? WaveType { get; init; }
	/// <summary>Gets the oneshot wave height.</summary>
	public int WaveHeight { get; init; }
	/// <summary>Gets the oneshot wave width.</summary>
	public int WaveWidth { get; init; }
	/// <summary>Gets whether the oneshot wave wobbles.</summary>
	public bool? WaveWobble { get; init; }
	/// <summary>Gets the heart sub-transform.</summary>
	public RowTransform Heart { get; init; }
	/// <summary>Gets the character sub-transform.</summary>
	public RowTransform Character { get; init; }
	/// <summary>Gets whether the heart has been moved independently.</summary>
	public bool HeartMoved { get; init; }
	/// <summary>Gets whether the character has been moved independently.</summary>
	public bool CharacterMoved { get; init; }
	/// <summary>Gets whether an expression has been played on the row.</summary>
	public bool HasExpression { get; init; }
	/// <summary>Gets the current row expression name.</summary>
	public string Expression { get; init; }
	/// <summary>Gets how many ticks (beats) have elapsed since the expression last changed.</summary>
	public float ExpressionElapsedTicks { get; init; }
}

/// <summary>
/// A row sub-element transform. Positions, sizes and origins are percentages.
/// </summary>
public readonly struct RowTransform
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

/// <summary>The state of every row.</summary>
public readonly struct RowsSnapshot
{
	/// <summary>Gets the row snapshots.</summary>
	public IReadOnlyList<RowSnapshot> Items { get; init; }
	/// <summary>Gets the effective draw order (row indices sorted by <c>SortingOrder</c>, then index).</summary>
	public IReadOnlyList<int> SortedIndices { get; init; }
}

/// <summary>
/// Tracks the visibility, layout, content mode and background of every room.
/// </summary>
internal sealed class RoomModule : IPlayheadObject
{
	private readonly RoomState[] _rooms;
	private int[] _order = [0, 1, 2, 3];

	internal RoomModule(Playhead playhead)
	{
		Playhead = playhead;
		int count = Constants.RoomCapacity + 1;
		_rooms = new RoomState[count];
		for (int i = 0; i < count; i++)
		{
			// The game enables only room 0 by default (full window); the others start hidden
			// (zero-height quad) until ShowRooms or MoveRoom reveals them. See RDRoom setup.
			_rooms[i] = i == 0
				? new RoomState { Visible = true, Height = FloatTween.Constant(100f) }
				: new RoomState { Visible = false, Height = FloatTween.Constant(0f) };
		}
	}

	private RoomModule(RoomModule source)
	{
		Playhead = source.Playhead;
		_rooms = new RoomState[source._rooms.Length];
		for (int i = 0; i < _rooms.Length; i++)
			_rooms[i] = source._rooms[i].Clone();
		_order = (int[])source._order.Clone();
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Rooms;

	public string Id => "rooms";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Room;

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.MoveRoom,
		EventType.ShowRooms,
		EventType.SetRoomContentMode,
		EventType.SetBackgroundColor,
		EventType.MoveCamera,
		EventType.PulseCamera,
		EventType.SetRoomPerspective,
		EventType.ReorderRooms,
		EventType.FadeRoom,
		EventType.SetForeground,
		EventType.Flash,
		EventType.CustomFlash,
	];

	public void MoveNext(IBaseEvent e)
	{
		float tick = e.TickTime.Tick;
		switch (e)
		{
			case MoveRoom move:
				ApplyMove(move, tick);
				break;
			case ShowRooms show:
				ApplyShowRooms(show, tick);
				break;
			case SetRoomContentMode mode:
				if (Valid(mode.Room.Value))
					_rooms[mode.Room.Value].ContentMode = mode.Mode;
				break;
			case SetBackgroundColor background:
				foreach (byte room in background.Rooms.Rooms)
				{
					if (!Valid(room))
						continue;
					RoomState state = _rooms[room];
					state.BackgroundColor = background.Color;
					state.BackgroundType = background.BackgroundType;
					state.BackgroundImages = background.BackgroundType == BackgroundType.Image ? [.. background.Images] : [];
					state.BackgroundContentMode = background.ContentMode;
					state.BackgroundTiling = background.TilingType;
					state.BackgroundFps = background.Fps;
				}
				break;
			case MoveCamera camera:
				ApplyCamera(camera, tick);
				break;
			case PulseCamera pulse:
				foreach (byte room in pulse.Rooms.Rooms)
				{
					if (!Valid(room))
						continue;
					_rooms[room].PulseStrength = pulse.Strength;
					_rooms[room].PulseCount = pulse.Count;
					_rooms[room].PulseFrequency = pulse.Frequency;
				}
				break;
			case SetRoomPerspective perspective:
				ApplyPerspective(perspective, tick);
				break;
			case ReorderRooms reorder:
				_order = [.. reorder.Order];
				break;
			case FadeRoom fade:
				if (Valid(fade.Room.Value))
				{
					RoomState room = _rooms[fade.Room.Value];
					room.Opacity = FloatTween.Start(room.Opacity.At(tick), fade.Opacity, tick, fade.Duration, fade.Ease);
				}
				break;
			case SetForeground foreground:
				foreach (byte room in foreground.Rooms.Rooms)
				{
					if (!Valid(room))
						continue;
					RoomState state = _rooms[room];
					state.ForegroundColor = foreground.Color;
					state.ForegroundImages = [.. foreground.Images];
					state.ForegroundContentMode = foreground.ContentMode;
					state.ForegroundTiling = foreground.TilingType;
					state.ForegroundFps = foreground.Fps;
					// With no images the game disables the foreground layer.
					state.HasForeground = state.ForegroundImages.Count > 0;
				}
				break;
			case Flash flash:
				foreach (byte room in flash.Rooms.Rooms)
				{
					if (!Valid(room))
						continue;
					RoomState state = _rooms[room];
					state.FlashColor = Global.Components.Color.White;
					state.FlashStartTick = tick;
					state.FlashDuration = FlashDurationTicks(flash.Duration);
					state.FlashStartOpacity = 100f;
					state.FlashEndOpacity = 0f;
					state.FlashBackground = false;
				}
				break;
			case CustomFlash custom:
				foreach (byte room in custom.Rooms.Rooms)
				{
					if (!Valid(room))
						continue;
					RoomState state = _rooms[room];
					state.FlashColor = custom.EndColor is { } end
						? end.Color
						: custom.StartColor is { } start ? start.Color : Global.Components.Color.White;
					state.FlashStartTick = tick;
					state.FlashDuration = custom.Duration;
					state.FlashStartOpacity = custom.StartOpacity ?? 0f;
					state.FlashEndOpacity = custom.EndOpacity ?? 0f;
					state.FlashBackground = custom.Background;
				}
				break;
		}
	}

	private static float FlashDurationTicks(DurationType duration) => (float)duration;

	private void ApplyPerspective(SetRoomPerspective perspective, float tick)
	{
		int index = perspective.Room.Value;
		if (!Valid(index))
			return;
		RoomState state = _rooms[index];
		Corner corner = perspective.Corner;
		float duration = perspective.Duration;
		ApplyPoint(ref state.PerspLbX, ref state.PerspLbY, corner.LeftBottom, tick, duration, perspective.Ease);
		ApplyPoint(ref state.PerspRbX, ref state.PerspRbY, corner.RightBottom, tick, duration, perspective.Ease);
		ApplyPoint(ref state.PerspLtX, ref state.PerspLtY, corner.LeftTop, tick, duration, perspective.Ease);
		ApplyPoint(ref state.PerspRtX, ref state.PerspRtY, corner.RightTop, tick, duration, perspective.Ease);
	}

	private static void ApplyPoint(ref FloatTween x, ref FloatTween y, Point? point, float tick, float duration, EaseType ease)
	{
		if (point is not { } value)
			return;
		if (value.X is { } px)
			x = FloatTween.Start(x.At(tick), px, tick, duration, ease);
		if (value.Y is { } py)
			y = FloatTween.Start(y.At(tick), py, tick, duration, ease);
	}

	private void ApplyCamera(MoveCamera camera, float tick)
	{
		foreach (byte room in camera.Rooms.Rooms)
		{
			if (!Valid(room))
				continue;
			RoomState state = _rooms[room];
			float duration = camera.Duration;
			if (camera.Position is { } position)
			{
				if (position.X is { } x)
					state.CameraX = FloatTween.Start(state.CameraX.At(tick), x, tick, duration, camera.Ease);
				if (position.Y is { } y)
					state.CameraY = FloatTween.Start(state.CameraY.At(tick), y, tick, duration, camera.Ease);
			}
			if (camera.Zoom is { } zoom)
				state.CameraZoom = FloatTween.Start(state.CameraZoom.At(tick), zoom, tick, duration, camera.Ease);
			if (camera.Angle is { } angle)
				state.CameraAngle = FloatTween.Start(state.CameraAngle.At(tick), angle, tick, duration, camera.Ease);
		}
	}

	private void ApplyMove(MoveRoom move, float tick)
	{
		int index = move.Room.Value;
		if (!Valid(index))
			return;
		RoomState room = _rooms[index];
		float duration = move.Duration;

		// The game's MoveRoom fields are non-null with used=true defaults and its deserializer only
		// overwrites present keys, so an event always re-applies the components it does not specify:
		// position (50,50) = window center, scale (100,100) = window size, angle 0, pivot (50,50).
		// This is what makes a scale-only MoveRoom fill the window instead of just its band.
		float targetW = move.Scale?.Width ?? 100f;
		float targetH = move.Scale?.Height ?? 100f;
		room.X = FloatTween.Start(room.X.At(tick), move.Position?.X ?? 50f, tick, duration, move.Ease);
		room.Y = FloatTween.Start(room.Y.At(tick), move.Position?.Y ?? 50f, tick, duration, move.Ease);
		room.PositionXSet = true;
		room.PositionYSet = true;

		room.Width = FloatTween.Start(room.Width.At(tick), targetW, tick, duration, move.Ease);
		room.HeightScale = FloatTween.Start(room.HeightScale.At(tick), targetH, tick, duration, move.Ease);
		room.ScaleXSet = true;
		room.ScaleYSet = true;

		room.Angle = FloatTween.Start(room.Angle.At(tick), move.Angle ?? 0f, tick, duration, move.Ease);
		room.AngleSet = true;

		room.OriginX = FloatTween.Start(room.OriginX.At(tick), move.Pivot?.X ?? 50f, tick, duration, move.Ease);
		room.OriginY = FloatTween.Start(room.OriginY.At(tick), move.Pivot?.Y ?? 50f, tick, duration, move.Ease);
		room.PivotSet = true;

		// A non-zero quad makes the room render; a zero-sized one hides it (RDRoom.EnableRendering...).
		room.Visible = Math.Abs(targetW) * Math.Abs(targetH) > 1E-06f;
	}

	private void ApplyShowRooms(ShowRooms show, float tick)
	{
		float[] percents = show.Height.GetHeightPercents(show.Rooms);
		// The game only repositions the rooms listed in the event; other rooms keep their state.
		for (int i = 0; i < _rooms.Length; i++)
		{
			if (!show.Rooms[(byte)i])
				continue;
			_rooms[i].Visible = true;
			if (i < percents.Length)
				_rooms[i].Height = FloatTween.Start(_rooms[i].Height.At(tick), percents[i] * 100f, tick, show.Duration, show.Ease);
			// ShowRooms calls AnimateQuad, which resets the MoveRoom overrides and sets the quad
			// back to the band (see scrVfxControl.ShowRooms / RDRoom.AnimateQuad).
			RoomState room = _rooms[i];
			room.X = FloatTween.Start(room.X.At(tick), 50f, tick, show.Duration, show.Ease);
			room.Y = FloatTween.Start(room.Y.At(tick), 50f, tick, show.Duration, show.Ease);
			room.Width = FloatTween.Start(room.Width.At(tick), 100f, tick, show.Duration, show.Ease);
			room.HeightScale = FloatTween.Start(room.HeightScale.At(tick), 100f, tick, show.Duration, show.Ease);
			room.OriginX = FloatTween.Start(room.OriginX.At(tick), 50f, tick, show.Duration, show.Ease);
			room.OriginY = FloatTween.Start(room.OriginY.At(tick), 50f, tick, show.Duration, show.Ease);
			room.Angle = FloatTween.Start(room.Angle.At(tick), 0f, tick, show.Duration, show.Ease);
			room.PositionXSet = false;
			room.PositionYSet = false;
			room.ScaleXSet = false;
			room.ScaleYSet = false;
			room.AngleSet = false;
			room.PivotSet = false;
		}
	}

	private bool Valid(int index) => index >= 0 && index < _rooms.Length;

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<RoomSnapshot> items = [];
		for (int i = 0; i < _rooms.Length; i++)
			items.Add(BuildRoom(i, tick));
		return new RoomsSnapshot
		{
			Items = items,
			Order = (int[])_order.Clone(),
			// The last slot is the top (HUD) camera, targeted by RoomIndex.RoomTop.
			TopCamera = BuildRoom(_rooms.Length - 1, tick).Camera,
		};
	}

	public IReadOnlyList<PlayheadObjectSnapshot> Describe()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<PlayheadObjectSnapshot> objects = [];
		for (int i = 0; i < _rooms.Length; i++)
		{
			RoomSnapshot room = BuildRoom(i, tick);
			objects.Add(new PlayheadObjectSnapshot { Id = "room" + i, Kind = PlayheadObjectKind.Room, State = room });
			objects.Add(new PlayheadObjectSnapshot { Id = "room" + i + ".camera", Kind = PlayheadObjectKind.Camera, State = room.Camera });
		}
		return objects;
	}

	private RoomSnapshot BuildRoom(int i, float tick)
	{
		RoomState room = _rooms[i];
		PerspectiveSnapshot perspective = new()
		{
			LeftBottomX = room.PerspLbX.At(tick),
			LeftBottomY = room.PerspLbY.At(tick),
			RightBottomX = room.PerspRbX.At(tick),
			RightBottomY = room.PerspRbY.At(tick),
			LeftTopX = room.PerspLtX.At(tick),
			LeftTopY = room.PerspLtY.At(tick),
			RightTopX = room.PerspRtX.At(tick),
			RightTopY = room.PerspRtY.At(tick),
		};
		float flashOpacity = 0f;
		if (room.FlashStartTick >= 0f)
		{
			float progress = room.FlashDuration > 0f ? (tick - room.FlashStartTick) / room.FlashDuration : 1f;
			progress = progress < 0f ? 0f : progress > 1f ? 1f : progress;
			flashOpacity = room.FlashStartOpacity + ((room.FlashEndOpacity - room.FlashStartOpacity) * progress);
		}
		return new RoomSnapshot
		{
			Index = i,
			Visible = room.Visible,
			Opacity = room.Opacity.At(tick),
			HasForeground = room.HasForeground,
			ForegroundColor = room.ForegroundColor,
			ForegroundImages = room.ForegroundImages,
			ForegroundContentMode = room.ForegroundContentMode,
			FlashColor = room.FlashColor,
			FlashOpacity = flashOpacity,
			FlashBackground = room.FlashBackground,
			Height = room.Height.At(tick),
			X = room.X.At(tick),
			Y = room.Y.At(tick),
			Width = room.Width.At(tick),
			HeightScale = room.HeightScale.At(tick),
			OriginX = room.OriginX.At(tick),
			OriginY = room.OriginY.At(tick),
			Angle = room.Angle.At(tick),
			PositionXSet = room.PositionXSet,
			PositionYSet = room.PositionYSet,
			ScaleXSet = room.ScaleXSet,
			ScaleYSet = room.ScaleYSet,
			AngleSet = room.AngleSet,
			PivotSet = room.PivotSet,
			ContentMode = room.ContentMode,
			BackgroundColor = room.BackgroundColor,
			BackgroundType = room.BackgroundType,
			BackgroundImages = room.BackgroundImages,
			BackgroundContentMode = room.BackgroundContentMode,
			Camera = new CameraSnapshot
			{
				X = room.CameraX.At(tick),
				Y = room.CameraY.At(tick),
				Zoom = room.CameraZoom.At(tick),
				Angle = room.CameraAngle.At(tick),
				PulseStrength = room.PulseStrength,
				PulseCount = room.PulseCount,
				PulseFrequency = room.PulseFrequency,
			},
			Perspective = perspective,
			EffectivePerspective = CompositePerspective(room, tick, perspective),
		};
	}

	private static PerspectiveSnapshot CompositePerspective(RoomState room, float tick, PerspectiveSnapshot perspective)
	{
		float px = room.X.At(tick);
		float py = room.Y.At(tick);
		float sx = room.Width.At(tick) / 100f;
		float sy = room.HeightScale.At(tick) / 100f;
		return new PerspectiveSnapshot
		{
			LeftBottomX = px + ((perspective.LeftBottomX - 50f) * sx),
			LeftBottomY = py + ((perspective.LeftBottomY - 50f) * sy),
			RightBottomX = px + ((perspective.RightBottomX - 50f) * sx),
			RightBottomY = py + ((perspective.RightBottomY - 50f) * sy),
			LeftTopX = px + ((perspective.LeftTopX - 50f) * sx),
			LeftTopY = py + ((perspective.LeftTopY - 50f) * sy),
			RightTopX = px + ((perspective.RightTopX - 50f) * sx),
			RightTopY = py + ((perspective.RightTopY - 50f) * sy),
		};
	}

	public IPlayheadObject Clone(Playhead playhead) => new RoomModule(this) { Playhead = playhead };

	private sealed class RoomState
	{
		public bool Visible;
		public FloatTween Height = FloatTween.Constant(0f);
		public FloatTween X = FloatTween.Constant(50f);
		public FloatTween Y = FloatTween.Constant(50f);
		public FloatTween Width = FloatTween.Constant(100f);
		public FloatTween HeightScale = FloatTween.Constant(100f);
		public FloatTween OriginX = FloatTween.Constant(50f);
		public FloatTween OriginY = FloatTween.Constant(50f);
		public FloatTween Angle = FloatTween.Constant(0f);
		public bool PositionXSet;
		public bool PositionYSet;
		public bool ScaleXSet;
		public bool ScaleYSet;
		public bool AngleSet;
		public bool PivotSet;
		public ContentMode ContentMode;
		public PaletteColorWithAlpha BackgroundColor;
		public BackgroundType BackgroundType;
		public List<FileReference> BackgroundImages = [];
		public ContentMode BackgroundContentMode;
		public TilingType BackgroundTiling;
		public float BackgroundFps = 30f;
		public FloatTween CameraX = FloatTween.Constant(50f);
		public FloatTween CameraY = FloatTween.Constant(50f);
		public FloatTween CameraZoom = FloatTween.Constant(100f);
		public FloatTween CameraAngle = FloatTween.Constant(0f);
		public StrengthType PulseStrength;
		public int PulseCount;
		public float PulseFrequency;
		public FloatTween PerspLbX = FloatTween.Constant(0f);
		public FloatTween PerspLbY = FloatTween.Constant(0f);
		public FloatTween PerspRbX = FloatTween.Constant(100f);
		public FloatTween PerspRbY = FloatTween.Constant(0f);
		public FloatTween PerspLtX = FloatTween.Constant(0f);
		public FloatTween PerspLtY = FloatTween.Constant(100f);
		public FloatTween PerspRtX = FloatTween.Constant(100f);
		public FloatTween PerspRtY = FloatTween.Constant(100f);
		public FloatTween Opacity = FloatTween.Constant(100f);
		public PaletteColorWithAlpha ForegroundColor;
		public bool HasForeground;
		public List<FileReference> ForegroundImages = [];
		public ContentMode ForegroundContentMode;
		public TilingType ForegroundTiling;
		public float ForegroundFps = 30f;
		public PaletteColorWithAlpha FlashColor;
		public float FlashStartTick = -1f;
		public float FlashDuration;
		public float FlashStartOpacity;
		public float FlashEndOpacity;
		public bool FlashBackground;

		public RoomState Clone() => (RoomState)MemberwiseClone();
	}
}

/// <summary>
/// Tracks the visibility, layout and tint of every row.
/// </summary>
internal sealed class RowModule : IPlayheadObject
{
	private readonly RowState[] _rows;

	internal RowModule(Playhead playhead)
	{
		Playhead = playhead;
		Chart chart = playhead.CurrentChart;
		int count = chart.Rows.Count;
		_rows = new RowState[count];
		for (int i = 0; i < count; i++)
		{
			Row row = chart.Rows[i];
			_rows[i] = new RowState { Room = row.Room, Visible = !row.HideAtStart };
		}
	}

	private RowModule(RowModule source)
	{
		Playhead = source.Playhead;
		_rows = new RowState[source._rows.Length];
		for (int i = 0; i < _rows.Length; i++)
			_rows[i] = source._rows[i].Clone();
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Rows;

	public string Id => "rows";

	public PlayheadObjectKind Kind => PlayheadObjectKind.Row;

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.MoveRow,
		EventType.HideRow,
		EventType.TintRows,
		EventType.SetRowXs,
		EventType.LinkRows,
		EventType.ReorderRow,
		EventType.SpinningRows,
		EventType.SetOneshotWave,
		EventType.PlayExpression,
	];

	public void MoveNext(IBaseEvent e)
	{
		float tick = e.TickTime.Tick;
		switch (e)
		{
			case MoveRow move:
				if (Valid(move.Row))
					ApplyMove(_rows[move.Row], move, tick);
				break;
			case HideRow hide:
				if (Valid(hide.Row))
					_rows[hide.Row].Visible = hide.Show == ShowTargetType.Visible;
				break;
			case TintRows tint:
				ApplyTint(tint, tick);
				break;
			case SetRowXs setXs:
				if (Valid(setXs.Row))
				{
					RowState state = _rows[setXs.Row];
					state.Pattern = setXs.Pattern.ToString();
					state.SyncoBeat = setXs.SyncoBeat;
					state.SyncoSwing = setXs.SyncoSwing;
					state.SyncoStyle = setXs.SyncoStyle;
					state.SyncoVolume = setXs.SyncoVolume;
					state.SyncoPitch = setXs.SyncoPitch;
				}
				break;
			case LinkRows link:
				if (Valid(link.Row))
				{
					RowState state = _rows[link.Row];
					state.LinkedSourceRow = link.SourceRow;
					state.LinkAction = link.Action;
				}
				break;
			case ReorderRow reorder:
				if (Valid(reorder.Row))
				{
					RowState state = _rows[reorder.Row];
					if (reorder.NewRoom is { } newRoom)
						state.Room = new SingleRoom(newRoom);
					state.Layer = reorder.Layer;
					state.SortingOrder = reorder.SortingOrder;
				}
				break;
			case SpinningRows spin:
				if (Valid(spin.Row))
				{
					RowState state = _rows[spin.Row];
					state.SpinAction = spin.Action;
					state.SpinAngle = spin.Angle;
					state.SpinToRow = spin.ToRow;
				}
				break;
			case SetOneshotWave wave:
				if (Valid(wave.Row))
				{
					RowState state = _rows[wave.Row];
					state.WaveType = wave.WaveType;
					state.WaveHeight = wave.Height;
					state.WaveWidth = wave.Width;
					state.WaveWobble = wave.Wobble;
				}
				break;
			case PlayExpression expression:
				if (Valid(expression.Row))
				{
					// Replace a substring of the current expression, or switch to the new one.
					RowState state = _rows[expression.Row];
					state.Expression = expression.Replace && state.HasExpression
						? state.Expression.Replace(expression.Target, expression.Expression)
						: expression.Expression;
					state.ExpressionStartTick = tick;
					state.HasExpression = true;
				}
				break;
		}
	}

	private void ApplyMove(RowState row, MoveRow move, float tick)
	{
		RowTransformTweens target = move.Target switch
		{
			MoveRowTarget.Heart => row.Heart,
			MoveRowTarget.Character => row.Character,
			_ => row.Whole,
		};
		if (move.Target == MoveRowTarget.Heart)
			row.HeartMoved = true;
		else if (move.Target == MoveRowTarget.Character)
			row.CharacterMoved = true;

		float duration = move.Duration;
		if (move.Position is { } position)
		{
			if (position.X is { } x)
				target.X = FloatTween.Start(target.X.At(tick), Playhead.EvaluateExpression(x), tick, duration, move.Ease);
			if (position.Y is { } y)
				target.Y = FloatTween.Start(target.Y.At(tick), Playhead.EvaluateExpression(y), tick, duration, move.Ease);
		}
		if (move.Scale is { } scale)
		{
			if (scale.Width is { } width)
				target.Width = FloatTween.Start(target.Width.At(tick), Playhead.EvaluateExpression(width), tick, duration, move.Ease);
			if (scale.Height is { } height)
				target.Height = FloatTween.Start(target.Height.At(tick), Playhead.EvaluateExpression(height), tick, duration, move.Ease);
		}
		if (move.Angle is { } angle)
			target.Angle = FloatTween.Start(target.Angle.At(tick), Playhead.EvaluateExpression(angle), tick, duration, move.Ease);
		if (move.Pivot is { } pivot)
		{
			float value = pivot * 100f;
			target.OriginX = FloatTween.Start(target.OriginX.At(tick), value, tick, duration, move.Ease);
			target.OriginY = FloatTween.Start(target.OriginY.At(tick), value, tick, duration, move.Ease);
		}
	}

	private void ApplyTint(TintRows tint, float tick)
	{
		if (tint.Row == -1)
		{
			foreach (RowState row in _rows)
				ApplyTint(row, tint, tick);
			return;
		}
		if (Valid(tint.Row))
			ApplyTint(_rows[tint.Row], tint, tick);
	}

	private static void ApplyTint(RowState row, TintRows tint, float tick)
	{
		row.TintColor = tint.TintColor;
		if (tint.Opacity is { } opacity)
			row.Opacity = FloatTween.Start(row.Opacity.At(tick), opacity, tick, tint.Duration, tint.Ease);
	}

	private bool Valid(int index) => index >= 0 && index < _rows.Length;

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<RowSnapshot> items = [];
		for (int i = 0; i < _rows.Length; i++)
			items.Add(BuildRow(i, tick));
		List<int> sorted = [];
		for (int i = 0; i < _rows.Length; i++)
			sorted.Add(i);
		sorted.Sort((a, b) =>
		{
			int orderA = items[a].SortingOrder ?? int.MaxValue;
			int orderB = items[b].SortingOrder ?? int.MaxValue;
			int compare = orderA.CompareTo(orderB);
			return compare != 0 ? compare : a.CompareTo(b);
		});
		return new RowsSnapshot { Items = items, SortedIndices = sorted };
	}

	public IReadOnlyList<PlayheadObjectSnapshot> Describe()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		List<PlayheadObjectSnapshot> objects = [];
		for (int i = 0; i < _rows.Length; i++)
			objects.Add(new PlayheadObjectSnapshot { Id = "row" + i, Kind = PlayheadObjectKind.Row, State = BuildRow(i, tick) });
		return objects;
	}

	private RowSnapshot BuildRow(int i, float tick)
	{
		RowState row = _rows[i];
		return new RowSnapshot
		{
			Index = i,
			Room = row.Room,
			Visible = row.Visible,
			X = row.Whole.X.At(tick),
			Y = row.Whole.Y.At(tick),
			Width = row.Whole.Width.At(tick),
			Height = row.Whole.Height.At(tick),
			OriginX = row.Whole.OriginX.At(tick),
			OriginY = row.Whole.OriginY.At(tick),
			Angle = row.Whole.Angle.At(tick),
			TintColor = row.TintColor,
			Opacity = (int)Math.Round(row.Opacity.At(tick)),
			Pattern = row.Pattern,
			SyncoBeat = row.SyncoBeat,
			SyncoSwing = row.SyncoSwing,
			SyncoStyle = row.SyncoStyle,
			SyncoVolume = row.SyncoVolume,
			SyncoPitch = row.SyncoPitch,
			LinkedSourceRow = row.LinkedSourceRow,
			LinkAction = row.LinkAction,
			Layer = row.Layer,
			SortingOrder = row.SortingOrder,
			SpinAction = row.SpinAction,
			SpinAngle = row.SpinAngle,
			SpinToRow = row.SpinToRow,
			WaveType = row.WaveType,
			WaveHeight = row.WaveHeight,
			WaveWidth = row.WaveWidth,
			WaveWobble = row.WaveWobble,
			HeartMoved = row.HeartMoved,
			CharacterMoved = row.CharacterMoved,
			Heart = BuildTransform(row.Heart, tick),
			Character = BuildTransform(row.Character, tick),
			HasExpression = row.HasExpression,
			Expression = row.Expression,
			ExpressionElapsedTicks = row.HasExpression ? Math.Max(0f, tick - row.ExpressionStartTick) : 0f,
		};
	}

	private static RowTransform BuildTransform(RowTransformTweens tweens, float tick) => new()
	{
		X = tweens.X.At(tick),
		Y = tweens.Y.At(tick),
		Width = tweens.Width.At(tick),
		Height = tweens.Height.At(tick),
		OriginX = tweens.OriginX.At(tick),
		OriginY = tweens.OriginY.At(tick),
		Angle = tweens.Angle.At(tick),
	};

	public IPlayheadObject Clone(Playhead playhead) => new RowModule(this) { Playhead = playhead };

	private sealed class RowTransformTweens
	{
		public FloatTween X = FloatTween.Constant(50f);
		public FloatTween Y = FloatTween.Constant(50f);
		public FloatTween Width = FloatTween.Constant(100f);
		public FloatTween Height = FloatTween.Constant(100f);
		public FloatTween OriginX = FloatTween.Constant(50f);
		public FloatTween OriginY = FloatTween.Constant(50f);
		public FloatTween Angle = FloatTween.Constant(0f);

		public RowTransformTweens Clone() => (RowTransformTweens)MemberwiseClone();
	}

	private sealed class RowState
	{
		public SingleRoom Room;
		public bool Visible;
		public RowTransformTweens Whole = new();
		public RowTransformTweens Heart = new();
		public RowTransformTweens Character = new();
		public bool HeartMoved;
		public bool CharacterMoved;
		public PaletteColorWithAlpha? TintColor;
		public FloatTween Opacity = FloatTween.Constant(100f);
		public string? Pattern;
		public int SyncoBeat;
		public float SyncoSwing;
		public SetRowXsSyncoStyle SyncoStyle;
		public int SyncoVolume;
		public int SyncoPitch;
		public int? LinkedSourceRow;
		public LinkRowsAction? LinkAction;
		public LayerType? Layer;
		public int? SortingOrder;
		public SpiningAction? SpinAction;
		public float SpinAngle;
		public int SpinToRow;
		public OneshotWaveShapeType? WaveType;
		public int WaveHeight;
		public int WaveWidth;
		public bool? WaveWobble;
		public string Expression = "";
		public float ExpressionStartTick;
		public bool HasExpression;

		public RowState Clone()
		{
			RowState clone = (RowState)MemberwiseClone();
			clone.Whole = Whole.Clone();
			clone.Heart = Heart.Clone();
			clone.Character = Character.Clone();
			return clone;
		}
	}
}






