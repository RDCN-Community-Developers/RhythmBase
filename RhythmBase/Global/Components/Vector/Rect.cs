using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rectangle defined by its left, top, right, and bottom edges.
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct Rect(float? left, float? top, float? right, float? bottom) : IEquatable<Rect>
	{
		/// <summary>
		/// Gets or sets the left edge of the rectangle.
		/// </summary>
		public float? Left { get; set; } = left;
		/// <summary>
		/// Gets or sets the right edge of the rectangle.
		/// </summary>
		public float? Right { get; set; } = right;
		/// <summary>
		/// Gets or sets the top edge of the rectangle.
		/// </summary>
		public float? Top { get; set; } = top;
		/// <summary>
		/// Gets or sets the bottom edge of the rectangle.
		/// </summary>
		public float? Bottom { get; set; } = bottom;
		/// <summary>
		/// Gets the point at the left-bottom corner of the rectangle.
		/// </summary>
		public readonly Point LeftBottom => new(Left, Bottom);

		/// <summary>
		/// Gets the point at the right-bottom corner of the rectangle.
		/// </summary>
		public readonly Point RightBottom => new(Right, Bottom);

		/// <summary>
		/// Gets the point at the left-top corner of the rectangle.
		/// </summary>
		public readonly Point LeftTop => new(Left, Top);

		/// <summary>
		/// Gets the point at the right-top corner of the rectangle.
		/// </summary>
		public readonly Point RightTop => new(Right, Top);

		/// <summary>
		/// Gets the width of the rectangle.
		/// </summary>
		public readonly float? Width => Right - Left;
		/// <summary>
		/// Gets the height of the rectangle.
		/// </summary>
		public readonly float? Height => Top - Bottom;
		/// <summary>
		/// Initializes a new instance of the <see cref="Rect"/> struct with the specified location and size.
		/// </summary>
		/// <param name="location">The location of the rectangle.</param>
		/// <param name="size">The size of the rectangle.</param>
		public Rect(Point? location, Size? size) : this(location?.X, location?.Y + size?.Height, location?.X + size?.Width, location?.Y) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Rect"/> struct with the specified size.
		/// </summary>
		/// <param name="size">The size of the rectangle.</param>
		public Rect(Size size) : this(new float?(0f), size.Height, size.Width, new float?(0f)) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="Rect"/> struct with the specified width and height.
		/// </summary>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public Rect(float? width, float? height) : this(new float?(0f), height, width, new float?(0f)) { }
		/// <summary>
		/// Gets the location of the rectangle.
		/// </summary>
		public readonly Point Location => new(Left, Bottom);
		/// <summary>
		/// Gets the size of the rectangle.
		/// </summary>
		public readonly Size Size => new(Width, Height);
		/// <summary>
		/// Inflates the specified rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static Rect Inflate(Rect rect, Size size)
		{
			Rect result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
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
		public static Rect Inflate(Rect rect, float? x, float? y)
		{
			Rect result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Returns the union of two rectangles.
		/// </summary>
		/// <param name="rect1">The first rectangle.</param>
		/// <param name="rect2">The second rectangle.</param>
		/// <returns>The union of the two rectangles.</returns>
		public static Rect Union(Rect rect1, Rect rect2)
		{
			Rect Union = new(new float?(rect1.Left == null || rect2.Left == null ? 0f : Math.Min(rect1.Left.Value, rect2.Left.Value)), new float?(rect1.Top == null || rect2.Top == null ? 0f : Math.Min(rect1.Top.Value, rect2.Top.Value)), new float?(rect1.Right == null || rect2.Right == null ? 0f : Math.Min(rect1.Right.Value, rect2.Right.Value)), new float?(rect1.Bottom == null || rect2.Bottom == null ? 0f : Math.Min(rect1.Bottom.Value, rect2.Bottom.Value)));
			return Union;
		}
		/// <summary>
		/// Returns the intersection of two rectangles.
		/// </summary>
		/// <param name="rect1">The first rectangle.</param>
		/// <param name="rect2">The second rectangle.</param>
		/// <returns>The intersection of the two rectangles.</returns>
		public static Rect Intersect(Rect rect1, Rect rect2) => rect1.IntersectsWithInclusive(rect2) ? new Rect(new float?(rect1.Left == null || rect2.Left == null ? 0f : Math.Max(rect1.Left.Value, rect2.Left.Value)), new float?(rect1.Top == null || rect2.Top == null ? 0f : Math.Max(rect1.Top.Value, rect2.Top.Value)), new float?(rect1.Right == null || rect2.Right == null ? 0f : Math.Min(rect1.Right.Value, rect2.Right.Value)), new float?(rect1.Bottom == null || rect2.Bottom == null ? 0f : Math.Min(rect1.Bottom.Value, rect2.Bottom.Value))) : default;

		/// <summary>
		/// Truncates the edges of the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to truncate.</param>
		/// <returns>The truncated rectangle.</returns>
		public static Rect Truncate(Rect rect)
		{
			Rect Truncate = new(
				rect.Left == null ? null : (float)Math.Truncate((double)rect.Left),
				rect.Top == null ? null : (float)Math.Truncate((double)rect.Top),
				rect.Right == null ? null : (float)Math.Truncate((double)rect.Right),
				rect.Bottom == null ? null : (float)Math.Truncate((double)rect.Bottom));
			return Truncate;
		}
		/// <summary>
		/// Offsets the rectangle by the specified width and height.
		/// </summary>
		/// <param name="x">The width to offset by.</param>
		/// <param name="y">The height to offset by.</param>
		public void Offset(float? x, float? y)
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
		public void Offset(Point p) => Offset(p.X, p.Y);
		/// <summary>
		/// Inflates the rectangle by the specified size.
		/// </summary>
		/// <param name="size">The size to inflate by.</param>
		public void Inflate(Size size)
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
		public void Inflate(float? width, float? height)
		{
			Left -= width;
			Top += height;
			Right += width;
			Bottom -= height;
		}
		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the point.</param>
		/// <param name="y">The y-coordinate of the point.</param>
		/// <returns>true if the rectangle contains the point; otherwise, false.</returns>
		public readonly bool Contains(float? x, float? y) => Left < x && x < Right && Bottom < y && y < Top;
		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="p">The point to check.</param>
		/// <returns>true if the rectangle contains the point; otherwise, false.</returns>
		public readonly bool Contains(Point p) => Contains(p.X, p.Y);
		/// <summary>
		/// Determines whether the rectangle contains the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to check.</param>
		/// <returns>true if the rectangle contains the specified rectangle; otherwise, false.</returns>
		public readonly bool Contains(Rect rect) => Left < rect.Left && rect.Right < Right && Bottom < rect.Bottom && rect.Top < Top;
		/// <summary>
		/// Returns the union of this rectangle and the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to union with.</param>
		/// <returns>The union of the two rectangles.</returns>
		public readonly Rect Union(Rect rect) => Union(this, rect);
		/// <summary>
		/// Returns the intersection of this rectangle and the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to intersect with.</param>
		/// <returns>The intersection of the two rectangles.</returns>
		public readonly object Intersect(Rect rect) => Intersect(this, rect);
		/// <summary>
		/// Determines whether this rectangle intersects with the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to check.</param>
		/// <returns>true if the rectangles intersect; otherwise, false.</returns>
		public readonly bool IntersectsWith(Rect rect) => Left < rect.Right && Right > rect.Left && Top < rect.Bottom && Bottom > rect.Top;
		/// <summary>
		/// Determines whether this rectangle intersects with the specified rectangle, including edges.
		/// </summary>
		/// <param name="rect">The rectangle to check.</param>
		/// <returns>true if the rectangles intersect; otherwise, false.</returns>
		public readonly bool IntersectsWithInclusive(Rect rect) => Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom && Bottom >= rect.Top;
		/// <inheritdoc/>
		public static bool operator ==(Rect rect1, Rect rect2) => rect1.Equals(rect2);
		/// <inheritdoc/>
		public static bool operator !=(Rect rect1, Rect rect2) => !rect1.Equals(rect2);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is Rect e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + (Left?.GetHashCode() ?? 0);
			hash = hash * 23 + (Top?.GetHashCode() ?? 0);
			hash = hash * 23 + (Right?.GetHashCode() ?? 0);
			hash = hash * 23 + (Bottom?.GetHashCode() ?? 0);
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"{{Location=[{Left?.ToString() ?? "null"},{Bottom?.ToString() ?? "null"}],Size=[{Width?.ToString() ?? "null"},{Height?.ToString() ?? "null"}]}}";
		/// <inheritdoc/>
		public readonly bool Equals(Rect other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
		private readonly string GetDebuggerDisplay() => ToString();
	}