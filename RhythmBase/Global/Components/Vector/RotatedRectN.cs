using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// Represents a rotated rectangle with non-nullable float coordinates.
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct RotatedRectN(PointN location, SizeN size, PointN pivot, float angle = 0) : IEquatable<RotatedRectN>
	{
		/// <summary>
		/// Gets or sets the location of the rectangle.
		/// </summary>
		public PointN Location { get; set; } = location;
		/// <summary>
		/// Gets or sets the size of the rectangle.
		/// </summary>
		public SizeN Size { get; set; } = size;
		/// <summary>
		/// Gets or sets the pivot point of the rotation.
		/// </summary>
		public PointN Pivot { get; set; } = pivot;
		/// <summary>
		/// Gets or sets the angle of rotation in radians.
		/// </summary>
		public float Angle { get; set; } = angle;
		/// <summary>
		/// Gets the top-left point of the rotated rectangle.
		/// </summary>
		public readonly PointN LeftTop => (Location - (SizeN)Pivot + new SizeN(0, Size.Height)).Rotate(Location, Angle);
		/// <summary>
		/// Gets the top-right point of the rotated rectangle.
		/// </summary>
		public readonly PointN RightTop => (Location - (SizeN)Pivot + Size).Rotate(Location, Angle);
		/// <summary>
		/// Gets the bottom-left point of the rotated rectangle.
		/// </summary>
		public readonly PointN LeftBottom => (Location - (SizeN)Pivot).Rotate(Location, Angle);
		/// <summary>
		/// Gets the bottom-right point of the rotated rectangle.
		/// </summary>
		public readonly PointN RightBottom => (Location - (SizeN)Pivot + new SizeN(Size.Width, 0)).Rotate(Location, Angle);
		/// <summary>
		/// Gets the rectangle without rotation.
		/// </summary>
		public readonly RectN WithoutRotate => new(Location - (SizeN)Pivot, Size);
		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedRectN"/> struct.
		/// </summary>
		/// <param name="rect">The rectangle.</param>
		/// <param name="pivot">The pivot point.</param>
		/// <param name="angle">The angle of rotation.</param>
		public RotatedRectN(RectN rect, PointN pivot, float angle) : this(rect.Location, rect.Size, pivot, angle) { }
		/// <summary>
		/// Initializes a new instance of the <see cref="RotatedRectN"/> struct.
		/// </summary>
		/// <param name="rect">The rectangle.</param>
		public RotatedRectN(RectN rect) : this(rect.Location, rect.Size, default, 0f) { }
		/// <summary>
		/// Inflates the specified rectangle by the specified size.
		/// </summary>
		/// <param name="rect">The rectangle to inflate.</param>
		/// <param name="size">The size to inflate by.</param>
		/// <returns>The inflated rectangle.</returns>
		public static RotatedRectN Inflate(RotatedRectN rect, SizeN size)
		{
			RotatedRectN result = rect;
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
		public static RotatedRectN Inflate(RotatedRectN rect, int x, int y)
		{
			RotatedRectN result = rect;
			result.Inflate(x, y);
			return result;
		}
		/// <summary>
		/// Offsets the rectangle by the specified x and y values.
		/// </summary>
		/// <param name="x">The x value to offset by.</param>
		/// <param name="y">The y value to offset by.</param>
		public void Offset(float x, float y) => Location += (SizeN)new PointN(x, y);
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
			Size += new SizeN(size.Width * 2, size.Height * 2);
			Pivot -= (SizeN)new PointN(size.Width, size.Height);
		}
		/// <summary>
		/// Inflates the rectangle by the specified width and height.
		/// </summary>
		/// <param name="width">The width to inflate by.</param>
		/// <param name="height">The height to inflate by.</param>
		public void Inflate(float width, float height)
		{
			Size += new SizeN(width * 2, height * 2);
			Pivot -= (SizeN)new PointN(width, height);
		}
		/// <summary>
		/// Determines whether the rectangle contains the specified point.
		/// </summary>
		/// <param name="x">The x coordinate of the point.</param>
		/// <param name="y">The y coordinate of the point.</param>
		/// <returns><c>true</c> if the rectangle contains the point; otherwise, <c>false</c>.</returns>
		public readonly bool Contains(float x, float y) => WithoutRotate.Contains(new PointN(x, y).Rotate(-Angle));
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
		public readonly bool Contains(RotatedRectN rect) => Contains(rect.LeftTop) &&
			Contains(rect.RightTop) &&
			Contains(rect.LeftBottom) &&
			Contains(rect.RightBottom);
		/// <summary>
		/// Determines whether the rectangle intersects with the specified rotated rectangle.
		/// </summary>
		/// <param name="rect">The rotated rectangle.</param>
		/// <returns><c>true</c> if the rectangle intersects with the rotated rectangle; otherwise, <c>false</c>.</returns>
		public readonly bool IntersectsWith(RotatedRectN rect) => Contains(rect.LeftTop) ||
			Contains(rect.RightTop) ||
			Contains(rect.LeftBottom) ||
			Contains(rect.RightBottom);
		/// <summary>
		/// Determines whether two rotated rectangles are equal.
		/// </summary>
		/// <param name="rect1">The first rotated rectangle.</param>
		/// <param name="rect2">The second rotated rectangle.</param>
		/// <returns><c>true</c> if the rectangles are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(RotatedRectN rect1, RotatedRectN rect2) => rect1.Equals(rect2);
		/// <summary>
		/// Determines whether two rotated rectangles are not equal.
		/// </summary>
		/// <param name="rect1">The first rotated rectangle.</param>
		/// <param name="rect2">The second rotated rectangle.</param>
		/// <returns><c>true</c> if the rectangles are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(RotatedRectN rect1, RotatedRectN rect2) => !rect1.Equals(rect2);
		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is RotatedRectN e && Equals(e);
		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + Location.GetHashCode();
			hash = hash * 31 + Size.GetHashCode();
			hash = hash * 31 + Pivot.GetHashCode() ;
			hash = hash * 31 + Angle.GetHashCode();
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Location, Size, Pivot, Angle);
#endif
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public readonly override string ToString() => $"{{Location=[{Location}],Size=[{Size}],Pivot[{Pivot}],Angle={Angle}}}";
		/// <summary>
		/// Determines whether the specified <see cref="RotatedRectN"/> is equal to the current <see cref="RotatedRectN"/>.
		/// </summary>
		/// <param name="other">The <see cref="RotatedRectN"/> to compare with the current <see cref="RotatedRectN"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="RotatedRectN"/> is equal to the current <see cref="RotatedRectN"/>; otherwise, <c>false</c>.</returns>
		public readonly bool Equals(RotatedRectN other) => Location == other.Location &&
			Size == other.Size && Pivot
			== other.Pivot &&
			Angle == other.Angle;
		private readonly string GetDebuggerDisplay() => ToString();
	}
