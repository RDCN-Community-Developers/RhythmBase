using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rectangle structure containing left, top, right, and bottom boundary values.
	/// </summary>
	/// <param name="left">The left boundary of the rectangle.</param>
	/// <param name="top">The top boundary of the rectangle.</param>
	/// <param name="right">The right boundary of the rectangle.</param>
	/// <param name="bottom">The bottom boundary of the rectangle.</param>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct RectI(int? left, int? top, int? right, int? bottom) : IEquatable<RectI>
	{
		/// <summary>
		/// Gets or sets the left boundary of the rectangle.
		/// </summary>
		public int? Left { get; set; } = left;
		/// <summary>
		/// Gets or sets the right boundary of the rectangle.
		/// </summary>
		public int? Right { get; set; } = right;
		/// <summary>
		/// Gets or sets the top boundary of the rectangle.
		/// </summary>
		public int? Top { get; set; } = top;
		/// <summary>
		/// Gets or sets the bottom boundary of the rectangle.
		/// </summary>
		public int? Bottom { get; set; } = bottom;
		/// <summary>
		/// Gets the bottom-left corner point of the rectangle.
		/// </summary>
		public readonly PointI LeftBottom => new(Left, Bottom);

		/// <summary>
		/// Gets the bottom-right corner point of the rectangle.
		/// </summary>
		public readonly PointI RightBottom => new(Right, Bottom);

		/// <summary>
		/// Gets the top-left corner point of the rectangle.
		/// </summary>
		public readonly PointI LeftTop => new(Left, Top);

		/// <summary>
		/// Gets the top-right corner point of the rectangle.
		/// </summary>
		public readonly PointI RightTop => new(Right, Top);

		/// <summary>
		/// Gets the width of the rectangle.
		/// </summary>
		public readonly int? Width => checked(Right - Left);
		/// <summary>
		/// Gets the height of the rectangle.
		/// </summary>
		public readonly int? Height => checked(Top - Bottom);
		/// <summary>
		/// Initializes a new instance of the rectangle with the specified location and size.
		/// </summary>
		/// <param name="location">The location of the rectangle.</param>
		/// <param name="size">The size of the rectangle.</param>
		public RectI(PointI? location, SizeI? size) : this(location?.X, location?.Y + size?.Height, location?.X + size?.Width, location?.Y) { }
		/// <summary>
		/// Initializes a new instance of the rectangle with the specified size.
		/// </summary>
		/// <param name="size">The size of the rectangle.</param>
		public RectI(SizeI? size) : this(0, size?.Height, size?.Width, 0) { }
		/// <summary>
		/// Initializes a new instance of the rectangle with the specified width and height.
		/// </summary>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public RectI(int? width, int? height) : this(0, height, width, 0) { }
		/// <summary>
		/// Gets the location of the rectangle.
		/// </summary>
		public readonly PointI? Location => Left is null && Right is null ? null : new(Left, Bottom);
		/// <summary>
		/// Gets the size of the rectangle.
		/// </summary>
		public readonly SizeI? Size => Width is null && Height is null ? null : new(Width, Height);
		/// <summary>
		/// Inflates the rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RectI Inflate(RectI rect, SizeI size)
		{
			RectI result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
			result.Inflate(size);
			return result;
		}
		/// <summary>
		/// Inflates the rectangle by the specified width and height.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="x">The width to inflate by.</param>
		/// <param name="y">The height to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RectI Inflate(RectI rect, int? x, int? y)
		{
			RectI result = new(rect.Left, rect.Top, rect.Right, rect.Bottom);
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Converts the specified RDRect to RDRectI using ceiling.
		/// </summary>
		/// <param name="rect">The RDRect to convert.</param>
		/// <returns>The converted RDRectI.</returns>
		public static RectI Ceiling(Rect rect) => Ceiling(rect, false);
		/// <summary>
		/// Converts the specified RDRect to RDRectI using ceiling, and specifies whether to expand outwards.
		/// </summary>
		/// <param name="rect">The RDRect to convert.</param>
		/// <param name="outwards">Whether to expand outwards.</param>
		/// <returns>The converted RDRectI.</returns>
		public static RectI Ceiling(Rect rect, bool outwards) => new(
				rect.Left == null ? null : (int)(outwards && rect.Width > 0 ? Math.Floor((double)rect.Left) : Math.Ceiling((double)rect.Left)),
				rect.Top == null ? null : (int)(outwards && rect.Height > 0 ? Math.Floor((double)rect.Top) : Math.Ceiling((double)rect.Top)),
				rect.Right == null ? null : (int)(outwards && rect.Width < 0 ? Math.Floor((double)rect.Right) : Math.Ceiling((double)rect.Right)),
				rect.Bottom == null ? null : (int)(outwards && rect.Height < 0 ? Math.Floor((double)rect.Bottom) : Math.Ceiling((double)rect.Bottom)));
		/// <summary>
		/// Converts the specified RDRect to RDRectI using floor.
		/// </summary>
		/// <param name="rect">The RDRect to convert.</param>
		/// <returns>The converted RDRectI.</returns>
		public static RectI Floor(Rect rect) => Ceiling(rect, false);
		/// <summary>
		/// Converts the specified RDRect to RDRectI using floor, and specifies whether to shrink inwards.
		/// </summary>
		/// <param name="rect">The RDRect to convert.</param>
		/// <param name="inwards">Whether to shrink inwards.</param>
		/// <returns>The converted RDRectI.</returns>
		public static RectI Floor(Rect rect, bool inwards) => new(
				rect.Left == null ? null : (int)(inwards && rect.Width > 0 ? Math.Ceiling((double)rect.Left) : Math.Floor((double)rect.Left)),
				rect.Top == null ? null : (int)(inwards && rect.Height > 0 ? Math.Ceiling((double)rect.Top) : Math.Floor((double)rect.Top)),
				rect.Right == null ? null : (int)(inwards && rect.Width < 0 ? Math.Ceiling((double)rect.Right) : Math.Floor((double)rect.Right)),
				rect.Bottom == null ? null : (int)(inwards && rect.Height < 0 ? Math.Ceiling((double)rect.Bottom) : Math.Floor((double)rect.Bottom)));
		/// <summary>
		/// Converts the specified RDRect to RDRectI using rounding.
		/// </summary>
		/// <param name="rect">The RDRect to convert.</param>
		/// <returns>The converted RDRectI.</returns>
		public static RectI Round(Rect rect) => new(
				new int?((int)Math.Round(rect.Left == null ? 0.0 : Math.Round((double)rect.Left.Value))),
				new int?((int)Math.Round(rect.Top == null ? 0.0 : Math.Round((double)rect.Top.Value))),
				new int?((int)Math.Round(rect.Right == null ? 0.0 : Math.Round((double)rect.Right.Value))),
				new int?((int)Math.Round(rect.Bottom == null ? 0.0 : Math.Round((double)rect.Bottom.Value))));

		/// <summary>
		/// Returns a new RDRectI that is the union of two rectangles.
		/// </summary>
		/// <param name="rect1">The first rectangle.</param>
		/// <param name="rect2">The second rectangle.</param>
		/// <returns>The union of the two rectangles.</returns>
		public static RectI Union(RectI rect1, RectI rect2) => new(
				new int?(rect1.Left == null || rect2.Left == null ? 0 : Math.Min(rect1.Left.Value, rect2.Left.Value)),
				new int?(rect1.Top == null || rect2.Top == null ? 0 : Math.Min(rect1.Top.Value, rect2.Top.Value)),
				new int?(rect1.Right == null || rect2.Right == null ? 0 : Math.Min(rect1.Right.Value, rect2.Right.Value)),
				new int?(rect1.Bottom == null || rect2.Bottom == null ? 0 : Math.Min(rect1.Bottom.Value, rect2.Bottom.Value)));

		/// <summary>
		/// Returns a new RDRectI that is the intersection of two rectangles.
		/// </summary>
		/// <param name="rect1">The first rectangle.</param>
		/// <param name="rect2">The second rectangle.</param>
		/// <returns>The intersection of the two rectangles.</returns>
		public static RectI Intersect(RectI rect1, RectI rect2) => rect1.IntersectsWithInclusive(rect2) ? new RectI(
			new int?(rect1.Left == null || rect2.Left == null ? 0 : Math.Max(rect1.Left.Value, rect2.Left.Value)),
			new int?(rect1.Top == null || rect2.Top == null ? 0 : Math.Max(rect1.Top.Value, rect2.Top.Value)),
			new int?(rect1.Right == null || rect2.Right == null ? 0 : Math.Min(rect1.Right.Value, rect2.Right.Value)),
			new int?(rect1.Bottom == null || rect2.Bottom == null ? 0 : Math.Min(rect1.Bottom.Value, rect2.Bottom.Value))) : default;

		/// <summary>
		/// Converts the specified RDRect to RDRectI by truncating the decimal part.
		/// </summary>
		/// <param name="rect">The RDRect to convert.</param>
		/// <returns>The converted RDRectI.</returns>
		public static RectI Truncate(Rect rect) => new(
				(int?)rect.Left,
				(int?)rect.Top,
				(int?)rect.Right,
				(int?)rect.Bottom);
		/// <summary>
		/// Offsets the rectangle by the specified amounts.
		/// </summary>
		/// <param name="x">The horizontal offset.</param>
		/// <param name="y">The vertical offset.</param>
		public void Offset(int? x, int? y)
		{
			Left += x;
			Top += y;
			Right += x;
			Bottom += y;
		}
		/// <summary>
		/// Moves the rectangle by the specified point.
		/// </summary>
		/// <param name="p">The point containing the offset.</param>
		public void Offset(PointI p) => Offset(p.X, p.Y);
		/// <summary>
		/// Inflates the rectangle by the specified size.
		/// </summary>
		/// <param name="size">The size to inflate by.</param>
		public void Inflate(SizeI size)
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
		public void Inflate(int? width, int? height)
		{
			Left -= width;
			Top += height;
			Right += width;
			Bottom -= height;
		}
		///<summary>
		/// Returns a new RDRectI that is the union of the current rectangle and the specified rectangle.
		/// </summary>
		/// <param name="rect">The rectangle to merge with.</param>
		/// <returns>The union of the two rectangles.</returns>
		public readonly RectI Union(RectI rect) => Union(this, rect);
		/// <summary>
		/// Determines whether the current rectangle intersects with the specified rectangle (including edges).
		/// </summary>
		/// <param name="rect">The rectangle to check.</param>
		/// <returns>true if the two rectangles intersect (including edges); otherwise, false.</returns>
		public readonly bool IntersectsWithInclusive(RectI rect) => Left <= rect.Right && Right >= rect.Left && Top <= rect.Bottom && Bottom >= rect.Top;
		/// <inheritdoc/>
		public static bool operator ==(RectI rect1, RectI rect2) => rect1.Equals(rect2);
		/// <inheritdoc/>
		public static bool operator !=(RectI rect1, RectI rect2) => !rect1.Equals(rect2);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is RectI e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			unchecked
			{
				return Left.GetHashCode() ^ Top.GetHashCode()
					^ Right.GetHashCode() ^ Bottom.GetHashCode();
			};
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"{{Location=[{Left},{Bottom}],Size=[{Width},{Height}]}}";
		/// <inheritdoc/>
		public readonly bool Equals(RectI other) => Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
		/// <summary>
		/// Implicitly converts an <see cref="RectI"/> to an <see cref="Rect"/>.
		/// </summary>
		/// <param name="rect">The <see cref="RectI"/> instance to convert.</param>
		/// <returns>The converted <see cref="Rect"/> instance.</returns>
		public static implicit operator Rect(RectI rect) => new(rect.Left, rect.Top, rect.Right, rect.Bottom);
		private readonly string GetDebuggerDisplay() => ToString();
	}
