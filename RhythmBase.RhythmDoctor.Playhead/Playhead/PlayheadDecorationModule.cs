using System.Text.Json;
using RhythmBase.Global.Components;
using RhythmBase.Global.Components.Easing;
using RhythmBase.Global.Components.Vector;
using RhythmBase.RhythmDoctor.Events;
using RhythmBase.RhythmDoctor.Extensions;
using RhythmBase.RhythmDoctor.Utils;

namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Tracks the visibility, transform and tint of one decoration.
/// </summary>
internal sealed class DecorationModule : IPlayheadObject
{
	private readonly string _id;
	private readonly SingleRoom _room;
	private readonly DecorationType _type;
	private readonly LayerType _layer;
	private readonly int _depth;
	private float _visibleAt;
	private bool _visible;
	private FloatTween _x = FloatTween.Constant(50f);
	private FloatTween _y = FloatTween.Constant(50f);
	private FloatTween _width = FloatTween.Constant(1f);
	private FloatTween _height = FloatTween.Constant(1f);
	private FloatTween _originX = FloatTween.Constant(50f);
	private FloatTween _originY = FloatTween.Constant(50f);
	private FloatTween _angle = FloatTween.Constant(0f);
	private PaletteColorWithAlpha? _tintColor;
	private FloatTween _opacity = FloatTween.Constant(100f);
	private bool _isTint;
	private bool _hasText;
	private string _text = "";
	private int _textSize = 8;
	private PaletteColorWithAlpha _textColor;
	private PaletteColorWithAlpha _outlineColor;
	private string _font = "";
	private int _advanceCount;
	private string _expression = "";
	private float _expressionStartTick;
	private bool _hasExpression;

	internal DecorationModule(Playhead playhead, Decoration decoration)
	{
		Playhead = playhead;
		_id = decoration.Id;
		_room = decoration.Room;
		_type = decoration.Type;
		// The game sorts sprites on the Default layer (sortingOrder = -depth); text uses its layer.
		_layer = decoration.Type == DecorationType.Text ? decoration.Layer : LayerType.Default;
		_depth = decoration.Depth;
		_visible = decoration.Visible;
		// The decoration's initial size is a ratio of the sprite's own pixel size (1 = original)
		// and is stored in the raw extra data.
		JsonElement size = decoration["size"];
		if (size.ValueKind == JsonValueKind.Array && size.GetArrayLength() >= 2)
		{
			_width = FloatTween.Constant((float)size[0].GetDouble());
			_height = FloatTween.Constant((float)size[1].GetDouble());
		}
	}

	private DecorationModule(DecorationModule source)
	{
		Playhead = source.Playhead;
		_id = source._id;
		_room = source._room;
		_type = source._type;
		_layer = source._layer;
		_depth = source._depth;
		_visibleAt = source._visibleAt;
		_visible = source._visible;
		_x = source._x;
		_y = source._y;
		_width = source._width;
		_height = source._height;
		_originX = source._originX;
		_originY = source._originY;
		_angle = source._angle;
		_tintColor = source._tintColor;
		_opacity = source._opacity;
		_isTint = source._isTint;
		_hasText = source._hasText;
		_text = source._text;
		_textSize = source._textSize;
		_textColor = source._textColor;
		_outlineColor = source._outlineColor;
		_font = source._font;
		_advanceCount = source._advanceCount;
		_expression = source._expression;
		_expressionStartTick = source._expressionStartTick;
		_hasExpression = source._hasExpression;
	}

	public Playhead Playhead { get; private set; }

	public void Rebind(Playhead playhead) => Playhead = playhead;

	public PlayheadModules Module => PlayheadModules.Decorations;

	public string Id => _id;

	public PlayheadObjectKind Kind => _type == DecorationType.Text ? PlayheadObjectKind.TextDecoration : PlayheadObjectKind.SpriteDecoration;

	public IReadOnlyList<PlayheadObjectSnapshot> Describe() => [new PlayheadObjectSnapshot { Id = Id, Kind = Kind, State = Snapshot() }];

	public ReadOnlyEnumCollection<EventType> RecordingTypes { get; } =
	[
		EventType.Move,
		EventType.SetVisible,
		EventType.Tint,
		EventType.SetText,
		EventType.TintText,
		EventType.SetFont,
		EventType.AdvanceTextDecoration,
		EventType.PlayAnimation,
	];

