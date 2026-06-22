using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor;
/// <summary>
/// Rhythm Doctor event types.
/// </summary>
[JsonEnumSerializable]
public enum EventType
{
	/// <summary>
	/// Add a classic beat.
	/// </summary>
	AddClassicBeat,
	/// <summary>
	/// Add a free time beat.
	/// </summary>
	AddFreeTimeBeat,
	/// <summary>
	/// Add a oneshot beat.
	/// </summary>
	AddOneshotBeat,
	/// <summary>
	/// Advance the text.
	/// </summary>
	AdvanceText,
	/// <summary>
	/// Drop the bass.
	/// </summary>
	BassDrop,
	/// <summary>  
	/// Blend effect.  
	/// </summary>  
	Blend,
	/// <summary>
	/// Call a custom method.
	/// </summary>
	CallCustomMethod,
	/// <summary>  
	/// Change the character.  
	/// </summary>  
	ChangeCharacter,
	/// <summary>
	/// Change the players' rows.
	/// </summary>
	ChangePlayersRows,
	/// <summary>
	/// Add a comment.
	/// </summary>
	Comment,
	/// <summary>
	/// Custom flash event.
	/// </summary>
	CustomFlash,
	/// <summary>
	/// Set the desktop color.
	/// </summary>
	DesktopColor,
	/// <summary>
	/// Fade the room.
	/// </summary>
	FadeRoom,
	/// <summary>
	/// Finish the level.
	/// </summary>
	FinishLevel,
	/// <summary>
	/// Flash the screen.
	/// </summary>
	Flash,
	/// <summary>
	/// Flip the screen.
	/// </summary>
	FlipScreen,
	/// <summary>
	/// Display floating text.
	/// </summary>
	FloatingText,
	/// <summary>
	/// Represents a decoration event that is not natively recognized by the engine,
	/// used to preserve unknown or user-defined decoration event data during serialization and deserialization.
	/// </summary>
	ForwardDecorationEvent,
	/// <summary>
	/// Represents a general event that is not natively recognized by the engine,
	/// used to preserve unknown or user-defined event data during serialization and deserialization.
	/// </summary>
	ForwardEvent,
	/// <summary>
	/// Represents a row event that is not natively recognized by the engine,
	/// used to preserve unknown or user-defined row event data during serialization and deserialization.
	/// </summary>
	ForwardRowEvent,
	/// <summary>
	/// Hide the row.
	/// </summary>
	HideRow,
	/// <summary>
	/// Hides the window.
	/// </summary>
	HideWindow,
	/// <summary>
	/// Invert the colors.
	/// </summary>
	InvertColors,
	/// <summary>
	/// Mask the room.
	/// </summary>
	MaskRoom,
	/// <summary>
	/// Move an object.
	/// </summary>
	Move,
	/// <summary>
	/// Move the camera.
	/// </summary>
	MoveCamera,
	/// <summary>
	/// Move the room.
	/// </summary>
	MoveRoom,
	/// <summary>
	/// Move the row.
	/// </summary>
	MoveRow,
	/// <summary>
	/// Narrate row information.
	/// </summary>
	NarrateRowInfo,
	/// <summary>
	/// Start a new window dance.
	/// </summary>
	NewWindowDance,
	/// <summary>
	/// Paint the hands.
	/// </summary>
	PaintHands,
	/// <summary>
	/// Play an animation.
	/// </summary>
	PlayAnimation,
	/// <summary>
	/// Play an expression.
	/// </summary>
	PlayExpression,
	/// <summary>
	/// Play a song.
	/// </summary>
	PlaySong,
	/// <summary>
	/// Play a sound.
	/// </summary>
	PlaySound,
	/// <summary>
	/// Pulse the camera.
	/// </summary>
	PulseCamera,
	/// <summary>
	/// Pulse a free time beat.
	/// </summary>
	PulseFreeTimeBeat,
	/// <summary>
	/// Read the narration.
	/// </summary>
	ReadNarration,
	/// <summary>
	/// Rename the window.
	/// </summary>
	RenameWindow,
	/// <summary>
	/// Reorder the rooms.
	/// </summary>
	ReorderRooms,
	/// <summary>  
	/// Reorder the rows.  
	/// </summary>  
	ReorderRow,
	/// <summary>  
	/// Reorder the sprites.  
	/// </summary>  
	ReorderSprite,
	/// <summary>
	/// Reorder the windows.
	/// </summary>
	ReorderWindows,
	/// <summary>
	/// Say "Ready, Get Set, Go".
	/// </summary>
	SayReadyGetSetGo,
	/// <summary>
	/// Set the background color.
	/// </summary>
	SetBackgroundColor,
	/// <summary>
	/// Set the beat sound.
	/// </summary>
	SetBeatSound,
	/// <summary>
	/// Set the beats per minute.
	/// </summary>
	SetBeatsPerMinute,
	/// <summary>
	/// Set the clap sounds.
	/// </summary>
	SetClapSounds,
	/// <summary>
	/// Set the counting sound.
	/// </summary>
	SetCountingSound,
	/// <summary>
	/// Set the crotchets per bar.
	/// </summary>
	SetCrotchetsPerBar,
	/// <summary>
	/// Set the foreground.
	/// </summary>
	SetForeground,
	/// <summary>
	/// Set the game sound.
	/// </summary>
	SetGameSound,
	/// <summary>
	/// Set the hand owner.
	/// </summary>
	SetHandOwner,
	/// <summary>
	/// Set the heart explode interval.
	/// </summary>
	SetHeartExplodeInterval,
	/// <summary>
	/// Set the heart explode volume.
	/// </summary>
	SetHeartExplodeVolume,
	/// <summary>
	/// Sets the game's main window.
	/// </summary>
	SetMainWindow,
	/// <summary>
	/// Set the oneshot wave.
	/// </summary>
	SetOneshotWave,
	/// <summary>
	/// Set the play style.
	/// </summary>
	SetPlayStyle,
	/// <summary>
	/// Set the room content mode.
	/// </summary>
	SetRoomContentMode,
	/// <summary>
	/// Set the room perspective.
	/// </summary>
	SetRoomPerspective,
	/// <summary>
	/// Set the row X positions.
	/// </summary>
	SetRowXs,
	/// <summary>
	/// Set the speed.
	/// </summary>
	SetSpeed,
	/// <summary>
	/// Set the theme.
	/// </summary>
	SetTheme,
	/// <summary>
	/// Set the VFX preset.
	/// </summary>
	SetVFXPreset,
	/// <summary>
	/// Set the visibility.
	/// </summary>
	SetVisible,
	/// <summary>  
	/// Sets the content of the window.  
	/// </summary>  
	SetWindowContent,
	/// <summary>
	/// Shake the screen.
	/// </summary>
	ShakeScreen,
	/// <summary>
	/// Shake the screen, the custom version.
	/// </summary>
	ShakeScreenCustom,
	/// <summary>
	/// Show the dialogue.
	/// </summary>
	ShowDialogue,
	/// <summary>
	/// Show the hands.
	/// </summary>
	ShowHands,
	/// <summary>
	/// Show the rooms.
	/// </summary>
	ShowRooms,
	/// <summary>
	/// Show the status sign.
	/// </summary>
	ShowStatusSign,
	/// <summary>
	/// Displays the rows of subdivisions.
	/// </summary>
	ShowSubdivisionsRows,
	/// <summary>
	/// Spin the rows.
	/// </summary>
	SpinningRows,
	/// <summary>
	/// Stutter effect.
	/// </summary>
	Stutter,
	/// <summary>
	/// Tag an action.
	/// </summary>
	TagAction,
	/// <summary>
	/// Text explosion effect.
	/// </summary>
	TextExplosion,
	/// <summary>
	/// Tile effect.
	/// </summary>
	Tile,
	/// <summary>
	/// Tint effect.
	/// </summary>
	Tint,
	/// <summary>
	/// Tint rows effect.
	/// </summary>
	TintRows,
	/// <summary>  
	/// Resize the game window.  
	/// </summary>  
	WindowResize,
}

