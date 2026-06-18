namespace RhythmBase.BeatBlock;

/// <summary>
/// Represents the types of events available in the BeatBlock editor.
/// </summary>
[JsonEnumSerializable(false)]
public enum EventType
{
    /// <summary>
    /// Unknown event type, used when the event type cannot be determined from the Lua file.
    /// </summary>
    Unknown,
    /// <summary>
    /// _TEMPLATE
    /// </summary>
    /// <remarks>
    /// No description
    /// </remarks>
    Template,
    /// <summary>
    /// Forward event, used for unrecognized event types.
    /// </summary>
    ForwardEvent,
    /// <summary>
    /// Bookmark
    /// </summary>
    /// <remarks>
    /// Marks a section of the chart
    /// </remarks>
    Bookmark,
    /// <summary>
    /// Comment
    /// </summary>
    /// <remarks>
    /// Has a textbox to put text in, but does nothing to the level itself.
    /// </remarks>
    Comment,
    /// <summary>
    /// Run tag
    /// </summary>
    /// <remarks>
    /// Runs a tag, which is a collection of events
    /// </remarks>
    Tag,
    /// <summary>
    /// Edit Paddles
    /// </summary>
    /// <remarks>
    /// Change paddle properties
    /// </remarks>
    Paddles,
    /// <summary>
    /// Play song
    /// </summary>
    /// <remarks>
    /// Plays a song
    /// </remarks>
    Play,
    /// <summary>
    /// Retime
    /// </summary>
    /// <remarks>
    /// Skip a certain amount of time during playback
    /// </remarks>
    Retime,
    /// <summary>
    /// Set Bounce Height
    /// </summary>
    /// <remarks>
    /// Set how high bounces bounce based on delay
    /// </remarks>
    SetBounceHeight,
    /// <summary>
    /// Set BPM
    /// </summary>
    /// <remarks>
    /// Change the BPM
    /// </remarks>
    [JsonAlias("setBPM")]
    SetBeatsPerMinute,
    /// <summary>
    /// Show Results
    /// </summary>
    /// <remarks>
    /// Shows the results screen, ending the level
    /// </remarks>
    ShowResults,
    /// <summary>
    /// Block
    /// </summary>
    /// <remarks>
    /// Basic note
    /// </remarks>
    Block,
    /// <summary>
    /// Bounce
    /// </summary>
    /// <remarks>
    /// No description
    /// </remarks>
    Bounce,
    /// <summary>
    /// Extra Tap
    /// </summary>
    /// <remarks>
    /// A tap independent of other notes
    /// </remarks>
    ExtraTap,
    /// <summary>
    /// Hold
    /// </summary>
    /// <remarks>
    /// No description
    /// </remarks>
    Hold,
    /// <summary>
    /// Inverse Block
    /// </summary>
    /// <remarks>
    /// Similar to a basic note, but must be hit with the back of the paddle
    /// </remarks>
    Inverse,
    /// <summary>
    /// Mine
    /// </summary>
    /// <remarks>
    /// Similar to a basic note, but must NOT be hit.
    /// </remarks>
    Mine,
    /// <summary>
    /// MineHold
    /// </summary>
    /// <remarks>
    /// No description
    /// </remarks>
    MineHold,
    /// <summary>
    /// Side
    /// </summary>
    /// <remarks>
    /// A note that must be hit from the side
    /// </remarks>
    Side,
    /// <summary>
    /// Trace
    /// </summary>
    /// <remarks>
    /// No description
    /// </remarks>
    Trace,
    /// <summary>
    /// Advance Text Decoration
    /// </summary>
    /// <remarks>
    /// Adds a new syllable to a targeted TextDeco object
    /// </remarks>
    [JsonAlias("advanceTextDeco")]
    AdvanceTextDecoration,
    /// <summary>
    /// AFT
    /// </summary>
    /// <remarks>
    /// Freeze frames, feedback loops, and more!
    /// </remarks>
    Aft,
    /// <summary>
    /// Decoration
    /// </summary>
    /// <remarks>
    /// Draws a sprite on the screen, and allows updating its properties over time
    /// </remarks>
    [JsonAlias("deco")]
    Decoration,
    /// <summary>
    /// Deco Shader
    /// </summary>
    /// <remarks>
    /// Create a new shader that can be applied to decorations
    /// </remarks>
    [JsonAlias("decoShader")]
    DecorationShader,
    /// <summary>
    /// Ease
    /// </summary>
    /// <remarks>
    /// Eases a variable over time
    /// </remarks>
    Ease,
    /// <summary>
    /// Ease Sequence
    /// </summary>
    /// <remarks>
    /// Define a new ease sequence
    /// </remarks>
    EaseSequence,
    /// <summary>
    /// Force Player Sprite
    /// </summary>
    /// <remarks>
    /// Forces the player to use a specific sprite
    /// </remarks>
    ForcePlayerSprite,
    /// <summary>
    /// Hall of Mirrors
    /// </summary>
    /// <remarks>
    /// When enabled, the screen will no longer be cleared between frames, creating a "hall of mirrors" effect
    /// </remarks>
    HallOfMirrors,
    /// <summary>
    /// Load Custom Font
    /// </summary>
    /// <remarks>
    /// Loads a custom font file from your custom level folder
    /// </remarks>
    LoadCustomFont,
    /// <summary>
    /// Noise
    /// </summary>
    /// <remarks>
    /// Enables a noise effect on the background
    /// </remarks>
    Noise,
    /// <summary>
    /// Outline
    /// </summary>
    /// <remarks>
    /// Adds an outline to objects
    /// </remarks>
    Outline,
    /// <summary>
    /// Play Sound
    /// </summary>
    /// <remarks>
    /// Plays a sound effect
    /// </remarks>
    PlaySound,
    /// <summary>
    /// Recolor notes
    /// </summary>
    /// <remarks>
    /// Sets note colors
    /// </remarks>
    RecolorNotes,
    /// <summary>
    /// Set BG color
    /// </summary>
    /// <remarks>
    /// Sets the background color channel
    /// </remarks>
    [JsonAlias("setBgColor")]
    SetBackgroundColor,
    /// <summary>
    /// Set Boolean
    /// </summary>
    /// <remarks>
    /// Sets a boolean variable
    /// </remarks>
    SetBoolean,
    /// <summary>
    /// Set color
    /// </summary>
    /// <remarks>
    /// Changes a color in the palette
    /// </remarks>
    SetColor,
    /// <summary>
    /// Set joystick color
    /// </summary>
    /// <remarks>
    /// Sets the Joystick LED to a color channel
    /// </remarks>
    SetJoystickColor,   
    /// <summary>
    /// Shader Uniform
    /// </summary>
    /// <remarks>
    /// Eases a shader uniform over time
    /// </remarks>
    ShaderUniform,
    /// <summary>
    /// Song Name Override
    /// </summary>
    /// <remarks>
    /// Override the song name shown in the corner of the screen. leave empty to use the default song name.
    /// </remarks>
    SongNameOverride,
    /// <summary>
    /// Stamp
    /// </summary>
    /// <remarks>
    /// Stamps an image onto an offscreen canvas, which can then be called as a deco.
    /// </remarks>
    Stamp,
    /// <summary>
    /// Text Decoration
    /// </summary>
    /// <remarks>
    /// Draws a string on the screen, and allows updating its properties over time
    /// </remarks>
    [JsonAlias("textdeco")]
    TextDecoration,
    /// <summary>
    /// Toggle Particles
    /// </summary>
    /// <remarks>
    /// Toggle whether various particle effects are spawned in by notes
    /// </remarks>
    ToggleParticles,
}
/// <summary>
/// Specifies the rendering layer for decorations and text decorations.
/// </summary>
[JsonEnumSerializable(false)]
public enum Layer
{
    /// <summary>
    /// Background layer.
    /// </summary>
    Bg,
}
/// <summary>
/// Specifies the effect canvas type for shader backgrounds, including an option to disable it.
/// </summary>
[JsonEnumSerializable(false)]
public enum EffectCanvasTypeWdisable
{
    /// <summary>
    /// Disables the effect canvas.
    /// </summary>
    Disable,
    /// <summary>
    /// Applies a halftone effect.
    /// </summary>
    Halftone,
}
/// <summary>
/// Specifies which directional indicators are shown for side notes.
/// </summary>
[JsonEnumSerializable(false)]
public enum SideIndicatorTypes
{
    /// <summary>
    /// Show indicators on both sides.
    /// </summary>
    Both,
}
/// <summary>
/// Specifies the source location of a shader.
/// </summary>
[JsonEnumSerializable(false)]
public enum ShaderSource
{
    /// <summary>
    /// The shader is loaded from an external file.
    /// </summary>
    File,
}
/// <summary>
/// Specifies which note types should be recolored.
/// </summary>
[JsonEnumSerializable(false)]
public enum NoteRecolorTarget
{
    /// <summary>
    /// Recolor all note types.
    /// </summary>
    All,
}
/// <summary>
/// Specifies the horizontal alignment of text in a text decoration.
/// </summary>
[JsonEnumSerializable(false)]
public enum TextJustification
{
    /// <summary>
    /// No justification.
    /// </summary>
    None,
    /// <summary>
    /// Align text to the left.
    /// </summary>
    Left,
    /// <summary>
    /// Center align text.
    /// </summary>
    Center,
    /// <summary>
    /// Align text to the right.
    /// </summary>
    Right,
}
/// <summary>
/// Specifies how a decoration is mirrored.
/// </summary>
[JsonEnumSerializable(false)]
public enum MirrorType
{
    /// <summary>
    /// No mirroring is applied.
    /// </summary>
    None,
}
//[RDJsonEnumSerializable]
//public enum TileRepeatMode
//{
//    None,
//}
/// <summary>
/// Specifies the effect canvas type used when drawing decorations.
/// </summary>
[JsonEnumSerializable(false)]
public enum EffectCanvasType
{
    /// <summary>
    /// No effect canvas is used.
    /// </summary>
    None,
    /// <summary>
    /// Apply a recolor effect on the canvas.
    /// </summary>
    Recolor,
}