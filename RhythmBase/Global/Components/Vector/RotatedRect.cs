using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rotated rectangle with non-integer coordinates.
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct RotatedRect(Point location, Size size, Point? pivot, float? angle = 0) : IEquatable<RotatedRect>
	{
		/// <summary>
		/// Gets or sets the location of the rectangle.
		/// </summary>
		public Point? Location { get; set; } = location;
		/// <summary>
		/// Gets or sets the size of the rectangle.
		/// </summary>
		public Size? Size { get; set; } = size;
		/// <summary>
		/// Gets or sets the pivot point of the rotation.
		/// </summary>
		public Point? Pivot { get; set; } = pivot;
		/// <summary>
		/// Gets or sets the angle of rotation in degrees.
		/// </summary>
		public float? Angle { get; set; } = angle;
		/// <summary>
		/// Gets the rectangle without rotation.
		/// </summary>
		public readonly Rect WithoutRotate => new(Location - (Size?)Pivot, Size);
		/// <summary>
		/// IItializes a new instance of the <see cref="RotatedRect"/> struct.
		/// </summary>
		/// <param name="rect">The rectangle.</param>
		/// <param name="pivot">The pivot point.</param>
		/// <param name="angle">The angle of rotation.</param>
		public RotatedRect(Rect rect, Point? pivot, float angle) : this(rect.Location, rect.Size, pivot, angle) { }
		/// <summary>
		/// IItializes a new instance of the <see cref="RotatedRect"/> struct.
		/// </summary>
		/// <param name="rect">The rectangle.</param>
		public RotatedRect(Rect rect) : this(rect.Location, rect.Size, default, 0f) { }
		/// <summary>
		/// Inflates the specified rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RotatedRect Inflate(RotatedRect rect, Size size)
		{
			RotatedRect result = rect;
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
		public static RotatedRect Inflate(RotatedRect rect, int x, int y)
		{
			RotatedRect result = rect;
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Offsets the rectangle by the specified x and y values.
		/// </summary>
		/// <param name="x">The x value to offset by.</param>
		/// <param name="y">The y value to offset by.</param>
		public void Offset(float? x, float? y) => Location += new Size(x, y);
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
			Size += new Size(size.Width * 2, size.Height * 2);
			Pivot -= new Size(size.Width, size.Height);
		}
		/// <summary>
		/// Inflates the rectangle by the specified width and height.
		/// </summary>
		/// <param name="width">The width to inflate by.</param>
		/// <param name="height">The height to inflate by.</param>
		public void Inflate(float? width, float? height)
		{
			Size += new Size(width * 2, height * 2);
			Pivot -= new Size(width, height);
		}
		/// <inheritdoc/>
		public static bool operator ==(RotatedRect rect1, RotatedRect rect2) => rect1.Equals(rect2);
		/// <inheritdoc/>
		public static bool operator !=(RotatedRect rect1, RotatedRect rect2) => !rect1.Equals(rect2);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is RotatedRect e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + (Location?.GetHashCode() ?? 0);
			hash = hash * 31 + (Size?.GetHashCode() ?? 0);
			hash = hash * 31 + (Pivot?.GetHashCode() ?? 0);
			hash = hash * 31 + (Angle?.GetHashCode() ?? 0);
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Location, Size, Pivot, Angle);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"{{Location=[{Location}],Size=[{Size}],Pivot[{Pivot}],Angle={Angle}}}";
		/// <inheritdoc/>
		public readonly bool Equals(RotatedRect other) =>
			Location == other.Location &&
			Size == other.Size && Pivot
			== other.Pivot &&
			Angle == other.Angle;
		private readonly string GetDebuggerDisplay() => ToString();
	}