/// <summary>
/// Defines the classic beat patterns.
/// </summary>
[JsonEnumSerializable]
public enum ClassicBeatPattern
{
	/// <summary>
	/// No change in the beat pattern.
	/// </summary>
	NoChange,
	/// <summary>
	/// Three beat pattern.
	/// </summary>
	ThreeBeat,
	/// <summary>
	/// Four beat pattern.
	/// </summary>
	FourBeat
}/// <summary>
 /// Represents the freeze burn mode.
 /// </summary>
[JsonEnumSerializable]
public enum OneshotType
{
	/// <summary>
	/// A wave freeze burn mode.
	/// </summary>
	Wave,
	/// <summary>
	/// A freeze shot mode.
	/// </summary>
	Freezeshot,
	/// <summary>
	/// A burn shot mode.
	/// </summary>
	Burnshot
}
/// <summary>
/// Represents the type of pulse.
/// </summary>
[JsonEnumSerializable]
public enum OneshotPulseShapeType
{
	/// <summary>
	/// A wave pulse.
	/// </summary>
	Wave,
	/// <summary>
	/// A square pulse.
	/// </summary>
	Square,
	/// <summary>
	/// A triangle pulse.
	/// </summary>
	Triangle,
	/// <summary>
	/// A heart-shaped pulse.
	/// </summary>
	Heart
}
/// <summary>
/// Specifies when a hold cue should be triggered for a oneshot beat.
/// </summary>
[JsonEnumSerializable]
public enum HoldCue
{
	/// <summary>
	/// Let the system select the most appropriate cue timing automatically.
	/// </summary>
	Auto,
	/// <summary>
	/// Force the hold cue to trigger earlier than the default timing.
	/// </summary>
	Early,
	/// <summary>
	/// Force the hold cue to trigger later than the default timing.
	/// </summary>
	Late,
}
/// <summary>  
/// Defines the strength levels for the BassDrop event.  
/// </summary>  
[JsonEnumSerializable]
public enum StrengthType
{
	/// <summary>  
	/// Low strength.  
	/// </summary>  
	Low,
	/// <summary>  
	/// Medium strength.  
	/// </summary>  
	Medium,
	/// <summary>  
	/// High strength.  
	/// </summary>  
	High
}
/// <summary>  
/// Specifies the different types of blend effects available.  
/// </summary>  
[JsonEnumSerializable]
public enum BlendType
{
	/// <summary>  
	/// No blend effect.  
	/// </summary>  
	None,
	/// <summary>
	/// Additive blend effect.
	/// </summary>
	Additive,
	/// <summary>
	/// Multiply blend effect.
	/// </summary>
	Multiply,
	/// <summary>
	/// Invert blend effect.
	/// </summary>
	Invert,
}
/// <summary>
/// Specifies the types of borders that can be applied.
/// </summary>
[JsonEnumSerializable]
public enum Border
{
	/// <summary>
	/// No border.
	/// </summary>
	None,
	/// <summary>
	/// An outline border.
	/// </summary>
	Outline,
	/// <summary>
	/// A glowing border.
	/// </summary>
	Glow
}
/// <summary>
/// Specifies the execution time options for the method.
/// </summary>
[JsonEnumSerializable]
public enum EventExecutionTimeOption
{
	/// <summary>
	/// Execute the method on prebar.
	/// </summary>
	OnPrebar,
	/// <summary>
	/// Execute the method on bar.
	/// </summary>
	OnBar
}
/// <summary>
/// Represents the modes of players.
/// </summary>
[JsonEnumSerializable]
public enum PlayingMode
{
	/// <summary>
	/// Single player mode.
	/// </summary>
	OnePlayer,
	/// <summary>
	/// Two players mode.
	/// </summary>
	TwoPlayers,
	/// <summary>
	/// Single player mode or two players mode.
	/// </summary>
	OneOrTwoPlayers,
}
/// <summary>
/// Specifies the different modes for content display.
/// </summary>
[JsonEnumSerializable]
public enum ContentMode
{
	/// <summary>
	/// Scales the content to fill the available space.
	/// </summary>
	ScaleToFill,
	/// <summary>
	/// Scales the content to fit within the available space while maintaining the aspect ratio.
	/// </summary>
	AspectFit,
	/// <summary>
	/// Scales the content to fill the available space while maintaining the aspect ratio.
	/// </summary>
	AspectFill,
	/// <summary>
	/// Centers the content within the available space without scaling.
	/// </summary>
	Center,
	/// <summary>
	/// Tiles the content to fill the available space.
	/// </summary>
	Tiled,
	/// <summary>
	/// 
	/// </summary>
	Real,
}
/// <summary>
/// Enum representing default audio events in the RhythmBase application.
/// </summary>
[JsonEnumSerializable]
public enum DefaultAudio
{
	/// <summary>
	/// Base sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_Base,
	/// <summary>
	/// Rest sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_Rest,
	/// <summary>
	/// Amen fill sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_AmenFill,
	/// <summary>
	/// First freeze sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_Freeze1,
	/// <summary>
	/// Second freeze sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_Freeze2,
	/// <summary>
	/// CPU freeze sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_FreezeCPU,
	/// <summary>
	/// First burn sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_Burn1,
	/// <summary>
	/// Second burn sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_Burn2,
	/// <summary>
	/// CPU burn sound for the tutorial house.
	/// </summary>
	sndTutorialHouse_BurnCPU
}
/// <summary>
/// Specifies the possible durations for a flash event.
/// </summary>
[JsonEnumSerializable]
public enum DurationType
{
	/// <summary>
	/// A short duration.
	/// </summary>
	Short = 1,
	/// <summary>
	/// A medium duration.
	/// </summary>
	Medium = 2,
	/// <summary>
	/// A long duration.
	/// </summary>
	Long = 4,
}
/// <summary>  
/// Specifies the category of the narration.  
/// </summary>  
[JsonEnumSerializable]
public enum NarrationCategory
{
	/// <summary>  
	/// Fallback category, used as a default when no other category applies.  
	/// </summary>
	Fallback,
	/// <summary>  
	/// Navigation category, used for guiding the user through the interface or level.  
	/// </summary>  
	Navigation,
	/// <summary>  
	/// Instruction category, used for providing instructions or tutorials.  
	/// </summary>  
	Instruction,
	/// <summary>  
	/// Notification category, used for alerts or notifications.  
	/// </summary>  
	Notification,
	/// <summary>  
	/// Dialogue category, used for character or story dialogues.  
	/// </summary>  
	Dialogue,
	/// <summary>  
	/// Description category, used for descriptive text or explanations.  
	/// </summary>  
	Description,
	/// <summary>  
	/// Subtitles category, used for displaying subtitles.  
	/// </summary>  
	Subtitles,
}
/// <summary>
/// Specifies the mode of the text.
/// </summary>
[Flags]
[JsonEnumSerializable]
public enum FloatingTextFadeOutMode
{
	/// <summary>
	/// The text will fade out gradually.
	/// </summary>
	FadeOut = 0,
	/// <summary>
	/// The text will hide abruptly.
	/// </summary>
	HideAbruptly = 1
}
/// <summary>
/// Specifies the anchor style of the text.
/// </summary>
[Flags]
public enum FloatingTextAnchorStyle
{
	/// <summary>
	/// The lower anchor style.
	/// </summary>
	Lower = 1,
	/// <summary>
	/// The upper anchor style.
	/// </summary>
	Upper = 2,
	/// <summary>
	/// The left anchor style.
	/// </summary>
	Left = 4,
	/// <summary>
	/// The right anchor style.
	/// </summary>
	Right = 8,
	/// <summary>
	/// The center anchor style.
	/// </summary>
	Center = 0
}
/// <summary>
/// Defines the possible transition types for hiding the row.
/// </summary>
[JsonEnumSerializable]
public enum Transition
{
	/// <summary>
	/// Smooth transition.
	/// </summary>
	Smooth,
	/// <summary>
	/// Instant transition.
	/// </summary>
	Instant,
	/// <summary>
	/// Full transition.
	/// </summary>
	Full,
	/// <summary>
	/// Represents a placeholder or default value indicating the absence of a specific option or selection.
	/// </summary>
	None,
}
/// <summary>
/// Defines the possible visibility states of the row.
/// </summary>
[JsonEnumSerializable]
public enum ShowTargetType
{
	/// <summary>
	/// Row is visible.
	/// </summary>
	Visible,
	/// <summary>
	/// Row is hidden.
	/// </summary>
	Hidden,
	/// <summary>
	/// Only the character is visible.
	/// </summary>
	OnlyCharacter,
	/// <summary>
	/// Only the row is visible.
	/// </summary>
	OnlyRow
}
/// <summary>
/// Defines the types of masks available.
/// </summary>
[JsonEnumSerializable]
public enum RoomMaskType
{
	/// <summary>
	/// Uses an image as the mask.
	/// </summary>
	Image,
	/// <summary>
	/// Uses a room as the mask.
	/// </summary>
	Room,
	/// <summary>
	/// Uses a color as the mask.
	/// </summary>
	Color,
	/// <summary>
	/// No mask is applied.
	/// </summary>
	None
}
/// <summary>
/// Defines the alpha modes available.
/// </summary>
[JsonEnumSerializable]
public enum MaskAlphaMode
{
	/// <summary>
	/// Normal alpha mode.
	/// </summary>
	Normal,
	/// <summary>
	/// Inverted alpha mode.
	/// </summary>
	Inverted
}
/// <summary>
/// Specifies the targets for the move row event.
/// </summary>
[JsonEnumSerializable]
public enum MoveRowTarget
{
	/// <summary>
	/// Target the whole row.
	/// </summary>
	WholeRow,
	/// <summary>
	/// Target the heart.
	/// </summary>
	Heart,
	/// <summary>
	/// Target the character.
	/// </summary>
	Character
}
/// <summary>
/// Specifies the type of narration information.
/// </summary>
[JsonEnumSerializable]
public enum NarrateInfoType
{
	/// <summary>
	/// Indicates a connection event.
	/// </summary>
	Connect,
	/// <summary>
	/// Indicates an update event.
	/// </summary>
	Update,
	/// <summary>
	/// Indicates a disconnection event.
	/// </summary>
	Disconnect,
	/// <summary>
	/// Indicates an online event.
	/// </summary>
	Online,
	/// <summary>
	/// Indicates an offline event.
	/// </summary>
	Offline
}
/// <summary>
/// Specifies the beats to skip during narration.
/// </summary>
[JsonEnumSerializable]
public enum NarrateSkipBeat
{
	/// <summary>
	/// Skip beats is on.
	/// </summary>
	On,
	/// <summary>
	/// Custom skip beats.
	/// </summary>
	Custom,
	/// <summary>
	/// Skip beats is off.
	/// </summary>
	Off,
}
/// <summary>
/// Represents the presets.
/// </summary>
[JsonEnumSerializable]
public enum WindowDancePreset
{
	/// <summary>
	/// Move preset.
	/// </summary>
	Move,
	/// <summary>
	/// Sway preset.
	/// </summary>
	Sway,
	/// <summary>
	/// Wrap preset.
	/// </summary>
	Wrap,
	/// <summary>
	/// Ellipse preset.
	/// </summary>
	Ellipse,
	/// <summary>
	/// Shake per preset.
	/// </summary>
	ShakePer
}
/// <summary>
/// Represents the same preset behaviors.
/// </summary>
[JsonEnumSerializable]
public enum SamePresetBehavior
{
	/// <summary>
	/// Reset behavior.
	/// </summary>
	Reset,
	/// <summary>
	/// Keep behavior.
	/// </summary>
	Keep
}
/// <summary>
/// Represents the references.
/// </summary>
[JsonEnumSerializable]
public enum WindowDanceReference
{
	/// <summary>
	/// Center reference.
	/// </summary>
	Center,
	/// <summary>
	/// Edge reference.
	/// </summary>
	Edge
}
/// <summary>
/// Represents the ease types.
/// </summary>
[JsonEnumSerializable]
public enum WindowDanceEaseType
{
	/// <summary>
	/// Repeat ease type.
	/// </summary>
	Repeat,
	/// <summary>
	/// Mirror ease type.
	/// </summary>
	Mirror,
}
/// <summary>  
/// Enum representing different rhythm patterns.  
/// </summary>  
public enum Pattern
{
	/// <summary>  
	/// No pattern.  
	/// </summary>  
	None,
	/// <summary>  
	/// Pattern X.  
	/// </summary>  
	X,
	/// <summary>  
	/// Pattern Up.  
	/// </summary>  
	Up,
	/// <summary>  
	/// Pattern Down.  
	/// </summary>  
	Down,
	/// <summary>  
	/// Pattern Banana.  
	/// </summary>  
	Banana,
	/// <summary>  
	/// Pattern Return.  
	/// </summary>  
	ReturnBanana
}
/// <summary>
/// Represents the hands of a player.
/// </summary>
[JsonEnumSerializable]
public enum PlayerHand
{
	/// <summary>
	/// The left hand of the player.
	/// </summary>
	Left,
	/// <summary>
	/// The right hand of the player.
	/// </summary>
	Right,
	/// <summary>
	/// Both hands of the player.
	/// </summary>
	Both,
	/// <summary>
	/// Player 1's hand.
	/// </summary>
	p1,
	/// <summary>
	/// Player 2's hand.
	/// </summary>
	p2
}
/// <summary>
/// Represents the type of player in the game.
/// </summary>
[JsonEnumSerializable]
public enum PlayerType
{
	/// <summary>  
	/// Automatically detect the player.  
	/// </summary>  
	AutoDetect,
	/// <summary>
	/// Player 1.
	/// </summary>
	P1,
	/// <summary>
	/// Player 2.
	/// </summary>
	P2,
	/// <summary>
	/// Computer player.
	/// </summary>
	CPU,
	/// <summary>
	/// Both.
	/// </summary>
	BOTH,
	/// <summary>
	/// No change in player type.
	/// </summary>
	NoChange,
}
/// <summary>
/// Defines the types of custom sounds.
/// </summary>
[JsonEnumSerializable]
public enum CustomSoundType
{
	/// <summary>
	/// Cue sound type.
	/// </summary>
	CueSound,
	/// <summary>
	/// Music sound type.
	/// </summary>
	MusicSound,
	/// <summary>
	/// Beat sound type.
	/// </summary>
	BeatSound,
	/// <summary>
	/// Hit sound type.
	/// </summary>
	HitSound,
	/// <summary>
	/// Other sound type.
	/// </summary>
	OtherSound
}
/// <summary>
/// Defines the action types for the pulse free time beat.
/// </summary>
[JsonEnumSerializable]
public enum PulseAction
{
	/// <summary>
	/// Increment action.
	/// </summary>
	Increment,
	/// <summary>
	/// Decrement action.
	/// </summary>
	Decrement,
	/// <summary>
	/// Custom action.
	/// </summary>
	Custom,
	/// <summary>
	/// Remove action.
	/// </summary>
	Remove
}
/// <summary>
/// Indicates how the window name should be modified by a <see cref="RenameWindow"/> event.
/// </summary>
[JsonEnumSerializable]
public enum WindowNameAction
{
	/// <summary>
	/// Replace the current window name with the provided <see cref="RenameWindow.Text"/>.
	/// </summary>
	Set,

