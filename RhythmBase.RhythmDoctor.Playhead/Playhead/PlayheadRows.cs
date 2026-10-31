using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Components;


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