	public void MoveNext(IBaseEvent e)
	{
		if (e is not BaseDecorationAction action || action.Target != _id)
			return;
		float tick = e.TickTime.Tick;
		switch (action)
		{
			case Move move:
				ApplyMove(move, tick);
				break;
			case SetVisible visible:
				_visible = visible.Visible;
				_visibleAt = tick;
				break;
			case Tint tint:
				ApplyTint(tint, tick);
				break;
			case SetText text:
				ApplySetText(text, tick);
				break;
			case TintText tintText:
				if (tintText.TintColor is { } tintColor)
					_tintColor = tintColor;
				if (tintText.BorderColor is { } borderColor)
					_outlineColor = borderColor;
				break;
			case SetFont font:
				_font = font.Font.ToString() ?? "";
				break;
			case AdvanceTextDecoration:
				_advanceCount++;
				break;
			case PlayAnimation animation:
				// Switch the animation state and remember when, so the current frame can be derived.
				_expression = animation.Expression;
				_expressionStartTick = tick;
				_hasExpression = true;
				break;
		}
	}

	private void ApplySetText(SetText text, float tick)
	{
		_text = Playhead.Interpolate(text.Text);
		_textSize = text.Size;
		_textColor = text.Color;
		_outlineColor = text.OutlineColor;
		_hasText = true;
		_advanceCount = 0;
		if (text.Position is { } position)
		{
			if (position.X is { } x)
				_x = FloatTween.Start(_x.At(tick), x, tick, text.Duration, EaseType.Linear);
			if (position.Y is { } y)
				_y = FloatTween.Start(_y.At(tick), y, tick, text.Duration, EaseType.Linear);
		}
	}

	private void ApplyMove(Move move, float tick)
	{
		float duration = move.Duration;
		if (move.Position is { } position)
		{
			if (position.X is { } x)
				_x = FloatTween.Start(_x.At(tick), Playhead.EvaluateExpression(x), tick, duration, move.Ease);
			if (position.Y is { } y)
				_y = FloatTween.Start(_y.At(tick), Playhead.EvaluateExpression(y), tick, duration, move.Ease);
		}
		if (move.Scale is { } scale)
		{
			if (scale.Width is { } width)
				_width = FloatTween.Start(_width.At(tick), Playhead.EvaluateExpression(width), tick, duration, move.Ease);
			if (scale.Height is { } height)
				_height = FloatTween.Start(_height.At(tick), Playhead.EvaluateExpression(height), tick, duration, move.Ease);
		}
		if (move.Angle is { } angle)
			_angle = FloatTween.Start(_angle.At(tick), Playhead.EvaluateExpression(angle), tick, duration, move.Ease);
		if (move.Pivot is { } pivot)
		{
			if (pivot.X is { } ox)
				_originX = FloatTween.Start(_originX.At(tick), ox, tick, duration, move.Ease);
			if (pivot.Y is { } oy)
				_originY = FloatTween.Start(_originY.At(tick), oy, tick, duration, move.Ease);
		}
	}

	private void ApplyTint(Tint tint, float tick)
	{
		_tintColor = tint.TintColor;
		_isTint = tint.IsTint ?? true;
		if (tint.Opacity is { } opacity)
			_opacity = FloatTween.Start(_opacity.At(tick), opacity, tick, tint.Duration, tint.Ease);
	}

	public object Snapshot()
	{
		float tick = Playhead.CurrentTickTime.Tick;
		return new DecorationSnapshot
		{
			Id = _id,
			Room = _room,
			RoomIndex = _room.Value,
			Type = _type,
			Layer = _layer,
			Depth = _depth,
			Visible = _visible,
			TintColor = _tintColor,
			Opacity = (int)Math.Round(_opacity.At(tick)),
			IsTint = _isTint,
			HasText = _hasText,
			Text = _text,
			TextSize = _textSize,
			TextColor = _textColor,
			OutlineColor = _outlineColor,
			Font = _font,
			AdvanceCount = _advanceCount,
			HasExpression = _hasExpression,
			Expression = _expression,
			ExpressionElapsedTicks = _hasExpression ? Math.Max(0f, tick - _expressionStartTick) : 0f,
			Transform = new DecorationTransform
			{
				X = _x.At(tick),
				Y = _y.At(tick),
				Width = _width.At(tick),
				Height = _height.At(tick),
				OriginX = _originX.At(tick),
				OriginY = _originY.At(tick),
				Angle = _angle.At(tick),
			},
		};
	}

	public IPlayheadObject Clone(Playhead playhead) => new DecorationModule(this) { Playhead = playhead };
}
