using System.Reflection;
namespace RhythmBase.RhythmDoctor.Components;

/// <summary>
/// Variables.
/// </summary>
public sealed class Variables
{
	private static readonly Random _random = new();
	/// <summary>
	/// Enumeration representing different ranks.
	/// </summary>
	[Flags]
	public enum RDRank
	{
		/// <summary>
		/// S Rank.
		/// </summary>
		S = 1 << 2,
		/// <summary>
		/// A Rank.
		/// </summary>
		A = 2 << 2,
		/// <summary>
		/// B Rank.
		/// </summary>
		B = 3 << 2,
		/// <summary>
		/// C Rank.
		/// </summary>
		C = 4 << 2,
		/// <summary>
		/// D Rank.
		/// </summary>
		D = 5 << 2,
		/// <summary>
		/// F Rank.
		/// </summary>
		F = 6 << 2,

		/// <summary>
		/// Plus modifier for ranks.
		/// </summary>
		Plus = 0b01,
		/// <summary>
		/// Minus modifier for ranks.
		/// </summary>
		Minus = 0b10,

		/// <summary>
		/// S Plus Rank.
		/// </summary>
		SPlus = S | Plus,
		/// <summary>
		/// S Minus Rank.
		/// </summary>
		SMinus = S | Minus,
		/// <summary>
		/// A Plus Rank.
		/// </summary>
		APlus = A | Plus,
		/// <summary>
		/// A Minus Rank.
		/// </summary>
		AMinus = A | Minus,
		/// <summary>
		/// B Plus Rank.
		/// </summary>
		BPlus = B | Plus,
		/// <summary>
		/// B Minus Rank.
		/// </summary>
		BMinus = B | Minus,
		/// <summary>
		/// C Plus Rank.
		/// </summary>
		CPlus = C | Plus,
		/// <summary>
		/// C Minus Rank.
		/// </summary>
		CMinus = C | Minus,
		/// <summary>
		/// D Plus Rank.
		/// </summary>
		DPlus = D | Plus,
		/// <summary>
		/// D Minus Rank.
		/// </summary>
		DMinus = D | Minus,
		/// <summary>
		/// F Plus Rank.
		/// </summary>
		FPlus = F | Plus,
		/// <summary>
		/// F Minus Rank.
		/// </summary>
		FMinus = F | Minus,
	}
#pragma warning disable CS1591
#pragma warning disable IDE1006
	public Variables()
	{
		i = new int[10];
		f = new float[10];
		b = new bool[10];
	}
	public static int Rand(int value) =>
		(value > 0
			? _random.Next(1, value)
			: -_random.Next(1, -value)
		) + 1;
	public static bool atLeastRank(string rank) => rank switch
	{
		"S+" => SimulateCurrentRank <= RDRank.SPlus,
		"S" => SimulateCurrentRank <= RDRank.S,
		"A" => SimulateCurrentRank <= RDRank.A,
		"B" => SimulateCurrentRank <= RDRank.B,
		"C" => SimulateCurrentRank <= RDRank.C,
		"D" => SimulateCurrentRank <= RDRank.D,
		"F" => SimulateCurrentRank <= RDRank.F,
		_ => throw new ArgumentException("Invalid rank"),
	};
	public static bool atLeastNPerfects(int hitsToCheck, int numberOfPerfects) => numberOfPerfects / (float)hitsToCheck > SimulateAtLeastNPerfectsSuccessRate;
	public object? this[string variableName]
	{
		get => variableName switch
		{
			['i', char ii] => i[ii - '0'],
			['f', char fi] => f[fi - '0'],
			['b', char bi] => b[bi - '0'],
			_ => GetType().GetField(variableName)?.GetValue(this),
		};
		set
		{
			switch (variableName)
			{
				case ['i', char ii]:
					i[ii - '0'] =
						value is int v1 ? v1 :
						value is float v1f ? (int)float.Round(v1f) :
						throw new ArgumentException("Value is not an integer.");
					break;
				case ['f', char fi]:
					f[fi - '0'] =
						value is float v2 ? v2 :
						throw new ArgumentException("Value is not a float.");
					break;
				case ['b', char bi]:
					b[bi - '0'] =
						value is bool v3 ? v3 :
						value is float v3f ? v3f != 0 :
						throw new ArgumentException("Value is not a boolean.");
					break;
				default:
					FieldInfo? field = GetType().GetField(variableName);
					field?.SetValue(this, value);
					break;
			}
		}
	}
	public static float IIf(bool condition, float trueResult, float falseResult) => condition ? trueResult : falseResult;
	/// <summary>
	/// Integer variables.
	/// </summary>
	public readonly int[] i;
	/// <summary>
	/// Float variables.
	/// </summary>
	public readonly float[] f;
	/// <summary>
	/// Boolean variables.
	/// </summary>
	public readonly bool[] b;
	public int barNumber;
	public int buttonPressCount;
	public int missesToCrackHeart;
	public int numEarlyHits;
	public int numLateHits;
	public int numMisses;
	public int numPerfectHits;
	public float bpm;
	public float deltaTime;
	public float levelSpeed;
	public float numMistakes;
	public float numMistakesP1;
	public float numMistakesP2;
	public float shockwaveDistortionMultiplier;
	public float shockwaveDurationMultiplier;
	public float shockwaveSizeMultiplier;
	public float statusSignWidth;
	public bool activeDialogues;
	public bool activeDialoguesImmediately;
	public bool alternativeMatrix;
	public bool anyPlayerPress;
	public bool autoplay;
	public bool booleansDefaultToTrue;
	public bool charsOnlyOnStart;
	public bool cpuIsP2On2P;
	public bool disableRowChangeWarningFlashes;
	public bool downPress;
	public bool hideHandsOnStart;
	public bool invisibleChars;
	public bool invisibleHeart;
	public bool leftPress;
	public bool noBananaBeats;
	public bool noHands;
	public bool noHitFlashBorder;
	public bool noHitStrips;
	public bool noOneshotShadows;
	public bool noRowAnimsOnStart;
	public bool noSmartJudgment;
	public bool p1IsPressed;
	public bool p1Press;
	public bool p1Release;
	public bool p2IsPressed;
	public bool p2Press;
	public bool p2Release;
	public bool rightPress;
	public bool rotateShake;
	public bool rowReflectionsJumping;
	public bool skippableRankScreen;
	public bool skipRankText;
	public bool smoothShake;
	public bool upPress;
	public bool useFlashFontForFloatingText;
	public bool wobblyLines;
	public static float SimulateAtLeastNPerfectsSuccessRate { get; set; } = 0.9f;
	public static RDRank SimulateCurrentRank { get; set; } = RDRank.S;
}
