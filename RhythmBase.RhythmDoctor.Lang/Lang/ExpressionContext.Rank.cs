using System;

namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The rank model mirrored from the game, used by <see cref="IRankSimulation"/>.
/// </summary>
public partial class ExpressionContext
{
	/// <summary>
	/// Enumeration representing different ranks.
	/// </summary>
	[Flags]
	public enum RDRank
	{
		/// <summary>S Rank.</summary>
		SMinus = 3 * 0,
		/// <summary>A Rank.</summary>
		AMinus = 3 * 1,
		/// <summary>B Rank.</summary>
		BMinus = 3 * 2,
		/// <summary>C Rank.</summary>
		CMinus = 3 * 3,
		/// <summary>D Rank.</summary>
		DMinus = 3 * 4,
		/// <summary>F Rank.</summary>
		FMinus = 3 * 5,

		/// <summary>S Minus Rank.</summary>
		S = SMinus + 1,
		/// <summary>A Minus Rank.</summary>
		A = AMinus + 1,
		/// <summary>B Minus Rank.</summary>
		B = BMinus + 1,
		/// <summary>C Minus Rank.</summary>
		C = CMinus + 1,
		/// <summary>D Minus Rank.</summary>
		D = DMinus + 1,
		/// <summary>F Minus Rank.</summary>
		F = FMinus + 1,

		/// <summary>S Plus Rank.</summary>
		SPlus = SMinus + 2,
		/// <summary>A Plus Rank.</summary>
		APlus = AMinus + 2,
		/// <summary>B Plus Rank.</summary>
		BPlus = BMinus + 2,
		/// <summary>C Plus Rank.</summary>
		CPlus = CMinus + 2,
		/// <summary>D Plus Rank.</summary>
		DPlus = DMinus + 2,
		/// <summary>F Plus Rank.</summary>
		FPlus = FMinus + 2,
	}
}
