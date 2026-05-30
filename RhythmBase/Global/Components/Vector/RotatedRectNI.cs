using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rotated rectangle with non-integer coordinates.
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct RotatedRectNI(PointNI location, SizeNI size, PointNI pivot, float angle) : IEquatable<RotatedRectNI>
	{
		/// <summary>
		/// Gets or sets the location of the rectangle.
		/// </summary>
		public PointNI Location { get; set; } = location;
		/// <summary>
		/// Gets or sets the size of the rectangle.
		/// </summary>
		public SizeNI Size { get; set; } = size;
		/// <summary>
		/// Gets or sets the pivot point of the rotation.
		/// </summary>
		public PointNI Pivot { get; set; } = pivot;
		/// <summary>
		/// Gets or sets the angle of rotation in degrees.
		/// </summary>
		public float Angle { get; set; } = angle;
		/// <summary>
		/// Gets the top-left point of the rotated rectangle.
		/// </summary>
		public readonly PointN LeftTop => (Location - (SizeNI)Pivot + new SizeNI(0, Size.Height)).Rotate(Location, Angle);
		/// <summary>
		/// Gets the top-right point of the rotated rectangle.
		/// </summary>
		public readonly PointN RightTop => (Location - (SizeNI)Pivot + Size).Rotate(Location, Angle);
		/// <summary>
		/// Gets the bottom-left point of the rotated rectangle.
		/// </summary>
		public readonly PointN LeftBottom => (Location - (SizeNI)Pivot).Rotate(Location, Angle);
		/// <summary>
		/// Gets the bottom-right point of the rotated rectangle.
		/// </summary>
		public readonly PointN RightBottom => (Location - (SizeNI)Pivot + new SizeNI(Size.Width, 0)).Rotate(Location, Angle);
		/// <summary>
		/// Gets the rectangle without rotation.
		/// </summary>
		public readonly RectNI WithoutRotate => new(Location - (SizeNI)Pivot, Size);
		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedRectNI"/> struct.
		/// </summary>
		/// <param name="rect">The rectangle.</param>
		/// <param name="pivot">The pivot point.</param>
		/// <param name="angle">The angle of rotation.</param>
		public RotatedRectNI(RectNI rect, PointNI pivot, float angle) : this(rect.Location, rect.Size, pivot, angle) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedRectNI"/> struct.
		/// </summary>
		/// <param name="rect">The rectangle.</param>
		public RotatedRectNI(RectNI rect) : this(rect.Location, rect.Size, default, 0f) { }
		/// <summary>
		/// Inflates the specified rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RotatedRectNI Inflate(RotatedRectNI rect, SizeNI size)
		{
			RotatedRectNI result = rect;
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
		public static RotatedRectNI Inflate(RotatedRectNI rect, int x, int y)
		{
			RotatedRectNI result = rect;
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Offsets the rectangle by the specified x and y values.
		/// </summary>
		/// <param name="x">The x value to offset by.</param>
		/// <param name="y">The y value to offset by.</param>
		public void Offset(int x, int y) => Location += (SizeNI)new PointNI(x, y);
		/// <summary>
		/// Offsets the rectangle by the specified point.
		/// </summary>
		/// <param name="p">The point to offset by.</param>
		public void Offset(PointNI p) => Offset(p.X, p.Y);
		/// <summary>
		/// Inflates the rectangle by the specified size.
		/// </summary>
		/// <param name="size">The size to inflate by.</param>
		public void Inflate(SizeNI size)
		{
			Size += new SizeNI(size.Width * 2, size.Height * 2);
			Pivot -= (SizeNI)new PointNI(size.Width, size.Height);
		}
		/// <summary>
		/// Inflates the rectangle by the specified width and height.
		/// </summary>
		/// <param name="width">The width to inflate by.</param>
		/// <param name="height">The height to inflate by.</param>
		public void Inflate(int width, int height)
		{
			Size += new SizeNI(width * 2, height * 2);
			Pivot -= (SizeNI)new PointNI(width, height);
		}
		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="x">The x coordinate of the point.</param>
		/// <param name="y">The y coordinate of the point.</param>
		/// <returns><c>true</c> if the rectangle contains the point; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(int x, int y) => WithoutRotate.Contains(new PointN(x, y).Rotate(-Angle));

		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="p">The point.</param>
		/// <returns><c>true</c> if the rectangle contains the point; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(PointN p) => WithoutRotate.Contains(p.Rotate(-Angle));
		/// <summary>
		/// Determines whether the rectangle contains the specified rotated rectangle.
		/// </summary>
		/// <param name="rect">The rotated rectangle.</param>
		/// <returns><c>true</c> if the rectangle contains the rotated rectangle; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(RotatedRectNI rect) =>
			Contains(rect.LeftTop) &&
			Contains(rect.RightTop) &&
			Contains(rect.LeftBottom) &&
			Contains(rect.RightBottom);
		/// <summary>
		/// Determines whether the rectangle intersects with the specified rotated rectangle.
		/// </summary>
		/// <param name="rect">The rotated rectangle.</param>
		/// <returns><c>true</c> if the rectangle intersects with the rotated rectangle; otherwise, <c>false</c>.</returns>
		public readonly bool IntersectsWith(RotatedRectNI rect) =>
			Contains(rect.LeftTop) ||
			Contains(rect.RightTop) ||
			Contains(rect.LeftBottom) ||
			Contains(rect.RightBottom);
		/// <inheritdoc/>
		public static bool operator ==(RotatedRectNI rect1, RotatedRectNI rect2) => rect1.Equals(rect2);
		/// <inheritdoc/>
		public static bool operator !=(RotatedRectNI rect1, RotatedRectNI rect2) => !rect1.Equals(rect2);
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is RotatedRectNI e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + Location.GetHashCode();
			hash = hash * 31 + Size.GetHashCode();
			hash = hash * 31 + Pivot.GetHashCode();
			hash = hash * 31 + Angle.GetHashCode();
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Location, Size, Pivot, Angle);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"{{Location=[{Location}],Size=[{Size}],Pivot[{Pivot}],Angle=[{Angle}]}}";
		/// <inheritdoc/>
		public readonly bool Equals(RotatedRectNI other) =>
			Location == other.Location &&
			Size == other.Size && Pivot
			== other.Pivot &&
			Angle == other.Angle;
		private readonly string GetDebuggerDisplay() => ToString();
	}