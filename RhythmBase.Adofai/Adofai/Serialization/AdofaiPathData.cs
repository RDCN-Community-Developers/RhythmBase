namespace RhythmBase.Adofai.Serialization;

/// <summary>
/// Port of the game's <c>FloorHelper</c> path-data decoder, used to migrate pre-v5 levels from the
/// legacy relative <c>pathData</c> string to an absolute <c>angleData</c> array.
/// </summary>
internal static class AdofaiPathData
{
	/// <summary>Decodes the legacy relative path string into absolute tile angles (degrees, 999 = midspin).</summary>
	public static float[] Migrate(string pathData)
	{
		float[] result = new float[pathData.Length];
		float previous = 0f;
		for (int i = 0; i < pathData.Length; i++)
		{
			char c = pathData[i];
			float angle;
			if (!TryGetAngle(c, out angle))
			{
				// Relative characters accumulate on the previous absolute angle.
				angle = previous + c switch
				{
					'5' => 72f,
					'6' => -72f,
					'7' => 52f,
					'8' => -52f,
					'9' => -30f,
					'h' => 120f,
					'j' => -120f,
					't' => 60f,
					'y' => 300f,
					_ => 0f,
				};
			}
			result[i] = angle;
			previous = angle;
		}
		return result;
	}

	/// <summary>Gets a value indicating whether <paramref name="c"/> is a legacy path-data character.</summary>
	public static bool IsPathChar(char c) => c is
		'R' or 'E' or 'U' or 'Q' or 'L' or 'Z' or 'D' or 'C' or 'B' or 'T' or 'G' or 'F' or 'J' or 'H' or 'N' or 'M'
		or 'p' or 'o' or 'q' or 'W' or 'x' or 'V' or 'Y' or 'A' or '!' or '5' or '6' or '7' or '8' or '9' or 'h' or 'j' or 't' or 'y';

	private static bool TryGetAngle(char pathId, out float angle)
	{
		float? value = pathId switch
		{
			'R' => 0f,
			'E' => 45f,
			'U' => 90f,
			'Q' => 135f,
			'L' => 180f,
			'Z' => 225f,
			'D' => 270f,
			'C' => 315f,
			'B' => 300f,
			'T' => 60f,
			'G' => 120f,
			'F' => 240f,
			'J' => 30f,
			'H' => 150f,
			'N' => 210f,
			'M' => 330f,
			'p' => 15f,
			'o' => 75f,
			'q' => 105f,
			'W' => 165f,
			'x' => 195f,
			'V' => 255f,
			'Y' => 285f,
			'A' => 345f,
			'!' => 999f,
			_ => null,
		};
		angle = value.GetValueOrDefault();
		return value.HasValue;
	}
}