	/// <summary>
	/// Append the provided <see cref="RenameWindow.Text"/> to the existing window name.
	/// </summary>
	Append,

	/// <summary>
	/// Reset the window name to its default value. Any provided <see cref="RenameWindow.Text"/>
	/// is ignored when this action is used.
	/// </summary>
	Reset,
}
/// <summary>
/// Specifies the type of row in the rhythm base.
/// </summary>
[JsonEnumSerializable]
public enum RowType
{
	/// <summary>
	/// Represents a classic row type.
	/// </summary>
	Classic,
	/// <summary>
	/// Represents a oneshot row type.
	/// </summary>
	Oneshot
}
/// <summary>
/// Represents the sources of the voice.
/// </summary>
[JsonEnumSerializable]
public enum SayReadyGetSetGoVoiceSource
{
	/// <summary>
	/// Represents the voice source "Nurse".
	/// </summary>
	Nurse,
	/// <summary>
	/// Represents the voice source "Nurse Tired".
	/// </summary>
	NurseTired,
	/// <summary>
	/// Represents the voice source "Nurse Swing".
	/// </summary>
	NurseSwing,
	/// <summary>
	/// Represents the voice source "Nurse Swing Calm".
	/// </summary>
	NurseSwingCalm,
	/// <summary>
	/// Represents the voice source "Ian Excited".
	/// </summary>
	IanExcited,
	/// <summary>
	/// Represents the voice source "Ian Calm".
	/// </summary>
	IanCalm,
	/// <summary>
	/// Represents the voice source "Ian Slow".
	/// </summary>
	IanSlow,
	/// <summary>
	/// Represents the voice source "None Bottom".
	/// </summary>
	NoneBottom,
	/// <summary>
	/// Represents the voice source "None Top".
	/// </summary>
	NoneTop
}
/// <summary>
/// Represents the phrases that can be said.
/// </summary>
[JsonEnumSerializable]
public enum SayReadyGetSetGoWord
{
	/// <summary>
	/// Represents the phrase "Ready, Get Set, Go New".
	/// </summary>
	SayReaDyGetSetGoNew,
	/// <summary>
	/// Represents the phrase "Get Set, Go".
	/// </summary>
	SayGetSetGo,
	/// <summary>
	/// Represents the phrase "Ready, Get Set, One".
	/// </summary>
	SayReaDyGetSetOne,
	/// <summary>
	/// Represents the phrase "Get Set, One".
	/// </summary>
	SayGetSetOne,
	/// <summary>
	/// Represents the phrase "Rea".
	/// </summary>
	JustSayRea,
	/// <summary>
	/// Represents the phrase "Dy".
	/// </summary>
	JustSayDy,
	/// <summary>
	/// Represents the phrase "Get".
	/// </summary>
	JustSayGet,
	/// <summary>
	/// Represents the phrase "Set".
	/// </summary>
	JustSaySet,
	/// <summary>
	/// Represents the phrase "And".
	/// </summary>
	JustSayAnd,
	/// <summary>
	/// Represents the phrase "Go".
	/// </summary>
	JustSayGo,
	/// <summary>
	/// Represents the phrase "Stop".
	/// </summary>
	JustSayStop,
	/// <summary>
	/// Represents the phrase "And Stop".
	/// </summary>
	JustSayAndStop,
	/// <summary>
	/// Represents the phrase "Switch".
	/// </summary>
	SaySwitch,
	/// <summary>
	/// Represents the phrase "Watch".
	/// </summary>
	SayWatch,
	/// <summary>
	/// Represents the phrase "Listen".
	/// </summary>
	SayListen,
	/// <summary>
	/// Represents the count "1".
	/// </summary>
	Count1,
	/// <summary>
	/// Represents the count "2".
	/// </summary>
	Count2,
	/// <summary>
	/// Represents the count "3".
	/// </summary>
	Count3,
	/// <summary>
	/// Represents the count "4".
	/// </summary>
	Count4,
	/// <summary>
	/// Represents the count "5".
	/// </summary>
	Count5,
	/// <summary>
	/// Represents the count "6".
	/// </summary>
	Count6,
	/// <summary>
	/// Represents the count "7".
	/// </summary>
	Count7,
	/// <summary>
	/// Represents the count "8".
	/// </summary>
	Count8,
	/// <summary>
	/// Represents the count "9".
	/// </summary>
	Count9,
	/// <summary>
	/// Represents the count "10".
	/// </summary>
	Count10,
	/// <summary>
	/// Represents the phrase "Ready, Get Set, Go".
	/// </summary>
	SayReadyGetSetGo,
	/// <summary>
	/// Represents the phrase "Ready".
	/// </summary>
	JustSayReady
}
/// <summary>
/// Specifies the types of backgrounds.
/// </summary>
[JsonEnumSerializable]
public enum BackgroundType
{
	/// <summary>
	/// Background is a color.
	/// </summary>
	Color,
	/// <summary>
	/// Background is an image.
	/// </summary>
	Image
}
/// <summary>
/// Represents the different voice sources for the counting sound.
/// </summary>
[JsonEnumSerializable]
public enum CountingSoundVoiceSource
{
	/// <summary>
	/// Jyi Count
	/// </summary>
	JyiCount,
	/// <summary>
	/// Jyi Count Fast
	/// </summary>
	JyiCountFast,
	/// <summary>
	/// Jyi Count Calm
	/// </summary>
	JyiCountCalm,
	/// <summary>
	/// Jyi Count Tired
	/// </summary>
	JyiCountTired,
	/// <summary>
	/// Jyi Count Very Tired
	/// </summary>
	JyiCountVeryTired,
	/// <summary>
	/// Jyi Count Japanese
	/// </summary>
	JyiCountJapanese,
	/// <summary>
	/// Ian Count
	/// </summary>
	IanCount,
	/// <summary>
	/// Ian Count Fast
	/// </summary>
	IanCountFast,
	/// <summary>
	/// Ian Count Calm
	/// </summary>
	IanCountCalm,
	/// <summary>
	/// Ian Count Slow
	/// </summary>
	IanCountSlow,
	/// <summary>
	/// Ian Count Slower
	/// </summary>
	IanCountSlower,
	/// <summary>
	/// Whistle Count
	/// </summary>
	WhistleCount,
	/// <summary>
	/// Bird Count
	/// </summary>
	BirdCount,
	/// <summary>
	/// Parrot Count
	/// </summary>
	ParrotCount,
	/// <summary>
	/// Owl Count
	/// </summary>
	OwlCount,
	/// <summary>
	/// Oriole Count
	/// </summary>
	OrioleCount,
	/// <summary>
	/// Wren Count
	/// </summary>
	WrenCount,
	/// <summary>
	/// Canary Count
	/// </summary>
	CanaryCount,
	/// <summary>
	/// Spear Count
	/// </summary>
	SpearCount,
	/// <summary>
	/// Jyi Count Legacy
	/// </summary>
	JyiCountLegacy,
	/// <summary>
	/// Jyi Count English
	/// </summary>
	JyiCountEnglish,
	/// <summary>
	/// Ian Count English
	/// </summary>
	IanCountEnglish,
	/// <summary>
	/// Ian Count English Calm
	/// </summary>
	IanCountEnglishCalm,
	/// <summary>
	/// Ian Count English Slow
	/// </summary>
	IanCountEnglishSlow,
	/// <summary>
	/// Ian Count English Fast
	/// </summary>
	IanCountEnglishFast,
	/// <summary>
	/// Custom
	/// </summary>
	Custom
}
/// <summary>
/// Defines the types of intervals.
/// </summary>
[JsonEnumSerializable]
public enum HeartExplodeIntervalType
{
	/// <summary>
	/// Interval of one beat after.
	/// </summary>
	OneBeatAfter,
	/// <summary>
	/// Instant interval.
	/// </summary>
	Instant,
	/// <summary>
	/// Gather without ceiling.
	/// </summary>
	GatherNoCeil,
	/// <summary>
	/// Gather and ceiling.
	/// </summary>
	GatherAndCeil,
	/// <summary>
	/// Indicates that heart explode intervals are disabled.
	/// </summary>
	Disabled,
}
/// <summary>  
/// Defines the types of waves.  
/// </summary>  
[JsonEnumSerializable]
public enum OneshotWaveShapeType
{
	/// <summary>  
	/// Boom and rush wave.  
	/// </summary>  
	BoomAndRush,
	/// <summary>  
	/// Ball wave.  
	/// </summary>  
	Ball,
	/// <summary>  
	/// Spring wave.  
	/// </summary>  
	Spring,
	/// <summary>  
	/// Spike wave.  
	/// </summary>  
	Spike,
	/// <summary>  
	/// Huge spike wave.  
	/// </summary>  
	SpikeHuge,
	/// <summary>  
	/// Single wave.  
	/// </summary>  
	Single
}
/// <summary>
/// Defines the play styles.
/// </summary>
[JsonEnumSerializable]
public enum PlayStyleType
{
	/// <summary>
	/// None.
	/// </summary>
	None,
	/// <summary>
	/// Normal play style.
	/// </summary>
	Normal,
	/// <summary>
	/// Default.
	/// </summary>
	Default,
	/// <summary>
	/// Loop play style.
	/// </summary>
	Loop,
	/// <summary>
	/// Loop only in beat.
	/// </summary>
	BeatLoopOnly,
	/// <summary>
	/// Prolong play style.
	/// </summary>
	Prolong,
	/// <summary>
	/// Prolong one bar play style.
	/// </summary>
	ProlongOneBar,
	/// <summary>
	/// Play on next bar.
	/// </summary>
	OnNextBar,
	/// <summary>
	/// 
	/// </summary>
	ScrubToNext,
	/// <summary>
	/// Immediate play style.
	/// </summary>
	Immediately,
	/// <summary>
	/// Extra immediate play style.
	/// </summary>
	ExtraImmediately,
}
/// <summary>
/// Specifies the style of the synco sound for the SetRowXs event.
/// </summary>
[JsonEnumSerializable]
public enum SetRowXsSyncoStyle
{
	/// <summary>
	/// Use the "Chirp" style for the synco sound.
	/// </summary>
	Chirp,
	/// <summary>
	/// Use the "Beep" style for the synco sound.
	/// </summary>
	Beep,
}
/// <summary>  
/// Represents the available themes.  
/// </summary>  
[JsonEnumSerializable]
public enum Theme
{
#pragma warning disable CS1591
	None,
	Intimate,
	IntimateSimple,
	InsomniacDay,
	InsomniacNight,
	Matrix,
	NeonMuseum,
	CrossesStraight,
	CrossesFalling,
	CubesFalling,
	CubesFallingNiceBlue,
	OrientalTechno,
	Kaleidoscope,
	PoliticiansRally,
	Rooftop,
	RooftopSummer,
	RooftopAutumn,
	BackAlley,
	Sky,
	NightSky,
	HallOfMirrors,
	CoffeeShop,
	CoffeeShopNight,
	Garden,
	GardenNight,
	TrainDay,
	TrainNight,
	DesertDay,
	DesertNight,
	HospitalWard,
	HospitalWardNight,
	PaigeOffice,
	Basement,
	ColeWardNight,
	ColeWardSunrise,
	BoyWard,
	GirlWard,
	Skyline,
	SkylineBlue,
	FloatingHeart,
	FloatingHeartWithCubes,
	FloatingHeartBroken,
	FloatingHeartBrokenWithCubes,
	ZenGarden,
	Space,
	Vaporwave,
	RollerDisco,
	Stadium,
	StadiumStormy,
	AthleteWard,
	AthleteWardNight,
	ProceduralTree,
	RecordsRoom,
	Airport,
	AbandonedWard,
	CubesFallingWithBlueBloomAndCrossesAndMatrix,
#pragma warning restore CS1591
}
/// <summary>
/// Enum representing various VFX presets.
/// </summary>
[JsonEnumSerializable]
public enum VfxPreset
{
#pragma warning disable CS1591
	SilhouettesOnHBeat,
	Vignette,
	VignetteFlicker,
	ColourfulShockwaves,
	BassDropOnHit,
	ShakeOnHeartBeat,
	ShakeOnHit,
	LightStripVert,
	VHS,
	CutsceneMode,
	HueShift,
	Brightness,
	Contrast,
	Saturation,
	Noise,
	GlitchObstruction,
	Rain,
	Matrix,
	Confetti,
	FallingPetals,
	FallingPetalsInstant,
	FallingPetalsSnow,
	FallingLeaves,
	ConfettiBurst,
	Snow,
	Bloom,
	OrangeBloom,
	BlueBloom,
	HallOfMirrors,
	TileN,
	Sepia,
	CustomScreenScroll,
	JPEG,
	NumbersAbovePulses,
	Mosaic,
	ScreenWaves,
	Funk,
	Grain,
	Blizzard,
	Drawing,
	Aberration,
	Blur,
	RadialBlur,
	Dots,
	Fisheye,
	DisableAll,
	Diamonds,
	Tutorial,
	Balloons,
	GlassShatter,
	Embers,
	HeatDistortion,
	Pixelate,
	Scanlines,
	VHSRewind,
    EyesBig,
    EyesSmall,

