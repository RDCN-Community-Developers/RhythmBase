using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
namespace RhythmBase.Global.Components.Vector;

	/// <summary>
	/// A size whose horizontal and vertical coordinates are <strong>nullable</strong> <see langword="integer" />
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public struct SizeI(int? width, int? height) : IVector<SizeI, SizeI, int?>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SizeI"/> struct with the specified point.
		/// </summary>
		/// <param name="pt">The point to initialize the size with.</param>
		public SizeI(PointI pt) : this(pt.X, pt.Y) { }
		/// <summary>
		/// Gets a value indicating whether this size is empty (both width and height are null).
		/// </summary>
		public readonly bool IsEmpty => Width == null && Height == null;
		/// <summary>
		/// Gets or sets the width of the size.
		/// </summary>
		public int? Width { get; set; } = width;
		/// <summary>
		/// Gets or sets the height of the size.
		/// </summary>
		public int? Height { get; set; } = height;
		/// <summary>
		/// Gets the area of the size (width multiplied by height).
		/// </summary>
		public readonly int? Area => Width * Height;
		/// <summary>
		/// Adds two sizes together.
		/// </summary>
		/// <param name="sz1">The first size.</param>
		/// <param name="sz2">The second size.</param>
		/// <returns>The sum of the two sizes.</returns>
		public static SizeI Add(SizeI sz1, SizeI sz2) => new(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		/// <summary>
		/// Truncates the specified size to the nearest integer values.
		/// </summary>
		/// <param name="value">The size to truncate.</param>
		/// <returns>The truncated size.</returns>
		public static SizeI Truncate(Size value) => new(
			(int)Math.Round(value.Width == null ? 0.0 : Math.Truncate((double)value.Width.Value)),
			(int)Math.Round(value.Height == null ? 0.0 : Math.Truncate((double)value.Height.Value)));

		/// <summary>
		/// Subtracts one size from another.
		/// </summary>
		/// <param name="sz1">The size to subtract from.</param>
		/// <param name="sz2">The size to subtract.</param>
		/// <returns>The difference between the two sizes.</returns>
		public static SizeI Subtract(SizeI sz1, SizeI sz2) => new(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		/// <summary>
		/// Rounds the specified size up to the nearest integer values.
		/// </summary>
		/// <param name="value">The size to round up.</param>
		/// <returns>The rounded size.</returns>
		public static SizeI Ceiling(Size value) => new(
			(int)Math.Round(value.Width == null ? 0.0 : Math.Ceiling((double)value.Width.Value)),
			(int)Math.Round(value.Height == null ? 0.0 : Math.Ceiling((double)value.Height.Value)));

		/// <summary>
		/// Rounds the specified size to the nearest integer values.
		/// </summary>
		/// <param name="value">The size to round.</param>
		/// <returns>The rounded size.</returns>
		public static SizeI Round(Size value) => new(
				new int?((int)Math.Round(value.Width == null ? 0.0 : Math.Round((double)value.Width.Value))),
				new int?((int)Math.Round(value.Height == null ? 0.0 : Math.Round((double)value.Height.Value))));
		/// <inheritdoc/>
		public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is SizeI e && Equals(e);
		/// <inheritdoc/>
#if NETSTANDARD
		public readonly override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + (Width?.GetHashCode() ?? 0);
			hash = hash * 23 + (Height?.GetHashCode() ?? 0);
			return hash;
		}
#else
		public readonly override int GetHashCode() => HashCode.Combine(Width, Height);
#endif
		/// <inheritdoc/>
		public readonly override string ToString() => $"[{Width},{Height}]";
		/// <inheritdoc/>
		public readonly bool Equals(SizeI other) => Width == other.Width && Height == other.Height;
		/// <inheritdoc/>
		public static SizeI operator +(SizeI sz1, SizeI sz2) => Add(sz1, sz2);
		/// <inheritdoc/>
		public static SizeI operator -(SizeI sz1, SizeI sz2) => Subtract(sz1, sz2);
		/// <inheritdoc/>
		public static Size operator *(float left, SizeI right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static Size operator *(SizeI left, float right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static SizeI operator *(int left, SizeI right) => new(left * right.Width, left * right.Height);
		/// <inheritdoc/>
		public static SizeI operator *(SizeI left, int? right) => new(left.Width * right, left.Height * right);
		/// <inheritdoc/>
		public static Size operator /(SizeI left, float right) => new(left.Width / right, left.Height / right);
		/// <inheritdoc/>
		public static SizeI operator /(SizeI left, int? right) => new(left.Width / right, left.Height / right);
		/// <inheritdoc/>
		public static bool operator ==(SizeI sz1, SizeI sz2) => sz1.Equals(sz2);
		/// <inheritdoc/>
		public static bool operator !=(SizeI sz1, SizeI sz2) => !sz1.Equals(sz2);
		/// <summary>
		/// Implicitly converts an <see cref="SizeI"/> to an <see cref="Size"/>.
		/// </summary>
		/// <param name="p">The <see cref="SizeI"/> to convert.</param>
		/// <returns>A new <see cref="Size"/> with the same width and height.</returns>
		public static implicit operator Size(SizeI p) => new(p.Width, p.Height);
		/// <summary>
		/// Explicitly converts an <see cref="SizeI"/> to an <see cref="PointI"/>.
		/// </summary>
		/// <param name="size">The <see cref="SizeI"/> to convert.</param>
		/// <returns>A new <see cref="PointI"/> with the same width and height.</returns>
		public static explicit operator PointI(SizeI size) => new(size.Width, size.Height);
		private readonly string GetDebuggerDisplay() => ToString();
		/// <summary>
		/// Implicitly converts a tuple containing two <see cref="int"/>?.
		/// </summary>
		/// <remarks>This conversion enables concise and readable initialization of <see cref="SizeI"/> instances from tuples,
		/// allowing for more flexible assignment and construction patterns.</remarks>
		/// <param name="tuple">A tuple whose elements represent the x and y coordinates as <see cref="int"/>?. Either element may be null.</param>
		public static implicit operator SizeI((int? width, int? height) tuple) => new(tuple.width, tuple.height);
		/// <summary>
		/// Deconstructs the current instance into its Width and Height component values.
		/// </summary>
		/// <param name="width">When this method returns, contains the value of the Width component of the current instance.</param>
		/// <param name="height">When this method returns, contains the value of the Height component of the current instance.</param>
		public readonly void Deconstruct(out int? width, out int? height) { width = Width; height = Height; }
	}
