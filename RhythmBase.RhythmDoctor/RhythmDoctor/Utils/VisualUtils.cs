using RhythmBase.Global.Components.Vector;

namespace RhythmBase.RhythmDoctor.Utils;

	/// <summary>
	/// Visual utils.
	/// </summary>
	public static class VisualUtils
	{
		/// <summary>
		/// Converts percentage point to pixel point with default screen size (352 * 198).
		/// </summary>
		public static PointE PercentToPixel(PointE point) => PercentToPixel(point, SizeNI.Screen);
		/// <summary>
		/// Converts percentage point to pixel point with specified size.
		/// </summary>
		/// <param name="point">The percentage point.</param>
		/// <param name="size">Specified size.</param>
		/// <returns>The pixel point.</returns>
		public static PointE PercentToPixel(PointE point, SizeE size)
		{
			PointE PercentToPixel = new(point.X * size.Width / 100f, point.Y * size.Height / 100f);
			return PercentToPixel;
		}
		/// <summary>
		/// Converts pixel point to percentage point with default screen size (352 * 198).
		/// </summary>
		/// <param name="point">The pixel point.</param>
		/// <returns>The percentage point.</returns>
		public static (float? X, float? Y) PixelToPercent((float X, float Y) point) => PixelToPercent(point, (352f, 198f));
		/// <summary>
		/// Converts pixel point to percentage point with specified size.
		/// </summary>
		/// <param name="point">The pixel point.</param>
		/// <param name="size">Specified size.</param>
		/// <returns>The percentage point.</returns>
		public static (float? X, float? Y) PixelToPercent((float? X, float? Y) point, (float? X, float? Y) size)
		{
			(float? X, float? Y) PixelToPercent = (point.X * 100f / size.X, point.Y * 100f / size.Y);
			return PixelToPercent;
		}
		/// <summary>
		/// Translates a point within a room perspective based on the specified corner points of the room.
		/// </summary>
		/// <remarks>This method maps a normalized point to a custom quadrilateral defined by the four corner points
		/// of the room perspective. The input point <paramref name="p"/> is expected to be in normalized coordinates, where
		/// the X and Y values range from 0 to 1.</remarks>
		/// <param name="p">The point to translate, represented as a normalized coordinate where (0, 0) corresponds to the bottom-left corner
		/// and (1, 1) corresponds to the top-right corner.</param>
		/// <param name="plb">The bottom-left corner of the room perspective.</param>
		/// <param name="prb">The bottom-right corner of the room perspective.</param>
		/// <param name="plt">The top-left corner of the room perspective.</param>
		/// <param name="prt">The top-right corner of the room perspective.</param>
		/// <returns>The translated point in the room perspective, adjusted based on the provided corner points.</returns>
		public static PointN RoomPerspectiveTranslate(this PointN p, PointN plb, PointN prb, PointN plt, PointN prt)
		{
			PointN p1p2 = prb - (SizeN)plb;
			PointN p1p4 = plt - (SizeN)plb;
			PointN p1p3 = prt - (SizeN)plb;
			PointN pr = plb + new SizeN(p1p2.X * p.X, p1p2.Y * p.X) +
				new SizeN((p1p3.X - p1p2.X - p1p4.X) * p.X * p.Y, (p1p3.Y - p1p2.Y - p1p4.Y) * p.X * p.Y) +
				new SizeN(p1p4.X * p.Y, p1p4.Y * p.Y);
			return pr;
		}
		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="degree">The angle in degrees.</param>
		/// <returns>The angle in radians.</returns>
		public static float DegreeToRadius(float degree) => (float)Math.PI * degree / 180f;
		/// <summary>
		/// Converts radians to degrees.
		/// </summary>
		/// <param name="radius">The angle in radians.</param>
		/// <returns>The angle in degrees.</returns>
		public static float RadiusToDegree(float radius) => radius * 180f / (float)Math.PI;
	}