    WavyRows,
	Tile2,
	Tile3,
	Tile4,
	ScreenScroll,
	ScreenScrollX,
	ScreenScrollSansVHS,
	ScreenScrollXSansVHS,
	RowGlowWhite,
	RowOutline,
	RowShadow,
	RowAllWhite,
	RowSilhouetteGlow,
	RowPlain,
	BlackAndWhite,
	Blackout,
	MiawMiaw,

#pragma warning restore CS1591
}
/// <summary>  
/// Specifies the available modes for displaying content in the window.  
/// </summary>  
[JsonEnumSerializable]
public enum WindowContentMode
{
	/// <summary>  
	/// Show all rooms in this window.  
	/// </summary>  
	OnTop,
	/// <summary>
	/// Show one room in this window.
	/// </summary>
	Room,
}
/// <summary>
/// Represents the type of shake effect used in Rhythm Doctor events.
/// </summary>
[JsonEnumSerializable]
public enum ShakeType
{
	/// <summary>
	/// Standard shake effect.
	/// </summary>
	Normal,
	/// <summary>
	/// Smooth shake effect.
	/// </summary>
	Smooth,
	/// <summary>
	/// Rotational shake effect.
	/// </summary>
	Rotate,
	/// <summary>
	/// Bass drop shake effect.
	/// </summary>
	BassDrop,
}
/// <summary>
/// Specifies the sides where the dialogue panel can be shown.
/// </summary>
[JsonEnumSerializable]
public enum DialogueSide
{
	/// <summary>
	/// The bottom side.
	/// </summary>
	Bottom,
	/// <summary>
	/// The top side.
	/// </summary>
	Top
}
/// <summary>
/// Specifies the sides where the portrait can be shown.
/// </summary>
[JsonEnumSerializable]
public enum DialoguePortraitSide
{
	/// <summary>
	/// The left side.
	/// </summary>
	Left,
	/// <summary>
	/// The right side.
	/// </summary>
	Right
}
/// <summary>
/// Defines the possible actions for the event.
/// </summary>
[JsonEnumSerializable]
public enum ShowHandsAction
{
	/// <summary>
	/// Show the hands.
	/// </summary>
	Show,
	/// <summary>
	/// Hide the hands.
	/// </summary>
	Hide,
	/// <summary>
	/// Raise the hands.
	/// </summary>
	Raise,
	/// <summary>
	/// Lower the hands.
	/// </summary>
	Lower
}
/// <summary>
/// Defines the possible extents for the action.
/// </summary>
[JsonEnumSerializable]
public enum ShowHandsExtent
{
	/// <summary>
	/// Full extent.
	/// </summary>
	Full,
	/// <summary>
	/// Short extent.
	/// </summary>
	Short
}
/// <summary>
/// Specifies the display mode for subdivision rows.
/// </summary>
[JsonEnumSerializable]
public enum ShowSubdivisionsRowsMode
{
	/// <summary>
	/// Mini mode for displaying subdivision rows.
	/// </summary>
	Mini,
	/// <summary>
	/// Normal mode for displaying subdivision rows.
	/// </summary>
	Normal,
}
/// <summary>
/// Enumerates the possible spinning actions that can be applied to rows.
/// </summary>
[JsonEnumSerializable]
public enum SpiningAction
{
	/// <summary>
	/// Connect the current row to another row (use <see cref="SpinningRows.ToRow"/> to indicate the target).
	/// </summary>
	Connect,

