using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rectangle defined by its left, top, right, and bottom edges.
	/// </summary>
	/// <param name="left">The left edge of the rectangle.</param>
	/// <param name="top">The top edge of the rectangle.</param>
	/// <param name="right">The right edge of the rectangle.</param>
	/// <param name="bottom">The bottom edge of the rectangle.</param>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct RectN(float left, float top, float right, float bottom)
	{
		/// <summary>
		/// Gets or sets the left edge of the rectangle.
		/// </summary>
		public float Left { get; set; } = left;
		/// <summary>
		/// Gets or sets the right edge of the rectangle.
		/// </summary>
		public float Right { get; set; } = right;
		/// <summary>
		/// Gets or sets the top edge of the rectangle.
		/// </summary>
		public float Top { get; set; } = top;
		/// <summary>
		/// Gets or sets the bottom edge of the rectangle.
		/// </summary>
		public float Bottom { get; set; } = bottom;
		/// <summary>
		/// Gets the point at the left-bottom corner of the rectangle.
		/// </summary>
		public readonly PointN LeftBottom => new(Left, Bottom);
		/// <summary>
		/// Gets the point at the right-bottom corner of the rectangle.
		/// </summary>
		public readonly PointN RightBottom => new(Right, Bottom);
		/// <summary>
		/// Gets the point at the left-top corner of the rectangle.
		/// </summary>
		public readonly PointN LeftTop => new(Left, Top);
		/// <summary>
		/// Gets the point at the right-top corner of the rectangle.
		/// </summary>
		public readonly PointN RightTop => new(Right, Top);
		/// <summary>
		/// Gets the width of the rectangle.
		/// </summary>
		public readonly float Width => Right - Left;
		/// <summary>
		/// Gets the height of the rectangle.
		/// </summary>
		public readonly float Height => Top - Bottom;
		/// <summary>
		/// Initializes a new instance of the <see cref="RectN"/> struct with the specified location and size.
		/// </summary>
		/// <param name="location">The location of the rectangle.</param>
		/// <param name="size">The size of the rectangle.</param>
		public RectN(PointN location, SizeN size) : this(location.X, location.Y + size.Height, location.X + size.Width, location.Y) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="RectN"/> struct with the specified size.
		/// </summary>
		/// <param name="size">The size of the rectangle.</param>
		public RectN(SizeN size) : this(0f, size.Height, size.Width, 0f) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="RectN"/> struct with the specified width and height.
		/// </summary>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectN(float width, float height) : this(0f, height, width, 0f) { }
		/// <summary>
		/// Gets the location of the rectangle as an <see cref="PointNI"/>.
		/// </summary>
		public readonly PointN Location => new(Left, Bottom);
		/// <summary>
		/// Gets the size of the rectangle as an <see cref="SizeNI"/>.
		/// </summary>
		public readonly SizeN Size => new(Width, Height);

		/// <summary>
		/// Inflates the specified rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RectN Inflate(RectN rect, SizeNI size)
		{
			RectN result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
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
		public static RectN Inflate(RectN rect, float x, float y)
		{
			RectN result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the point.</param>
		/// <param name="y">The y-coordinate of the point.</param>
		/// <returns><c>true</c> if the rectangle contains the point; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(float x, float y) => Left < x && x < Right && Bottom < y && y < Top;
		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="p">The point to check.</param>
		/// <returns><c>true</c> if the rectangle contains the point; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(PointN p) => Left < p.X && p.X < Right && Bottom < p.Y && p.Y < Top;
		/// <summary>
		/// Determines whether the rectangle contains the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to check.</param>
		/// <returns><c>true</c> if the rectangle contains the specified rectangle; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(RectN rect) => Left < rect.Left && rect.Right < Right && Bottom < rect.Bottom && rect.Top < Top;
		/// <summary>
		/// Returns the union of two rectangles.
		/// </summary>
		/// <param name="rect1">The first rectangle.</param>
		/// <param name="rect2">The second rectangle.</param>
		/// <returns>The union of the two rectangles.</returns>
		public static RectN Union(RectN rect1, RectN rect2) => new(Math.Min(rect1.Left, rect2.Left), Math.Max(rect1.Top, rect2.Top), Math.Max(rect1.Right, rect2.Right), Math.Min(rect1.Bottom, rect2.Bottom));
		/// <summary>
		/// Returns the intersection of two rectangles.
		/// </summary>
		/// <param name="rect1">The first rectangle.</param>
		/// <param name="rect2">The second rectangle.</param>
		/// <returns>The intersection of the two rectangles, or the default rectangle if they do not intersect.</returns>
		public static RectN Intersect(RectN rect1, RectN rect2) => rect1.IntersectsWithInclusive(rect2) ? new RectN(
		Math.Max(rect1.Left, rect2.Left),
		Math.Max(rect1.Top, rect2.Top),
		Math.Min(rect1.Right, rect2.Right),
		Math.Min(rect1.Bottom, rect2.Bottom)) : default;
		/// <summary>
		/// Truncates the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to truncate.</param>
		/// <returns>The truncated rectangle.</returns>
		public static RectN Truncate(RectN rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
		/// <summary>
		/// Offsets the rectangle by the specified amount.
		/// </summary>
		/// <param name="x">The amount to offset by along the x-axis.</param>
		/// <param name="y">The amount to offset by along the y-axis.</param>
		public void Offset(float x, float y)
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
		public void Offset(PointN p) => Offset(p.X, p.Y);
		/// <summary>
		/// Inflates the rectangle by the specified size.
		/// </summary>
		/// <param name="size">The size to inflate by.</param>
		public void Inflate(SizeN size)
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
		public void Inflate(float width, float height)
		{
			Left -= width;
			Top += height;
			Right += width;
			Bottom -= height;
		}
		/// <summary>
		/// Returns the union of this rectangle with the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to union with.</param>
		/// <returns>The union of the two rectangles.</returns>
		public readonly RectN Union(RectN rect) => Union(this, rect);
		/// <summary>
		/// Determines whether this rectangle intersects with the specified rectangle, including the edges.
		/// </summary>
		/// <param name="rect">The rectangle to check for intersection.</param>
		/// <returns><c>true</c> if the rectangles intersect; otherwise, <c>false</c>.</returns>
		public readonly bool IntersectsWithInclusive(RectN rect) => Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom && Bottom >= rect.Top;
		/// <inheritdoc/>
		public static bool operator ==(RectN rect1, RectN rect2) => rect1.Equals(rect2);
		/// <inheritdoc/>
		public static bool operator !=(RectN rect1, RectN rect2) => !rect1.Equals(rect2);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is RectN e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + Left.GetHashCode();
			hash = hash * 31 + Top.GetHashCode();
			hash = hash * 31 + Right.GetHashCode();
			hash = hash * 31 + Bottom.GetHashCode();
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"{{Location=[{Left},{Bottom}],Size=[{Width},{Height}]}}";
		/// <inheritdoc/>
		public readonly bool Equals(RectN other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
		/// <summary>
		/// Converts an <see cref="RectN"/> to an <see cref="Rect"/>.
		/// </summary>
		/// <param name="rect">The <see cref="RectN"/> to convert.</param>
		/// <returns>An <see cref="Rect"/> that represents the same rectangle.</returns>
		public static implicit operator Rect(RectN rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
		private readonly string GetDebuggerDisplay() => ToString();
	}
