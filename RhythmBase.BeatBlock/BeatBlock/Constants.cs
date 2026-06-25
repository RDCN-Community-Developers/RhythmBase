using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock
{
	/// <summary>
	/// Contains constant values used by the BeatBlock level format.
	/// </summary>
	public static partial class Constants
	{ 
		public static partial float DefaultBpm => 100f;
		/// <summary>
		/// Represents the minimum supported version of the Rhythm Doctor application required for compatibility.
		/// </summary>
		public const int MinimumSupportedVersion = 14;

		/// <summary>
		/// The default Rhythm Doctor version used when no explicit target version is specified.
		/// </summary>
		public const int DefaultVersion = 18;
	}
}