	/// <summary>
	/// Disconnect the current row from any connected row.
	/// </summary>
	Disconnect,

	/// <summary>
	/// Rotate the row by a specified angle over a duration with optional easing.
	/// </summary>
	Rotate,

	/// <summary>
	/// Apply a continuous (constant speed) rotation to the row.
	/// </summary>
	ConstantRotation,

	/// <summary>
	/// Apply a wavy rotational motion using amplitude and frequency parameters.
	/// </summary>
	WavyRotation,

	/// <summary>
	/// Merge rows together with optional visual effects.
	/// </summary>
	Merge,

	/// <summary>
	/// Split a row into multiple parts using rotational animation.
	/// </summary>
	Split,
}
/// <summary>
/// Defines the possible actions for the stutter event.
/// </summary>
[JsonEnumSerializable]
public enum StutterAction
{
	/// <summary>
	/// Add action.
	/// </summary>
	Add,
	/// <summary>
	/// Cancel action.
	/// </summary>
	Cancel
}
/// <summary>  
/// Specifies the different tabs available in the RhythmBase application.  
/// </summary>  
[JsonEnumSerializable]
public enum Tab
{
	/// <summary>  
	/// Represents the Sounds tab.  
	/// </summary>  
	[JsonAlias("Song")]
	Sounds,
	/// <summary>  
	/// Represents the Rows tab.  
	/// </summary>  
	Rows,
	/// <summary>  
	/// Represents the Actions tab.  
	/// </summary>  
	Actions,
	/// <summary>  
	/// Represents the Decorations tab.  
	/// </summary>  
	[JsonAlias("Sprites")]
	Decorations,
	/// <summary>  
	/// Represents the Rooms tab.  
	/// </summary>  
	Rooms,
	/// <summary>  
	/// Represents the Windows tab.  
	/// </summary>  
	Windows,
	/// <summary>  
	/// Represents an unknown tab.  
	/// </summary>  
	Unknown
}
/// <summary>
/// Defines the possible actions for a tag.
/// </summary>
[JsonEnumSerializable]
public enum ActionTagAction
{
    /// <summary>
    /// Run events that match the specified tag.
    /// </summary>
    Run,
    /// <summary>
    /// Run events that its tag contains the specified tag.
    /// </summary>
    RunAll,
    /// <summary>
    /// Enables events that match the specified tag.
    /// </summary>
    Enable,
    /// <summary>
    /// Disables events that match the specified tag.
    /// </summary>
    Disable,
    /// <summary>
    /// Enables events that its tag contains the specified tag.
    /// </summary>
    EnableAll,
    /// <summary>
    /// Disables events that its tag contains the specified tag.
    /// </summary>
    DisableAll,
    /// <summary>
    /// Randomly choose an event from all the events that match the specified tag and run it.
    /// </summary>
    RunRandom,
}
/// <summary>
/// Specifies the direction of the text explosion.
/// </summary>
[JsonEnumSerializable]
public enum TextExplosionDirection
{
	/// <summary>
	/// The text explodes to the left.
	/// </summary>
	Left,
	/// <summary>
	/// The text explodes to the right.
	/// </summary>
	Right
}
/// <summary>
/// Specifies the mode of the text explosion.
/// </summary>
[JsonEnumSerializable]
public enum TextExplosionMode
{
	/// <summary>
	/// The text explosion uses one color.
	/// </summary>
	OneColor,
	/// <summary>
	/// The text explosion uses random colors.
	/// </summary>
	Random
}
/// <summary>
/// Represents the types of tiling that can be applied.
/// </summary>
[JsonEnumSerializable]
public enum TilingType
{
	/// <summary>
	/// Tiling type where the content scrolls.
	/// </summary>
	Scroll,
	/// <summary>
	/// Tiling type where the content pulses.
	/// </summary>
	Pulse
}
/// <summary>
/// Specifies the row effects.
/// </summary>
[JsonEnumSerializable]
public enum TintRowEffect
{
	/// <summary>
	/// No effect.
	/// </summary>
	None,
	/// <summary>
	/// Electric effect.
	/// </summary>
	Electric,
	/// <summary>
	/// Smoke effect.
	/// </summary>
	Smoke
}
/// <summary>
/// Specifies how the pivot is interpreted when applying a window resize.
/// </summary>
[JsonEnumSerializable]
public enum PivotMode
{
	/// <summary>
	/// Use the default pivot behavior. Typically uses the pivot value directly without special anchoring.
	/// </summary>
	Default,

