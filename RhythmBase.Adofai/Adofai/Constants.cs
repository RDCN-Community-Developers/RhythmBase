using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.Adofai;

/// <summary>
/// Provides constants and utility methods for A Dance of Fire and Ice level files.
/// </summary>
public static partial class Constants
{
	public static partial float DefaultBpm => 100f;
	/// <summary>
	/// Represents the minimum supported version of the A Dance of Fire and Ice (ADOFAI) game required by this application.
	/// </summary>
	public const int MinimumSupportedVersion = 15;

	/// <summary>
	/// The default Adofai format version assumed when creating or exporting content.
	/// </summary>
	public const int DefaultVersion = 15;
}
