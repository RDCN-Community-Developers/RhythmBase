namespace RhythmBase.Global.Components.Easing;

/// <summary>
/// Contains information about an easing operation, including its type and duration.
/// </summary>
public readonly struct EaseInfo
{
	/// <summary>
	/// Gets the type of easing function to use.
	/// </summary>
	public EaseType Type { get; }

	/// <summary>
	/// Gets the duration of the easing operation.
	/// </summary>
	public float Duration { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="EaseInfo"/> struct.
	/// </summary>
	/// <param name="type">The type of easing function.</param>
	/// <param name="duration">The duration of the easing.</param>
	public EaseInfo(EaseType type, float duration)
	{
		Type = type;
		Duration = duration;
	}
}
/// <summary>
/// Represents a node with a time value.
/// </summary>
public interface INode
{
	/// <summary>
	/// Gets the time value of the node.
	/// </summary>
	float Time { get; }
}
/// <summary>
/// Represents a curve node with a value at a specific time.
/// </summary>
/// <typeparam name="T">The type of value stored in the node.</typeparam>
public readonly struct CurveNode<T> : INode
{
	/// <summary>
	/// Gets the time value of the node.
	/// </summary>
	public float Time { get; }

	/// <summary>
	/// Gets the value at the specified time.
	/// </summary>
	public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CurveNode{T}"/> struct.
    /// </summary>
    /// <param name="time">The time value.</param>
    /// <param name="value">The value at the specified time.</param>
    public CurveNode(float time, T value)
	{
		Time = time;
		Value = value;
	}
}
/// <summary>
/// Represents a tween curve node with a target value and easing information at a specific time.
/// </summary>
/// <typeparam name="T">The type of the target value.</typeparam>
public readonly struct TweenCurveNode<T> : INode
{
	/// <summary>
	/// Gets the time value of the node.
	/// </summary>
	public float Time { get; }

	/// <summary>
	/// Gets the target value for the tween.
	/// </summary>
	public T Target { get; }

	/// <summary>
	/// Gets the easing information for the tween.
	/// </summary>
	public EaseInfo EaseInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TweenCurveNode{T}"/> struct.
    /// </summary>
    /// <param name="time">The time value.</param>
    /// <param name="target">The target value.</param>
    /// <param name="easeInfo">The easing information.</param>
    public TweenCurveNode(float time, T target, EaseInfo easeInfo)
	{
		Time = time;
		Target = target;
		EaseInfo = easeInfo;
	}
}
/// <summary>
/// Represents a curve of values over time.
/// </summary>
/// <typeparam name="T">The type of value in the curve.</typeparam>
public readonly struct Curve<T>
{
	private readonly T _default;
	private readonly CurveNode<T>[] _nodes = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Curve{T}"/> struct.
    /// </summary>
    /// <param name="values">The curve nodes.</param>
    /// <param name="defaultValue">The default value to use when no nodes are present or time is out of range.</param>
    public Curve(CurveNode<T>[] values, T defaultValue)
	{
		_nodes = [.. values.OrderBy(i => i.Time)];
		_default = defaultValue;
	}

	/// <summary>
	/// Gets the value of the curve at the specified time.
	/// </summary>
	/// <param name="time">The time to evaluate.</param>
	/// <returns>The value at the specified time.</returns>
	public readonly T this[float time]
	{
		get
		{
			if (_nodes.Length == 0)
				return _default;
			if (time < _nodes[0].Time)
				return _default;
			if (time == _nodes[0].Time)
				return _nodes[0].Value;
			if (time >= _nodes[^1].Time)
				return _nodes[^1].Value;
			int left = 0, right = _nodes.Length - 1;
			while (left < right - 1)
			{
				int mid = (left + right) / 2;
				if (_nodes[mid].Time <= time)
					left = mid;
				else
					right = mid;
			}
			CurveNode<T> leftNode = _nodes[left];
			return leftNode.Value;
		}
	}
}
/// <summary>
/// Represents a tween curve that interpolates between values over time using easing functions.
/// </summary>
/// <typeparam name="T">The type of value in the curve.</typeparam>
public readonly struct TweenCurve<T>
{
	private readonly T _default;
	private readonly TweenCurveNode<T>[] _nodes = [];
	private readonly Func<T, T, double, T> _interpolator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TweenCurve{T}"/> struct.
    /// </summary>
    /// <param name="values">The tween curve nodes.</param>
    /// <param name="defaultValue">The default value to use when no nodes are present or time is out of range.</param>
    /// <param name="interpolator">The interpolation function to use between values.</param>
    public TweenCurve(TweenCurveNode<T>[] values, T defaultValue, Func<T, T, double, T> interpolator)
	{
		_nodes = [.. values.OrderBy(i => i.Time)];
		_default = defaultValue;
		_interpolator = interpolator;
	}

	/// <summary>
	/// Gets the interpolated value of the curve at the specified time.
	/// </summary>
	/// <param name="time">The time to evaluate.</param>
	/// <returns>The interpolated value at the specified time.</returns>
	public readonly T this[float time]
	{
		get
		{
			if (_nodes.Length == 0 || time <= _nodes[0].Time)
				return _default;
			int left = 0, right = _nodes.Length - 1;
			if (time >= _nodes[^1].Time)
				left = _nodes.Length - 1;
			if (time >= _nodes[^1].Time + _nodes[^1].EaseInfo.Duration)
				return _nodes[^1].Target;
			while (left < right - 1)
			{
				int mid = (left + right) / 2;
				if (_nodes[mid].Time <= time)
					left = mid;
				else
					right = mid;
			}
			int cur = left;
			while (left > 0)
			{
				TweenCurveNode<T> prevNode = _nodes[left - 1];
				if (prevNode.Time + prevNode.EaseInfo.Duration < _nodes[left--].Time)
					break;
			}
			T origin = left == 0 ? _default : _nodes[left - 1].Target;
			while (left < cur)
			{
				origin = _interpolator(
					origin,
					_nodes[left].Target,
					_nodes[left].EaseInfo.Type.Calculate(
						(_nodes[left + 1].Time - _nodes[left].Time) / _nodes[left].EaseInfo.Duration
					));
				left++;
			}
			origin = _interpolator(
				origin,
				_nodes[left].Target,
				_nodes[left].EaseInfo.Type.Calculate(
					(time - _nodes[left].Time) / _nodes[left].EaseInfo.Duration
				));
			return origin;
		}
	}
}
/// <summary>
/// Represents a vector tween curve that interpolates between multi-dimensional values over time using easing functions.
/// </summary>
/// <typeparam name="T">The type of vector value in the curve.</typeparam>
public readonly struct VectorTweenCurve<T>
{
	private readonly float[] _default;
	private readonly TweenCurveNode<float>[][] _nodes = [];
	private readonly Func<float, float, double, float> _interpolator;
	private readonly Func<float[], T> _fromArray;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorTweenCurve{T}"/> struct.
    /// </summary>
    /// <param name="values">The array of tween curve nodes for each vector component.</param>
    /// <param name="defaultValue">The default vector value to use when no nodes are present or time is out of range.</param>
    /// <param name="interpolator">The interpolation function to use between values.</param>
    /// <param name="toArray">Function to convert the vector value to an array of floats.</param>
    /// <param name="fromArray">Function to convert an array of floats to the vector value.</param>
    public VectorTweenCurve(
		TweenCurveNode<float>[][] values,
		T defaultValue,
		Func<float, float, double, float> interpolator,
		Func<T, float[]> toArray,
		Func<float[], T> fromArray)
	{
		_nodes = [.. values.Select(i => i.OrderBy(j => j.Time).ToArray())];
		_default = toArray(defaultValue);
		_interpolator = interpolator;
		_fromArray = fromArray;
	}

	/// <summary>
	/// Gets the interpolated vector value of the curve at the specified time.
	/// </summary>
	/// <param name="time">The time to evaluate.</param>
	/// <returns>The interpolated vector value at the specified time.</returns>
	public readonly T this[float time]
	{
		get
		{
			float[] result = new float[_nodes.Length];
			for (int i = 0; i < _nodes.Length; i++)
			{
				if (_nodes[i].Length == 0 || time <= _nodes[i][0].Time)
				{
					result[i] = _default[i];
					continue;
				}
				int left = 0, right = _nodes[i].Length - 1;
				if (time >= _nodes[i][^1].Time)
					left = _nodes[i].Length - 1;
				if (time >= _nodes[i][^1].Time + _nodes[i][^1].EaseInfo.Duration)
				{
					result[i] = _nodes[i][^1].Target;
					continue;
				}
				while (left < right - 1)
				{
					int mid = (left + right) / 2;
					if (_nodes[i][mid].Time <= time)
						left = mid;
					else
						right = mid;
				}
				int cur = left;
				while (left > 0)
				{
					TweenCurveNode<float> prevNode = _nodes[i][left - 1];
					if (prevNode.Time + prevNode.EaseInfo.Duration < _nodes[i][left--].Time)
						break;
				}
				float origin = left == 0 ? _default[i] : _nodes[i][left - 1].Target;
				while (left < cur)
				{
					origin = _interpolator(
						origin,
						_nodes[i][left].Target,
						_nodes[i][left].EaseInfo.Type.Calculate(
							(_nodes[i][left + 1].Time - _nodes[i][left].Time) / _nodes[i][left].EaseInfo.Duration
						));
					left++;
				}
				origin = _interpolator(
					origin,
					_nodes[i][left].Target,
					_nodes[i][left].EaseInfo.Type.Calculate(
						(time - _nodes[i][left].Time) / _nodes[i][left].EaseInfo.Duration
					));
				result[i] = origin;
			}
			return _fromArray(result);
		}
	}
}