	/// <summary>
	/// Treat the pivot as an edge anchor. Pivot operations will align content relative to the specified window edge.
	/// </summary>
	AnchorEdge,
}

/// <summary>
/// Describes which edge of the window the content should be anchored to when resizing.
/// </summary>
[JsonEnumSerializable]
public enum WindowAnchorType
{
	/// <summary>
	/// No anchoring. Content is not anchored to any specific edge.
	/// </summary>
	None,

	/// <summary>
	/// Anchor to the left edge of the window.
	/// </summary>
	LeftEdge,

	/// <summary>
	/// Anchor to the right edge of the window.
	/// </summary>
	RightEdge,

	/// <summary>
	/// Anchor to the bottom edge of the window.
	/// </summary>
	BottomEdge,

	/// <summary>
	/// Anchor to the top edge of the window.
	/// </summary>
	TopEdge,
}
/// <summary>
/// Specifies how content is scaled or positioned within a container when zooming is applied.
/// </summary>
[JsonEnumSerializable]
public enum ZoomMode
{
	/// <summary>
	/// Gets or sets the fill brush used to paint the interior of the shape.
	/// </summary>
	Fill,
	/// <summary>
	/// Specifies how content is resized to fit within a given space.
	/// </summary>
	Fit,
	/// <summary>
	/// Represents the absence of a value or a default state.
	/// </summary>
	None,
}
/// <summary>
/// Specifies the type of render filter to be used.
/// </summary>
/// 
[JsonEnumSerializable]
public enum Filter
{
	/// <summary>
	/// Nearest neighbor filtering.
	/// </summary>
	NearestNeighbor,
	/// <summary>
	/// Bilinear filtering.
	/// </summary>
	Bilinear
}
/// <summary>  
/// Defines the types of sounds.  
/// </summary>  
[JsonEnumSerializable]
public enum SoundType
{
#pragma warning disable CS1591
	ClapSoundP1Classic,
	ClapSoundP2Classic,
	ClapSoundP1Oneshot,
	ClapSoundP2Oneshot,
	SmallMistake,
	BigMistake,
	Hand1PopSound,
	Hand2PopSound,
	HeartExplosion,
	HeartExplosion2,
	HeartExplosion3,
	ClapSoundHoldLongEnd,
	ClapSoundHoldLongStart,
	ClapSoundHoldShortEnd,
	ClapSoundHoldShortStart,
	PulseSoundHoldStart,
	PulseSoundHoldShortEnd,
	PulseSoundHoldEnd,
	PulseSoundHoldStartAlt,
	PulseSoundHoldShortEndAlt,
	PulseSoundHoldEndAlt,
	ClapSoundCPUClassic,
	ClapSoundCPUOneshot,
	ClapSoundHoldLongEndP2,
	ClapSoundHoldLongStartP2,
	ClapSoundHoldShortEndP2,
	ClapSoundHoldShortStartP2,
	PulseSoundHoldStartP2,
	PulseSoundHoldShortEndP2,
	PulseSoundHoldEndP2,
	PulseSoundHoldStartAltP2,
	PulseSoundHoldShortEndAltP2,
	PulseSoundHoldEndAltP2,
	FreezeshotSoundCueLow,
	FreezeshotSoundCueHigh,
	FreezeshotSoundRiser,
	FreezeshotSoundCymbal,
	BurnshotSoundCueLow,
	BurnshotSoundCueHigh,
	BurnshotSoundRiser,
	BurnshotSoundCymbal,
	ClapSoundHold,
	PulseSoundHold,
	ClapSoundHoldP2,
	PulseSoundHoldP2,
	FreezeshotSound,
	BurnshotSound,
	Skipshot,
	HoldshotSound,
	HoldshotSoundCue,
	HoldshotSoundClapStart,
	HoldshotSoundClapLongEnd,
	HoldshotSoundClapShortEnd,
#pragma warning restore CS1591
}
/// <summary>
/// Specifies the available font rendering styles for Rhythm Doctor text elements.
/// </summary>
[JsonEnumSerializable]
public enum RDFontType
{
	/// <summary>
	/// Uses the default project font, typically optimized for general UI text.
	/// </summary>
	Default,
	/// <summary>
	/// Renders text with pixel-perfect precision, ideal for retro aesthetics.
	/// </summary>
	Pixel,
	/// <summary>
	/// Utilizes vector-based rendering to keep text crisp at any scale.
	/// </summary>
	Vector,
	/// <summary>
	/// Applies a Flash-inspired font style for legacy content compatibility.
	/// </summary>
	Flash
}
/// <summary>
/// Defines the types of visual layers used in the rendering process for organizing and displaying scene elements.
/// </summary>
[JsonEnumSerializable]
public enum LayerType
{
	/// <summary>
	/// Dialogue layer.
	/// </summary>
	Dialogue,
	/// <summary>
	/// Foreground layer for particles.
	/// </summary>
	ForegroundParticles,
	/// <summary>
	/// Foreground layer.
	/// </summary>
	Foreground,
	/// <summary>
	/// Default layer.
	/// </summary>
	Default,
	/// <summary>
	/// Background layer.
	/// </summary>
	Background,
	/// <summary>
	/// Background layer for particles.
	/// </summary>
	BackgroundParticles,
}
/// <summary>
/// The heart types of the row.
/// </summary>
[JsonEnumSerializable]
public enum HeartType
{
	/// <summary>
	/// Default heart type.
	/// </summary>
	Default,
	/// <summary>
	/// The heart is infected by Connectifa.
	/// </summary>
	Infected,
	/// <summary>
	/// The heart is cracked.
	/// </summary>
	Cracked,
	/// <summary>
	/// The left half of the heart.
	/// </summary>
	SplitLeft,
	/// <summary>
	/// The right half of the heart.
	/// </summary>
	SplitRight,
	/// <summary>
	/// The pumpkin.
	/// </summary>
	Halloween,
	/// <summary>
	/// The heart of game Unbeatable.
	/// </summary>
	Unbeatable,
	/// <summary>
	/// None.
	/// </summary>
	None,
}
/// <summary>
/// Represents the index of a room with various possible values.
/// </summary>
[Flags]
public enum RoomIndex : byte
{
	/// <summary>
	/// No room selected.
	/// </summary>
	None = 0b0000_0000,
	/// <summary>
	/// Represents Room 1.
	/// </summary>
	Room1 = 0b0000_0001,
	/// <summary>
	/// Represents Room 2.
	/// </summary>
	Room2 = 0b0000_0010,
	/// <summary>
	/// Represents Room 3.
	/// </summary>
	Room3 = 0b0000_0100,
	/// <summary>
	/// Represents Room 4.
	/// </summary>
	Room4 = 0b0000_1000,
	/// <summary>
	/// Represents the top room.
	/// </summary>
	RoomTop = 0b0001_0000,
	/// <summary>
	/// Indicates that the room is not available.
	/// </summary>
	RoomNotAvaliable = byte.MaxValue,
}

