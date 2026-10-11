namespace RhythmBase.RhythmDoctor.Components.Lang;

/// <summary>
/// The named fields that simulate the game's resolved level variables, mirroring the fields of
/// <c>LevelBase</c> that <c>GetVariable</c> / <c>EvalStringWithVariables</c> expose. They are read
/// and written by name through the variable protocol (for example <c>{bpm}</c> or
/// <c>p1Press</c>).
/// </summary>
public partial class ExpressionContext
{
	// Rhythm / timing.
	/// <summary>The current bar number.</summary>
	public int barNumber;
	/// <summary>The current beats per minute.</summary>
	public float bpm;
	/// <summary>The elapsed time since the previous frame.</summary>
	public float deltaTime;
	/// <summary>The level speed multiplier.</summary>
	public float levelSpeed;

	// Hit and mistake counters.
	/// <summary>The number of button presses counted so far.</summary>
	public int buttonPressCount;
	/// <summary>The number of misses needed to crack the heart.</summary>
	public float missesToCrackHeart;
	/// <summary>The number of early hits.</summary>
	public int numEarlyHits;
	/// <summary>The number of late hits.</summary>
	public int numLateHits;
	/// <summary>The number of misses.</summary>
	public int numMisses;
	/// <summary>The number of perfect hits.</summary>
	public int numPerfectHits;
	/// <summary>The total number of mistakes.</summary>
	public float numMistakes;
	/// <summary>The number of player 1 mistakes.</summary>
	public float numMistakesP1;
	/// <summary>The number of player 2 mistakes.</summary>
	public float numMistakesP2;

	// Visual.
	/// <summary>The shockwave distortion multiplier.</summary>
	public float shockwaveDistortionMultiplier;
	/// <summary>The shockwave duration multiplier.</summary>
	public float shockwaveDurationMultiplier;
	/// <summary>The shockwave size multiplier.</summary>
	public float shockwaveSizeMultiplier;
	/// <summary>The status sign width.</summary>
	public float statusSignWidth;

	// Level flags.
	/// <summary>Whether dialogues are active.</summary>
	public bool activeDialogues;
	/// <summary>Whether dialogues start immediately.</summary>
	public bool activeDialoguesImmediately;
	/// <summary>Whether the alternative matrix is enabled.</summary>
	public bool alternativeMatrix;
	/// <summary>Whether autoplay is enabled.</summary>
	public bool autoplay;
	/// <summary>Whether booleans default to true.</summary>
	public bool booleansDefaultToTrue;
	/// <summary>Whether characters are only shown on start.</summary>
	public bool charsOnlyOnStart;
	/// <summary>Whether player 2 is controlled by the CPU in two-player mode.</summary>
	public bool cpuIsP2On2P;
	/// <summary>Whether the row-change warning flashes are disabled.</summary>
	public bool disableRowChangeWarningFlashes;
	/// <summary>Whether the hands are hidden on start.</summary>
	public bool hideHandsOnStart;
	/// <summary>Whether the characters are invisible.</summary>
	public bool invisibleChars;
	/// <summary>Whether the heart is invisible.</summary>
	public bool invisibleHeart;
	/// <summary>Whether banana beats are disabled.</summary>
	public bool noBananaBeats;
	/// <summary>Whether the hands are hidden.</summary>
	public bool noHands;
	/// <summary>Whether the hit flash border is disabled.</summary>
	public bool noHitFlashBorder;
	/// <summary>Whether the hit strips are disabled.</summary>
	public bool noHitStrips;
	/// <summary>Whether oneshot shadows are disabled.</summary>
	public bool noOneshotShadows;
	/// <summary>Whether row animations on start are disabled.</summary>
	public bool noRowAnimsOnStart;
	/// <summary>Whether smart judgment is disabled.</summary>
	public bool noSmartJudgment;
	/// <summary>Whether the shake rotates.</summary>
	public bool rotateShake;
	/// <summary>Whether the row reflections jump.</summary>
	public bool rowReflectionsJumping;
	/// <summary>Whether the rank screen is skippable.</summary>
	public bool skippableRankScreen;
	/// <summary>Whether the rank text is skipped.</summary>
	public bool skipRankText;
	/// <summary>Whether the shake is smoothed.</summary>
	public bool smoothShake;
	/// <summary>Whether the flash font is used for floating text.</summary>
	public bool useFlashFontForFloatingText;
	/// <summary>Whether the lines are wobbly.</summary>
	public bool wobblyLines;

	// Input.
	/// <summary>Whether any player is currently pressing.</summary>
	public bool anyPlayerPress;
	/// <summary>Whether player 1 is holding the key.</summary>
	public bool p1IsPressed;
	/// <summary>Whether player 1 pressed this frame.</summary>
	public bool p1Press;
	/// <summary>Whether player 1 released this frame.</summary>
	public bool p1Release;
	/// <summary>Whether player 2 is holding the key.</summary>
	public bool p2IsPressed;
	/// <summary>Whether player 2 pressed this frame.</summary>
	public bool p2Press;
	/// <summary>Whether player 2 released this frame.</summary>
	public bool p2Release;
	/// <summary>Whether the left key was pressed.</summary>
	public bool leftPress;
	/// <summary>Whether the right key was pressed.</summary>
	public bool rightPress;
	/// <summary>Whether the up key was pressed.</summary>
	public bool upPress;
	/// <summary>Whether the down key was pressed.</summary>
	public bool downPress;
}
