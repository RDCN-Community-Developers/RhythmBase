using RhythmBase.RhythmDoctor.Components;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rectangle defined by four expressions: left, top, right, and bottom.
	/// </summary>
	/// <param name="left">The left expression of the rectangle.</param>
	/// <param name="top">The top expression of the rectangle.</param>
	/// <param name="right">The right expression of the rectangle.</param>
	/// <param name="bottom">The bottom expression of the rectangle.</param>
	public struct RectE(Expression? left, Expression? top, Expression? right, Expression? bottom) : IEquatable<RectE>
	{
		/// <summary>
		/// Gets or sets the left expression of the rectangle.
		/// </summary>
		public Expression? Left { get; set; } = left;
		/// <summary>
		/// Gets or sets the right expression of the rectangle.
		/// </summary>
		public Expression? Right { get; set; } = right;
		/// <summary>
		/// Gets or sets the top expression of the rectangle.
		/// </summary>
		public Expression? Top { get; set; } = top;
		/// <summary>
		/// Gets or sets the bottom expression of the rectangle.
		/// </summary>
		public Expression? Bottom { get; set; } = bottom;
		/// <summary>
		/// Gets the left-bottom point of the rectangle.
		/// </summary>
		public readonly PointE LeftBottom => new(Left, Bottom);
		/// <summary>
		/// Gets the right-bottom point of the rectangle.
		/// </summary>
		public readonly PointE RightBottom => new(Right, Bottom);
		/// <summary>
		/// Gets the left-top point of the rectangle.
		/// </summary>
		public readonly PointE LeftTop => new(Left, Top);
		/// <summary>
		/// Gets the right-top point of the rectangle.
		/// </summary>
		public readonly PointE RightTop => new(Right, Top);
		/// <summary>
		/// Gets the width of the rectangle.
		/// </summary>
		public readonly Expression? Width => Right - Left;
		/// <summary>
		/// Gets the height of the rectangle.
		/// </summary>
		public readonly Expression? Height => Top - Bottom;
		/// <summary>
		/// Initializes a new instance of the <see cref="RectE"/> struct with the specified location and size.
		/// </summary>
		/// <param name="location">The location of the rectangle.</param>
		/// <param name="size">The size of the rectangle.</param>
		public RectE(PointE? location, SizeE? size) : this(location?.X, location?.Y + size?.Height, location?.X + size?.Width, location?.Y) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="RectE"/> struct with the specified size.
		/// </summary>
		/// <param name="size">The size of the rectangle.</param>
		public RectE(SizeE size) : this(new Expression?(0f), size.Height, size.Width, new Expression?(0f)) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="RectE"/> struct with the specified width and height.
		/// </summary>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectE(Expression? width, Expression? height) : this(new Expression?(0f), height, width, new Expression?(0f)) { }
		/// <summary>
		/// Gets the location of the rectangle.
		/// </summary>
		public readonly PointE Location => new(Left, Bottom);
		/// <summary>
		/// Gets the size of the rectangle.
		/// </summary>
		public readonly SizeE Size => new(Width, Height);
		/// <summary>
		/// Inflates the specified rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RectE Inflate(RectE rect, SizeE size)
		{
			RectE result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
			result.Inflate(size);
			return result;
		}
		/// <summary>
		/// Inflates the specified rectangle by the specified width and height.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="x">The width to inflate by.</param>
		/// <param name="y">The height to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RectE Inflate(RectE rect, Expression? x, Expression? y)
		{
			RectE result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Truncates the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to truncate.</param>
		/// <returns>The truncated rectangle.</returns>
		public static RectE Truncate(RectE rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
		/// <summary>
		/// Offsets the rectangle by the specified width and height.
		/// </summary>
		/// <param name="x">The width to offset by.</param>
		/// <param name="y">The height to offset by.</param>
		public void Offset(Expression? x, Expression? y)
		{
			Left += x;
			Top += y;
			Right += x;
			Bottom += y;
		}
		/// <summary>
		/// Offsets the rectangle by the specified point.
		/// </summary>
		/// <param name="p">The point to offset by.</param>
		public void Offset(PointE p) => Offset(p.X, p.Y);
		/// <summary>
		/// Inflates the rectangle by the specified size.
		/// </summary>
		/// <param name="size">The size to inflate by.</param>
		public void Inflate(SizeE size)
		{
			Left -= size.Width;
			Top += size.Height;
			Right += size.Width;
			Bottom -= size.Height;
		}
		/// <summary>
		/// Inflates the rectangle by the specified width and height.
		/// </summary>
		/// <param name="width">The width to inflate by.</param>
		/// <param name="height">The height to inflate by.</param>
		public void Inflate(Expression? width, Expression? height)
		{
			Left -= width;
			Top += height;
			Right += width;
			Bottom -= height;
		}
		/// <inheritdoc/>
		public static bool operator ==(RectE rect1, RectE rect2) => rect1.Equals(rect2);
		/// <inheritdoc/>
		public static bool operator !=(RectE rect1, RectE rect2) => !rect1.Equals(rect2);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is RectE e && Equals(e);
    /// <summary>
    /// Converts an <see cref="RectN"/> to an <see cref="RectE"/>.
    /// </summary>
    /// <param name="rect">The <see cref="RectN"/> to convert.</param>
    /// <returns>An <see cref="RectE"/> that represents the same rectangle.</returns>
    public static implicit operator RectE(RectN rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
    /// <summary>
    /// Implicitly converts an <see cref="RectNI"/> to an <see cref="RectE"/>.
    /// </summary>
    /// <param name="rect">The <see cref="RectNI"/> to convert.</param>
    /// <returns>An <see cref="RectE"/> that represents the same rectangle.</returns>
    public static implicit operator RectE(RectNI rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
    /// <summary>
    /// Implicitly converts an <see cref="RectI"/> to an <see cref="RectE"/>.
    /// </summary>
    /// <param name="rect">The <see cref="RectI"/> instance to convert.</param>
    /// <returns>The converted <see cref="RectE"/> instance.</returns>
    public static implicit operator RectE(RectI rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
    /// <summary>
    /// Implicitly converts an <see cref="Rect"/> to an <see cref="RectE"/>.
    /// </summary>
    /// <param name="rect">The <see cref="Rect"/> to convert.</param>
    /// <returns>The converted <see cref="RectE"/>.</returns>
    public static implicit operator RectE(Rect rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
    /// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + (Left?.GetHashCode() ?? 0);
			hash = hash * 31 + (Top?.GetHashCode() ?? 0);
			hash = hash * 31 + (Right?.GetHashCode() ?? 0);
			hash = hash * 31 + (Bottom?.GetHashCode() ?? 0);
			return hash;
		}
#else
    public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"{{Location=[{Left},{Bottom}],Size=[{Width},{Height}]}}";
		/// <inheritdoc/>
		public readonly bool Equals(RectE other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
	}