/// <summary>
/// In-game character.
/// </summary>
[JsonEnumSerializable]
public enum GameCharacter
{
#pragma warning disable CS1591
	Adog,
	Allison,
	Athlete,
	AthleteAlt,
	AthletePhysio,
	Barista,
	Beans,
	Beat,
	BlankCPU,
	Bodybuilder,
	BookDiary,
	BookEdega,
	BookEye,
	Boy,
	BoyRaya,
	BoyTangzhuang,
	Buro,
	Canary,
	Clef,
	Cockatiel,
	ColeGuitar,
	ColeSynth,
	Controller,
	Cranky,
	Custom,
	DancingCouple,
	Edega,
	Farmer,
	FarmerAlternate,
	Girl,
	GirlCNY,
	HoodieBoy,
	HoodieBoyAlternate,
	HoodieBoyBlue,
	Ian,
	IanBubble,
	Janitor,
	Lucia,
	LuckyBag,
	LuckyBaseball,
	LuckyIce,
	LuckyJersey,
	Lune,
	Marija,
	Mark,
	Miner,
	MrsStevendog,
	MrsStevenson,
	MrsStevensonTango,
	MrStevendog,
	MrStevenson,
	MrStevensonTango,
	New,
	NicoleCigs,
	NicoleCoffee,
	NicoleMints,
	None,
	Oriole,
	Otto,
	Owl,
	Paige,
	Parrot,
	Player,
	Politician,
	Purritician,
	Quaver,
	Rin,
	Rodney,
	RhythmJanitor,
	RhythmJanitorReal,
	RhythmNurse,
	RhythmSecurity,
	Samurai,
	SamuraiBaseball,
	SamuraiBlue,
	SamuraiBoss,
	SamuraiBossAlt,
	SamuraiGirl,
	SamuraiGreen,
	SamuraiPirate,
	SamuraiYellow,
	Saturday,
	SmokinBarista,
	Sophia,
	Tentacle,
	Treble,
	Weightlifter,
	Wren,
}
