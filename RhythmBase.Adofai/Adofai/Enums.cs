namespace RhythmBase.Adofai;

/// <summary>
/// Represents the types of events available in the Adofai editor.
/// </summary>
[JsonEnumSerializable]
public enum EventType

{
	/// <summary>
	/// Adds a decoration to the level.
	/// </summary>
	/// <summary>  
	/// Adds a decoration to the level.  
	/// </summary>  
	AddDecoration,
	/// <summary>  
	/// Adds a particle effect to the level.  
	/// </summary>  
	AddParticle,
	/// <summary>
	/// Adds an object to the level.
	/// </summary>
	AddObject,
	/// <summary>
	/// Adds text to the level.
	/// </summary>
	AddText,
	/// <summary>
	/// Animates a track in the level.
	/// </summary>
	AnimateTrack,
	/// <summary>
	/// Enables autoplay for specific tiles.
	/// </summary>
	AutoPlayTiles,
	/// <summary>
	/// Adjusts the bloom effect in the level.
	/// </summary>
	Bloom,
	/// <summary>
	/// Adds a bookmark to the level.
	/// </summary>
	Bookmark,
	/// <summary>
	/// Represents a change track for monitoring modifications to a collection of items.
	/// </summary>
	ChangeTrack,
	/// <summary>
	/// Creates a checkpoint in the level.
	/// </summary>
	Checkpoint,
	/// <summary>
	/// Changes the color of a track.
	/// </summary>
	ColorTrack,
	/// <summary>
	/// Sets a custom background for the level.
	/// </summary>
	CustomBackground,
	/// <summary>
	/// Defines a custom event in the level.
	/// </summary>
	ForwardEvent,
	/// <summary>
	/// Defines a custom tile event in the level.
	/// </summary>
	ForwardTileEvent,
	/// <summary>
	/// Adds a comment in the editor.
	/// </summary>
	EditorComment,
	/// <summary>  
	/// Emits a particle effect in the level.  
	/// </summary>  
	EmitParticle,
	/// <summary>
	/// Creates a flash effect in the level.
	/// </summary>
	Flash,
	/// <summary>
	/// Enables free roam mode.
	/// </summary>
	FreeRoam,
	/// <summary>
	/// Removes free roam mode.
	/// </summary>
	FreeRoamRemove,
	/// <summary>
	/// Adds a twirl effect in free roam mode.
	/// </summary>
	FreeRoamTwirl,
	/// <summary>
	/// Represents a warning message displayed when the user is in free roam mode.
	/// </summary>
	FreeRoamWarning,
	/// <summary>
	/// Applies a hall of mirrors effect.
	/// </summary>
	HallOfMirrors,
	/// <summary>
	/// Hides an object or element in the level.
	/// </summary>
	Hide,
	/// <summary>
	/// Holds an action or event.
	/// </summary>
	Hold,
	/// <summary>
	/// Moves the camera to a specific position.
	/// </summary>
	MoveCamera,
	/// <summary>
	/// Moves decorations in the level.
	/// </summary>
	MoveDecorations,
	/// <summary>
	/// Moves a track to a specific position.
	/// </summary>
	MoveTrack,
	/// <summary>
	/// Enables multi-planet mode.
	/// </summary>
	MultiPlanet,
	/// <summary>
	/// Pauses the level.
	/// </summary>
	Pause,
	/// <summary>
	/// Plays a sound in the level.
	/// </summary>
	PlaySound,
	/// <summary>
	/// Sets the position of a track.
	/// </summary>
	PositionTrack,
	/// <summary>
	/// Recolors a track in the level.
	/// </summary>
	RecolorTrack,
	/// <summary>
	/// Repeats specific events in the level.
	/// </summary>
	RepeatEvents,
	/// <summary>
	/// Scales the margin of the level.
	/// </summary>
	ScaleMargin,
	/// <summary>
	/// Scales the planets in the level.
	/// </summary>
	ScalePlanets,
	/// <summary>
	/// Scales the radius of the level.
	/// </summary>
	ScaleRadius,
	/// <summary>
	/// Scrolls the screen in the level.
	/// </summary>
	ScreenScroll,
	/// <summary>
	/// Sets the screen tile effect.
	/// </summary>
	ScreenTile,
	/// <summary>
	/// Sets the background of the level.
	/// </summary>
	SetBackground,
	/// <summary>
	/// Sets conditional events in the level.
	/// </summary>
	SetConditionalEvents,
	/// <summary>
	/// Sets the default text for the level.
	/// </summary>
	SetDefaultText,
	/// <summary>
	/// Applies a filter effect in the level.
	/// </summary>
	SetFilter,
	/// <summary>
	/// Sets an advanced filter effect in the level.
	/// </summary>
	SetFilterAdvanced,
	/// <summary>
	/// Sets the frame rate for the level.
	/// </summary>
	SetFrameRate,
	/// <summary>
	/// Sets the hitsound for the level.
	/// </summary>
	SetHitsound,
	/// <summary>
	/// Sets the hold sound for the level.
	/// </summary>
	SetHoldSound,
	/// <summary>
	/// Sets the input event for the level.
	/// </summary>
	SetInputEvent,
	/// <summary>
	/// Sets an object in the level.
	/// </summary>
	SetObject,
	/// <summary>
	/// Sets the particle for the level.
	/// </summary>
	SetParticle,
	/// <summary>
	/// Sets the rotation of a planet.
	/// </summary>
	SetPlanetRotation,
	/// <summary>
	/// Sets the speed of the level.
	/// </summary>
	SetSpeed,
	/// <summary>
	/// Sets text in the level.
	/// </summary>
	SetText,
	/// <summary>
	/// Creates a screen shake effect.
	/// </summary>
	ShakeScreen,
	/// <summary>
	/// Adds a twirl effect to the level.
	/// </summary>
	Twirl
}/// <summary>
 /// Specifies how a decoration's hitbox trigger should behave when activated.
 /// </summary>
[JsonEnumSerializable]
public enum HitboxTriggerType
{
	/// <summary>
	/// The hitbox triggers a single time and then stops until reactivated.
	/// </summary>
	Once,
	/// <summary>
	/// The hitbox triggers each time a distinct touch or contact occurs.
	/// </summary>
	PerTouch,
	/// <summary>
	/// The hitbox triggers repeatedly at the configured repeat interval while active.
	/// </summary>
	Repeat,
}
/// <summary>
/// Defines what kinds of objects the hitbox detection system should consider as valid targets.
/// </summary>
[JsonEnumSerializable]
public enum HitboxDetectTarget
{
	/// <summary>
	/// The hitbox detects collisions with planets.
	/// </summary>
	Planet,
	/// <summary>
	/// The hitbox detects collisions with decorations.
	/// </summary>
	Decoration,
}
/// <summary>
/// Specifies which planets are considered by the hitbox when the detect target is a planet.
/// </summary>
[JsonEnumSerializable]
public enum HitboxTargetPlanet
{
	/// <summary>
	/// The hitbox can target any planet (no restriction).
	/// </summary>
	Any,
	/// <summary>
	/// The hitbox targets the central planet.
	/// </summary>
	Center,
	/// <summary>
	/// The hitbox targets orbiting planets.
	/// </summary>
	Orbiting,
}
/// <summary>
/// Specifies the coordinate space or reference frame used to interpret an object's position and pivot offsets.
/// </summary>
/// <remarks>
/// Use this enum to indicate whether object coordinates are evaluated relative to a tile, the global level
/// coordinate system, a specific planet, the camera, or the last used object position. This affects how
/// Position, PivotOffset and related properties are applied when the object is placed or rendered.
/// </remarks>
[JsonEnumSerializable]
public enum ObjectRelativeTo
{
	/// <summary>
	/// Position is relative to the current tile. The object's coordinates are interpreted in the local tile space.
	/// </summary>
	Tile,
	/// <summary>
	/// Position is in global (level) coordinates. The object's coordinates are independent of individual tiles.
	/// </summary>
	Global,
	/// <summary>
	/// Position is relative to the red planet. Coordinates are interpreted in the red planet's local space.
	/// </summary>
	RedPlanet,
	/// <summary>
	/// Position is relative to the blue planet. Coordinates are interpreted in the blue planet's local space.
	/// </summary>
	BluePlanet,
	/// <summary>
	/// Position is relative to the green planet. Coordinates are interpreted in the green planet's local space.
	/// </summary>
	GreenPlanet,
	/// <summary>
	/// Position is relative to the camera. Use this to place objects in camera space (camera-local coordinates).
	/// </summary>
	Camera,
	/// <summary>
	/// Position is relative to the camera's aspect or viewport space. Useful for screen-aligned placements that depend on aspect ratio.
	/// </summary>
	CameraAspect,
	/// <summary>
	/// Position uses the last known position used by a previously placed object (e.g. for repeated placements).
	/// </summary>
	LastPosition,
}

/// <summary>  
/// Represents the types of objects that can be added.  
/// </summary>  
[JsonEnumSerializable]
public enum ObjectType
{
	/// <summary>  
	/// Represents a floor object.  
	/// </summary>  
	Floor,
	/// <summary>  
	/// Represents a planet object.  
	/// </summary>  
	Planet
}
/// <summary>  
/// Represents the types of planet colors.  
/// </summary>  
[JsonEnumSerializable]
public enum PlanetColorType
{
	/// <summary>  
	/// Default red color.  
	/// </summary>  
	DefaultRed,
	/// <summary>  
	/// Custom planet color type.  
	/// </summary>  
	planetColorType,
	/// <summary>  
	/// Gold color.  
	/// </summary>  
	Gold,
	/// <summary>  
	/// Overseer color.  
	/// </summary>  
	Overseer,
	/// <summary>  
	/// Custom color.  
	/// </summary>  
	Custom
}
/// <summary>  
/// Represents the types of tracks.  
/// </summary>  
[JsonEnumSerializable]
public enum TrackType
{
	/// <summary>  
	/// Normal track type.  
	/// </summary>  
	Normal,
	/// <summary>  
	/// Midspin track type.  
	/// </summary>  
	Midspin
}
/// <summary>
/// Specifies the type of icon used to represent a track element in the application.
/// </summary>
/// <remarks>The <see cref="TrackIconType"/> enumeration provides various icon types that can be used to visually
/// represent different track elements. Each value corresponds to a specific icon style, such as a swirl, snail, or
/// rabbit, which can be used to convey different meanings or statuses within the application's user
/// interface.</remarks>
[JsonEnumSerializable]
public enum TrackIconType
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
	None,
	Swirl,
	Snail,
	DoubleSnail,
	Rabbit,
	DoubleRabbit,
	Checkpoint,
	HoldArrowShort,
	HoldArrowLong,
	HoldReleaseShort,
	HoldReleaseLong,
	MultiPlanetTwo,
	MultiPlanetThreeMore,
	Portal,
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
/// <summary>
/// Specifies the shape type for particle emission.
/// </summary>
[JsonEnumSerializable]
public enum EmissionShapeType
{
	/// <summary>
	/// Particles are emitted in a circular shape.
	/// </summary>
	Circle,

	/// <summary>
	/// Particles are emitted in a rectangular shape.
	/// </summary>
	Rectangle,
}

/// <summary>
/// Specifies the mode of the color gradient.
/// </summary>
[JsonEnumSerializable]
public enum ColorMode
{
	/// <summary>
	/// A single color is used.
	/// </summary>
	Color,

	/// <summary>
	/// A gradient is used.
	/// </summary>
	Gradient,

	/// <summary>
	/// Two colors are used.
	/// </summary>
	TwoColors,

	/// <summary>
	/// Two gradients are used.
	/// </summary>
	TwoGradients,

	/// <summary>
	/// A random color is used.
	/// </summary>
	RandomColor,
}
/// <summary>
/// Specifies the mode of the gradient.
/// </summary>
[JsonEnumSerializable]
public enum GradientMode
{
	/// <summary>
	/// The gradient blends smoothly between colors.
	/// </summary>
	Blend,

	/// <summary>
	/// The gradient uses fixed color transitions.
	/// </summary>
	Fixed,

	/// <summary>
	/// The gradient uses perceptual blending for smoother transitions.
	/// </summary>
	PerceptualBlend,
}

/// <summary>
/// Specifies the mode of the arc emission.
/// </summary>
[JsonEnumSerializable]
public enum ArcMode
{
	/// <summary>
	/// Emission occurs at random angles within the arc.
	/// </summary>
	Random,

	/// <summary>
	/// Emission occurs sequentially in a loop within the arc.
	/// </summary>
	Loop,

	/// <summary>
	/// Emission alternates back and forth within the arc.
	/// </summary>
	PingPong,

	/// <summary>
	/// Emission occurs based on the burst speed within the arc.
	/// </summary>
	BurstSpeed,
}
/// <summary>
/// Specifies the simulation space for the particle effect.
/// </summary>
[JsonEnumSerializable]
public enum SimulationSpace
{
	/// <summary>
	/// The particle effect is simulated in the local coordinate space.
	/// </summary>
	Local,

	/// <summary>
	/// The particle effect is simulated in the world coordinate space.
	/// </summary>
	World
}
/// <summary>  
/// Specifies the hitbox types available for the decoration.  
/// </summary>  
[JsonEnumSerializable]
public enum HitboxTypes
{
	/// <summary>  
	/// No hitbox is applied.  
	/// </summary>  
	None,
	/// <summary>  
	/// A hitbox that causes the player to fail when touched.  
	/// </summary>  
	Kill,
	/// <summary>  
	/// A hitbox that triggers an event when touched.  
	/// </summary>  
	Event
}
/// <summary>  
/// Specifies the blend modes available for the decoration.  
/// </summary>  
[JsonEnumSerializable]
public enum BlendMode
{
	/// <summary>  
	/// No blending is applied.  
	/// </summary>  
	None,
	/// <summary>  
	/// Darkens the image by selecting the darker of the base or blend colors.  
	/// </summary>  
	Darken,
	/// <summary>  
	/// Multiplies the base color by the blend color.  
	/// </summary>  
	Multiply,
	/// <summary>  
	/// Darkens the base color to reflect the blend color by increasing contrast.  
	/// </summary>  
	ColorBurn,
	/// <summary>  
	/// Darkens the base color by decreasing brightness.  
	/// </summary>  
	LinearBurn,
	/// <summary>  
	/// Darkens the image by selecting the darkest color.  
	/// </summary> 
	DarkerColor,
	/// <summary>  
	/// Lightens the image by selecting the lighter of the base or blend colors.  
	/// </summary>  
	Lighten,
	/// <summary>  
	/// Multiplies the inverse of the base and blend colors.  
	/// </summary>  
	Screen,
	/// <summary>  
	/// Brightens the base color to reflect the blend color by decreasing contrast.  
	/// </summary>  
	ColorDodge,
	/// <summary>  
	/// Brightens the base color by increasing brightness.  
	/// </summary>  
	LinearDodge,
	/// <summary>  
	/// Lightens the image by selecting the lightest color.  
	/// </summary>  
	LighterColor,
	/// <summary>  
	/// Combines the base and blend colors to create a soft overlay effect.  
	/// </summary>  
	Overlay,
	/// <summary>  
	/// Applies a soft light effect to the base color.  
	/// </summary>  
	SoftLight,
	/// <summary>  
	/// Applies a hard light effect to the base color.  
	/// </summary>  
	HardLight,
	/// <summary>  
	/// Increases the contrast of the base color using the blend color.  
	/// </summary>  
	VividLight,
	/// <summary>  
	/// Adjusts the brightness of the base color using the blend color.  
	/// </summary>  
	LinearLight,
	/// <summary>  
	/// Replaces the base color with the blend color where the blend color is darker.  
	/// </summary>  
	PinLight,
	/// <summary>  
	/// Creates a high-contrast effect by combining the base and blend colors.  
	/// </summary>  
	HardMix,
	/// <summary>  
	/// Subtracts the darker color from the lighter color.  
	/// </summary>  
	Difference,
	/// <summary>  
	/// Subtracts the base color from the blend color or vice versa to create an exclusion effect.  
	/// </summary>  
	Exclusion,
	/// <summary>  
	/// Subtracts the blend color from the base color.  
	/// </summary>  
	Subtract,
	/// <summary>  
	/// Divides the base color by the blend color.  
	/// </summary>  
	Divide,
	/// <summary>  
	/// Applies the hue of the blend color to the base color.  
	/// </summary>  
	Hue,
	/// <summary>  
	/// Applies the saturation of the blend color to the base color.  
	/// </summary>  
	Saturation,
	/// <summary>  
	/// Applies the color of the blend color to the base color.  
	/// </summary>  
	Color,
	/// <summary>  
	/// Applies the luminosity of the blend color to the base color.  
	/// </summary>  
	Luminosity
}
/// <summary>
/// Specifies the reference point for the camera in the game.
/// </summary>
[JsonEnumSerializable]
public enum CameraRelativeTo
{
	/// <summary>
	/// The camera is relative to the player.
	/// </summary>
	Player,
	/// <summary>
	/// The camera is relative to the tile.
	/// </summary>
	Tile,
	/// <summary>
	/// The camera is relative to a global position.
	/// </summary>
	Global,
	/// <summary>
	/// The camera is relative to the last position.
	/// </summary>
	LastPosition,
	/// <summary>
	/// The camera is relative to the last position without considering rotation.
	/// </summary>
	LastPositionNoRotation
}
/// <summary>
/// Represents the different types of track colors available in the application.
/// </summary>
[JsonEnumSerializable]
public enum TrackColorType
{
	/// <summary>
	/// Represents a state where no specific value or option is selected.
	/// </summary>
	None,
	/// <summary>
	/// A single solid color for the track.
	/// </summary>
	Single,
	/// <summary>
	/// Alternating stripes of colors for the track.
	/// </summary>
	Stripes,
	/// <summary>
	/// A glowing effect for the track.
	/// </summary>
	Glow,
	/// <summary>
	/// A blinking effect for the track.
	/// </summary>
	Blink,
	/// <summary>
	/// A switching effect between different colors for the track.
	/// </summary>
	Switch,
	/// <summary>
	/// A rainbow gradient effect for the track.
	/// </summary>
	Rainbow,
	/// <summary>
	/// A color effect based on the volume of the track.
	/// </summary>
	Volume,
}
/// <summary>
/// Represents the types of track animations available in the ADTrack system.
/// </summary>
[JsonEnumSerializable]
public enum TrackAnimationType
{
	/// <summary>
	/// No animation.
	/// </summary>
	None,
	/// <summary>
	/// Assemble animation type.
	/// </summary>
	Assemble,
	/// <summary>
	/// Assemble animation type with a far effect.
	/// </summary>
	Assemble_Far,
	/// <summary>
	/// Extend animation type.
	/// </summary>
	Extend,
	/// <summary>
	/// Grow animation type.
	/// </summary>
	Grow,
	/// <summary>
	/// Grow animation type with a spinning effect.
	/// </summary>
	Grow_Spin,
	/// <summary>
	/// Fade animation type.
	/// </summary>
	Fade,
	/// <summary>
	/// Drop animation type.
	/// </summary>
	Drop,
	/// <summary>
	/// Rise animation type.
	/// </summary>
	Rise
}
/// <summary>
/// Represents the types of animations used for track disappearance.
/// </summary>
[JsonEnumSerializable]
public enum TrackDisappearAnimationType
{
	/// <summary>
	/// No animation is applied.
	/// </summary>
	None,
	/// <summary>
	/// The track disappears with a scatter effect.
	/// </summary>
	Scatter,
	/// <summary>
	/// The track disappears with a far scatter effect.
	/// </summary>
	Scatter_Far,
	/// <summary>
	/// The track retracts before disappearing.
	/// </summary>
	Retract,
	/// <summary>
	/// The track shrinks before disappearing.
	/// </summary>
	Shrink,
	/// <summary>
	/// The track shrinks and spins before disappearing.
	/// </summary>
	Shrink_Spin,
	/// <summary>
	/// The track fades out before disappearing.
	/// </summary>
	Fade
}
/// <summary>
/// Represents the different styles of tracks available in the game.
/// </summary>
[JsonEnumSerializable]
public enum TrackStyle
{
	/// <summary>
	/// The standard track style.
	/// </summary>
	Standard,
	/// <summary>
	/// A neon-themed track style.
	/// </summary>
	Neon,
	/// <summary>
	/// A neon light-themed track style.
	/// </summary>
	NeonLight,
	/// <summary>
	/// A basic and simple track style.
	/// </summary>
	Basic,
	/// <summary>
	/// A track style featuring gem-like visuals.
	/// </summary>
	Gems,
	/// <summary>
	/// A minimalistic track style.
	/// </summary>
	Minimal
}
/// <summary>  
/// Defines the types of speed adjustments available.  
/// </summary>  
[JsonEnumSerializable]
public enum SpeedType
{
	/// <summary>  
	/// Adjust speed using beats per minute (BPM).  
	/// </summary>  
	Bpm,
	/// <summary>  
	/// Adjust speed using a multiplier.  
	/// </summary>  
	Multiplier
}
/// <summary>  
/// Specifies the relative type of a tile reference.  
/// </summary>  
[JsonEnumSerializable]
public enum RelativeType
{
	/// <summary>  
	/// Refers to the current tile.  
	/// </summary>  
	ThisTile,
	/// <summary>  
	/// Refers to the starting tile.  
	/// </summary>  
	Start,
	/// <summary>  
	/// Refers to the ending tile.  
	/// </summary>  
	End,
}
/// <summary>
/// Represents the types of default background tile shapes.
/// </summary>
[JsonEnumSerializable]
public enum DefaultBGTileShapeType
{
	/// <summary>
	/// The default background tile shape.
	/// </summary>
	Default,

	/// <summary>
	/// A single color background tile shape.
	/// </summary>
	SingleColor,

	/// <summary>
	/// A disabled background tile shape.
	/// </summary>
	Disabled,
}
/// <summary>
/// Represents the different modes for displaying a background.
/// </summary>
[JsonEnumSerializable]
public enum BgDisplayMode
{
	/// <summary>
	/// Scales the background to fit the screen dimensions.
	/// </summary>
	FitToScreen,
	/// <summary>
	/// Displays the background without scaling.
	/// </summary>
	Unscaled,
	/// <summary>
	/// Tiles the background to fill the screen.
	/// </summary>
	Tiled
}

/// <summary>  
/// Represents the display modes for the background image.  
/// </summary>  
[JsonEnumSerializable]
public enum BackgroundDisplayMode
{
	/// <summary>  
	/// Fits the background image to the screen.  
	/// </summary>  
	FitToScreen,
	/// <summary>  
	/// Displays the background image without scaling.  
	/// </summary>  
	Unscaled,
	/// <summary>  
	/// Tiles the background image.  
	/// </summary>  
	Tiled
}
/// <summary>
/// Specifies the reference point for decoration placement in the game.
/// </summary>
[JsonEnumSerializable]
public enum DecorationRelativeTo
{
	/// <summary>
	/// Decoration is positioned relative to the tile.
	/// </summary>
	Tile,
	/// <summary>
	/// Decoration is positioned relative to the global coordinate system.
	/// </summary>
	Global,
	/// <summary>
	/// Decoration is positioned relative to the red planet.
	/// </summary>
	RedPlanet,
	/// <summary>
	/// Decoration is positioned relative to the blue planet.
	/// </summary>
	BluePlanet,
	/// <summary>
	/// Decoration is positioned relative to the green planet.
	/// </summary>
	GreenPlanet,
	/// <summary>
	/// Decoration is positioned relative to the camera.
	/// </summary>
	Camera,
	/// <summary>
	/// Decoration is positioned relative to the camera's aspect ratio.
	/// </summary>
	CameraAspect,
	/// <summary>
	/// Decoration is positioned relative to the last known position.
	/// </summary>
	LastPosition
}
/// <summary>
/// Represents the behaviors for easing parts in the ADEase system.
/// </summary>
[JsonEnumSerializable]
public enum EasePartBehaviors
{
	/// <summary>
	/// Repeats the easing behavior.
	/// </summary>
	Repeat,
	/// <summary>
	/// Mirrors the easing behavior.
	/// </summary>
	Mirror
}
/// <summary>  
/// Specifies the fail hitbox types available for the decoration.  
/// </summary>  
[JsonEnumSerializable]
public enum FailHitboxTypes
{
	/// <summary>  
	/// A rectangular fail hitbox.  
	/// </summary>  
	Box,
	/// <summary>  
	/// A circular fail hitbox.  
	/// </summary>  
	Circle,
	/// <summary>  
	/// A capsule-shaped fail hitbox.  
	/// </summary>  
	Capsule
}
/// <summary>
/// Specifies the plane on which a flash effect is rendered.
/// Use this enum to select whether the flash should appear behind or in front of foreground elements.
/// </summary>
[JsonEnumSerializable]
public enum FlashPlane
{
	/// <summary>
	/// Render the flash on the background plane (behind most foreground elements).
	/// </summary>
	Background,
	/// <summary>
	/// Render the flash on the foreground plane (in front of most foreground elements).
	/// </summary>
	Foreground,
}
/// <summary>
/// Specifies the direction used for angle correction when Free Roam mode adjusts or normalizes angles.
/// </summary>
/// <remarks>
/// Angle correction controls how the editor corrects or resolves angular discontinuities
/// when exiting or interacting with free roam. Use <see cref="None"/> to disable correction,
/// <see cref="Forward"/> to prefer increasing angle adjustments, or <see cref="Backward"/> to prefer decreasing adjustments.
/// </remarks>
[JsonEnumSerializable]
public enum AngleCorrectionDirection
{
	/// <summary>
	/// Do not perform any angle correction.
	/// </summary>
	None,
	/// <summary>
	/// Correct angles by choosing the forward (increasing) rotation direction.
	/// </summary>
	Forward,
	/// <summary>
	/// Correct angles by choosing the backward (decreasing) rotation direction.
	/// </summary>
	Backward,
}
/// <summary>  
/// Specifies the masking types available for the decoration.  
/// </summary>  
[JsonEnumSerializable]
public enum MaskingType
{
	/// <summary>  
	/// No masking is applied.  
	/// </summary>  
	None,
	/// <summary>  
	/// Applies a mask to the decoration.  
	/// </summary>  
	Mask,
	/// <summary>  
	/// Makes the decoration visible only inside the mask.  
	/// </summary>  
	VisibleInsideMask,
	/// <summary>  
	/// Makes the decoration visible only outside the mask.  
	/// </summary>  
	VisibleOutsideMask
}
/// <summary>  
/// Represents the number of planets associated with the multi-planet event.  
/// </summary>  
[JsonEnumSerializable]
public enum Planets
{
	/// <summary>  
	/// Two planets are active in the event.  
	/// </summary>  
	TwoPlanets,
	/// <summary>  
	/// Three planets are active in the event.  
	/// </summary>  
	ThreePlanets,
}

/// <summary>  
/// Defines the types of repetition available for the event.  
/// </summary>  
[JsonEnumSerializable]
public enum RepeatType
{
	/// <summary>  
	/// Repeats the event based on beats.  
	/// </summary>  
	Beat,
	/// <summary>  
	/// Repeats the event based on floors.  
	/// </summary>  
	Floor
}

/// <summary>  
/// Represents the target planets that can be scaled.  
/// </summary>  
[JsonEnumSerializable]
public enum TargetPlanets
{
	/// <summary>  
	/// The fire planet.  
	/// </summary>  
	FirePlanet,
	/// <summary>  
	/// The ice planet.  
	/// </summary>  
	IcePlanet,
	/// <summary>  
	/// The green planet.  
	/// </summary>  
	GreenPlanet,
	/// <summary>  
	/// All planets.  
	/// </summary>  
	All
}
/// <summary>  
/// Represents the available filter types.  
/// </summary>  
[JsonEnumSerializable]
public enum Filter
{
	/// <summary>  
	/// Grayscale filter.  
	/// </summary>  
	Grayscale,
	/// <summary>  
	/// Sepia filter.  
	/// </summary>  
	Sepia,
	/// <summary>  
	/// Invert colors filter.  
	/// </summary>  
	Invert,
	/// <summary>  
	/// VHS effect filter.  
	/// </summary>  
	VHS,
	/// <summary>  
	/// 1980s TV effect filter.  
	/// </summary>  
	EightiesTV,
	/// <summary>  
	/// 1950s TV effect filter.  
	/// </summary>  
	FiftiesTV,
	/// <summary>  
	/// Arcade effect filter.  
	/// </summary>  
	Arcade,
	/// <summary>  
	/// LED effect filter.  
	/// </summary>  
	LED,
	/// <summary>  
	/// Rain effect filter.  
	/// </summary>  
	Rain,
	/// <summary>  
	/// Blizzard effect filter.  
	/// </summary>  
	Blizzard,
	/// <summary>  
	/// Pixel snow effect filter.  
	/// </summary>  
	PixelSnow,
	/// <summary>  
	/// Compression effect filter.  
	/// </summary>  
	Compression,
	/// <summary>  
	/// Glitch effect filter.  
	/// </summary>  
	Glitch,
	/// <summary>  
	/// Pixelate effect filter.  
	/// </summary>  
	Pixelate,
	/// <summary>  
	/// Waves effect filter.  
	/// </summary>  
	Waves,
	/// <summary>  
	/// Static effect filter.  
	/// </summary>  
	Static,
	/// <summary>  
	/// Grain effect filter.  
	/// </summary>  
	Grain,
	/// <summary>  
	/// Motion blur effect filter.  
	/// </summary>  
	MotionBlur,
	/// <summary>  
	/// Fisheye effect filter.  
	/// </summary>  
	Fisheye,
	/// <summary>  
	/// Chromatic aberration effect filter.  
	/// </summary>  
	Aberration,
	/// <summary>  
	/// Drawing effect filter.  
	/// </summary>  
	Drawing,
	/// <summary>  
	/// Neon effect filter.  
	/// </summary>  
	Neon,
	/// <summary>  
	/// Handheld camera effect filter.  
	/// </summary>  
	Handheld,
	/// <summary>  
	/// Night vision effect filter.  
	/// </summary>  
	NightVision,
	/// <summary>  
	/// Funk effect filter.  
	/// </summary>  
	Funk,
	/// <summary>  
	/// Tunnel effect filter.  
	/// </summary>  
	Tunnel,
	/// <summary>  
	/// Weird 3D effect filter.  
	/// </summary>  
	Weird3D,
	/// <summary>  
	/// Blur effect filter.  
	/// </summary>  
	Blur,
	/// <summary>  
	/// Blur focus effect filter.  
	/// </summary>  
	BlurFocus,
	/// <summary>  
	/// Gaussian blur effect filter.  
	/// </summary>  
	GaussianBlur,
	/// <summary>  
	/// Hexagon black effect filter.  
	/// </summary>  
	HexagonBlack,
	/// <summary>  
	/// Posterize effect filter.  
	/// </summary>  
	Posterize,
	/// <summary>  
	/// Sharpen effect filter.  
	/// </summary>  
	Sharpen,
	/// <summary>  
	/// Contrast effect filter.  
	/// </summary>  
	Contrast,
	/// <summary>  
	/// Edge black line effect filter.  
	/// </summary>  
	EdgeBlackLine,
	/// <summary>  
	/// Oil paint effect filter.  
	/// </summary>  
	OilPaint,
	/// <summary>  
	/// Super dot effect filter.  
	/// </summary>  
	SuperDot,
	/// <summary>  
	/// Water drop effect filter.  
	/// </summary>  
	WaterDrop,
	/// <summary>  
	/// Light water effect filter.  
	/// </summary>  
	LightWater,
	/// <summary>  
	/// Petals effect filter.  
	/// </summary>  
	Petals,
	/// <summary>  
	/// Instant petals effect filter.  
	/// </summary>  
	PetalsInstant
}
/// <summary>
/// Represents the target type for the filter effect.
/// </summary>
[JsonEnumSerializable]
public enum TargetType
{
	/// <summary>
	/// The target is the camera.
	/// </summary>
	Camera,
	/// <summary>
	/// The target is the decoration.
	/// </summary>
	Decoration,
}
/// <summary>
/// Represents the plane where the filter is applied.
/// </summary>
[JsonEnumSerializable]
public enum Plane
{
	/// <summary>
	/// The background plane.
	/// </summary>
	Background,

	/// <summary>
	/// The foreground plane.
	/// </summary>
	Foreground,
}

/// <summary>  
/// Represents the predefined game sounds available for hitsounds.  
/// </summary>  
[JsonEnumSerializable]
public enum GameSound
{
	/// <summary>  
	/// The default hitsound.  
	/// </summary>  
	Hitsound,
	/// <summary>  
	/// The sound used for midspin events.  
	/// </summary>  
	Midspin
}
/// <summary>
/// 
/// </summary>
[JsonEnumSerializable]
public enum HitSound
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
	Hat,
	Kick,
	Shaker,
	Sizzle,
	Chuck,
	ShakerLoud,
	None,
	Hammer,
	KickChroma,
	SnareAcoustic2,
	Sidestick,
	Stick,
	ReverbClack,
	Squareshot,
	PowerDown,
	PowerUp,
	KickHouse,
	KickRupture,
	HatHouse,
	SnareHouse,
	SnareVapor,
	ClapHit,
	ClapHitEcho,
	ReverbClap,
	FireTile,
	IceTile,
	VehiclePositive,
	VehicleNegative
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}
/// <summary>  
/// Represents the types of sounds that can be played at the start or end of a hold.  
/// </summary>  
[JsonEnumSerializable]
public enum HoldSound
{
	/// <summary>  
	/// A fuse sound effect.  
	/// </summary>  
	Fuse,
	/// <summary>  
	/// No sound effect.  
	/// </summary>  
	None,
}
/// <summary>  
/// Represents the types of sounds that can be played in the middle of a hold.  
/// </summary>  
[JsonEnumSerializable]
public enum HoldMidSound
{
	/// <summary>  
	/// A fuse sound effect.  
	/// </summary>  
	Fuse,
	/// <summary>  
	/// A "SingSing" sound effect.  
	/// </summary>  
	SingSing,
	/// <summary>  
	/// No sound effect.  
	/// </summary>  
	None,
}
/// <summary>  
/// Represents the types of mid-hold sound playback behaviors.  
/// </summary>  
[JsonEnumSerializable]
public enum HoldMidSoundType
{
	/// <summary>  
	/// The sound is played once.  
	/// </summary>  
	Once,
	/// <summary>  
	/// The sound is repeated.  
	/// </summary>  
	Repeat,
}
/// <summary>
/// Specifies the possible input targets for the event.
/// </summary>
[JsonEnumSerializable]
public enum Target
{
	/// <summary>
	/// Any input target.
	/// </summary>
	Any,
	/// <summary>
	/// The first action button.
	/// </summary>
	Action1,
	/// <summary>
	/// The second action button.
	/// </summary>
	Action2,
	/// <summary>
	/// The confirm button.
	/// </summary>
	Confirm,
	/// <summary>
	/// The up direction.
	/// </summary>
	Up,
	/// <summary>
	/// The down direction.
	/// </summary>
	Down,
	/// <summary>
	/// The left direction.
	/// </summary>
	Left,
	/// <summary>
	/// The right direction.
	/// </summary>
	Right,
}

/// <summary>
/// Specifies the possible states of a key.
/// </summary>
[JsonEnumSerializable]
public enum KeyState
{
	/// <summary>
	/// The key is pressed down.
	/// </summary>
	Down,
	/// <summary>
	/// The key is released.
	/// </summary>
	Up,
}
/// <summary>  
/// Represents the various track icons that can be used in the Adofai editor.  
/// </summary>  
[JsonEnumSerializable]
public enum TrackIcon
{
	/// <summary>  
	/// No icon.  
	/// </summary>  
	None,
	/// <summary>  
	/// A single snail icon.  
	/// </summary>  
	Snall,
	/// <summary>  
	/// A double snail icon.  
	/// </summary>  
	DoubleSnall,
	/// <summary>  
	/// A rabbit icon.  
	/// </summary>  
	Rabbit,
	/// <summary>  
	/// A double rabbit icon.  
	/// </summary>  
	DoubleRabbit,
	/// <summary>  
	/// A swirl icon.  
	/// </summary>  
	Swirl,
	/// <summary>  
	/// A checkpoint icon.  
	/// </summary>  
	Checkpoint,
	/// <summary>  
	/// A short hold arrow icon.  
	/// </summary>  
	HoldArrowShort,
	/// <summary>  
	/// A long hold arrow icon.  
	/// </summary>  
	HoldArrowLong,
	/// <summary>  
	/// A short hold release icon.  
	/// </summary>  
	HoldReleaseShort,
	/// <summary>  
	/// A long hold release icon.  
	/// </summary>  
	HoldReleaseLong,
	/// <summary>  
	/// A two-planet multi-planet icon.  
	/// </summary>  
	MultiPlanetTwo,
	/// <summary>  
	/// A three-planet multi-planet icon.  
	/// </summary>  
	MultiPlanetThree,
	/// <summary>  
	/// A portal icon.  
	/// </summary>  
	Portal,
}

/// <summary>
/// Specifies the target mode for the particle effect.
/// </summary>
[JsonEnumSerializable]
public enum TargetMode
{
	/// <summary>
	/// Starts the particle effect.
	/// </summary>
	Start,

	/// <summary>
	/// Stops the particle effect.
	/// </summary>
	Stop,

	/// <summary>
	/// Clears the particle effect.
	/// </summary>
	Clear,
}

/// <summary>
/// Represents the direction of track color pulses in the Adofai rhythm game.
/// </summary>
[JsonEnumSerializable]
public enum TrackColorPulse
{
	/// <summary>
	/// No track color pulse.
	/// </summary>
	None,
	/// <summary>
	/// Track color pulses move forward.
	/// </summary>
	Forward,
	/// <summary>
	/// Track color pulses move backward.
	/// </summary>
	Backward,
}
[JsonEnumSerializable]
/// <summary>
/// The filter types.
/// </summary>
public enum FilterType
{
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AaaSuperComputer"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_AAA_SuperComputer")]
    AaaSuperComputer,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AaaSuperHexagon"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_AAA_SuperHexagon")]
    AaaSuperHexagon,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AaaWaterDrop"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_AAA_WaterDrop")]
    AaaWaterDrop,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AlienVision"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Alien_Vision")]
    AlienVision,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AntialiasingFxaa"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Antialiasing_FXAA")]
    AntialiasingFxaa,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AtmosphereRain"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Atmosphere_Rain")]
    AtmosphereRain,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AtmosphereRainPro"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Atmosphere_Rain_Pro")]
    AtmosphereRainPro,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.AtmosphereSnow8Bits"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Atmosphere_Snow_8bits")]
    AtmosphereSnow8Bits,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraBlend"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Blend")]
    BlendToCameraBlend,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraBlueScreen"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_BlueScreen")]
    BlendToCameraBlueScreen,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Color")]
    BlendToCameraColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraColorBurn"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_ColorBurn")]
    BlendToCameraColorBurn,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraColorDodge"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_ColorDodge")]
    BlendToCameraColorDodge,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraDarken"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Darken")]
    BlendToCameraDarken,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraDarkerColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_DarkerColor")]
    BlendToCameraDarkerColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraDifference"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Difference")]
    BlendToCameraDifference,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraDivide"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Divide")]
    BlendToCameraDivide,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraExclusion"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Exclusion")]
    BlendToCameraExclusion,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraGreenScreen"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_GreenScreen")]
    BlendToCameraGreenScreen,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraHardLight"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_HardLight")]
    BlendToCameraHardLight,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraHardMix"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_HardMix")]
    BlendToCameraHardMix,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraHue"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Hue")]
    BlendToCameraHue,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraLighten"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Lighten")]
    BlendToCameraLighten,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraLighterColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_LighterColor")]
    BlendToCameraLighterColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraLinearBurn"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_LinearBurn")]
    BlendToCameraLinearBurn,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraLinearDodge"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_LinearDodge")]
    BlendToCameraLinearDodge,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraLinearLight"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_LinearLight")]
    BlendToCameraLinearLight,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraLuminosity"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Luminosity")]
    BlendToCameraLuminosity,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraMultiply"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Multiply")]
    BlendToCameraMultiply,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraOverlay"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Overlay")]
    BlendToCameraOverlay,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraPhotoshopFilters"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_PhotoshopFilters")]
    BlendToCameraPhotoshopFilters,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraPinLight"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_PinLight")]
    BlendToCameraPinLight,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraSaturation"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Saturation")]
    BlendToCameraSaturation,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraScreen"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Screen")]
    BlendToCameraScreen,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraSoftLight"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_SoftLight")]
    BlendToCameraSoftLight,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraSplitScreen"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_SplitScreen")]
    BlendToCameraSplitScreen,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraSubtract"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_Subtract")]
    BlendToCameraSubtract,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlendToCameraVividLight"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blend2Camera_VividLight")]
    BlendToCameraVividLight,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.Blizzard"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blizzard")]
    Blizzard,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurBloom"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Bloom")]
    BlurBloom,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurBlurHole"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_BlurHole")]
    BlurBlurHole,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurBlurry"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Blurry")]
    BlurBlurry,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurDitheringToxTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Dithering2x2")]
    BlurDitheringToxTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurDitherOffset"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_DitherOffset")]
    BlurDitherOffset,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurFocus"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Focus")]
    BlurFocus,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurGaussianBlur"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_GaussianBlur")]
    BlurGaussianBlur,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurMovie"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Movie")]
    BlurMovie,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurNoise"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Noise")]
    BlurNoise,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurRadial"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Radial")]
    BlurRadial,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurRadialFast"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Radial_Fast")]
    BlurRadialFast,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurRegular"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Regular")]
    BlurRegular,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurSteam"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Steam")]
    BlurSteam,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurTiltShift"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Tilt_Shift")]
    BlurTiltShift,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurTiltShiftHole"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Tilt_Shift_Hole")]
    BlurTiltShiftHole,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.BlurTiltShiftV"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Blur_Tilt_Shift_V")]
    BlurTiltShiftV,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorBrightContrastSaturation"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_BrightContrastSaturation")]
    ColorBrightContrastSaturation,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorChromaticAberration"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_Chromatic_Aberration")]
    ColorChromaticAberration,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorContrast"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_Contrast")]
    ColorContrast,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorGrayScale"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_GrayScale")]
    ColorGrayScale,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorInvert"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_Invert")]
    ColorInvert,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorNoise"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_Noise")]
    ColorNoise,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorRgb"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_RGB")]
    ColorRgb,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsAdjustColorRGB"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_Adjust_ColorRGB")]
    ColorsAdjustColorRGB,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsAdjustFullColors"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_Adjust_FullColors")]
    ColorsAdjustFullColors,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsAdjustPreFilters"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_Adjust_PreFilters")]
    ColorsAdjustPreFilters,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsBleachBypass"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_BleachBypass")]
    ColorsBleachBypass,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsBrightness"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_Brightness")]
    ColorsBrightness,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsDarkColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_DarkColor")]
    ColorsDarkColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorSepia"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_Sepia")]
    ColorSepia,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsHsv"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_HSV")]
    ColorsHsv,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsHueRotate"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_HUE_Rotate")]
    ColorsHueRotate,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsNewPosterize"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_NewPosterize")]
    ColorsNewPosterize,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsRgbClamp"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_RgbClamp")]
    ColorsRgbClamp,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorsThreshold"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Colors_Threshold")]
    ColorsThreshold,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorSwitching"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_Switching")]
    ColorSwitching,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ColorYuv"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Color_YUV")]
    ColorYuv,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionAspiration"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Aspiration")]
    DistortionAspiration,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionBigFace"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_BigFace")]
    DistortionBigFace,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionBlackHole"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_BlackHole")]
    DistortionBlackHole,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionDissipation"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Dissipation")]
    DistortionDissipation,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionDream"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Dream")]
    DistortionDream,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionDreamTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Dream2")]
    DistortionDreamTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionFishEye"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_FishEye")]
    DistortionFishEye,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionFlag"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Flag")]
    DistortionFlag,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionFlush"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Flush")]
    DistortionFlush,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionHalfSphere"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Half_Sphere")]
    DistortionHalfSphere,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionHeat"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Heat")]
    DistortionHeat,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionLens"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Lens")]
    DistortionLens,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionNoise"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Noise")]
    DistortionNoise,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionShockWave"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_ShockWave")]
    DistortionShockWave,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionTwist"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Twist")]
    DistortionTwist,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionTwistSquare"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Twist_Square")]
    DistortionTwistSquare,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionWaterDrop"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Water_Drop")]
    DistortionWaterDrop,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DistortionWaveHorizontal"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Distortion_Wave_Horizontal")]
    DistortionWaveHorizontal,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingBluePrint"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_BluePrint")]
    DrawingBluePrint,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingCellShading"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_CellShading")]
    DrawingCellShading,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingCellShadingTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_CellShading2")]
    DrawingCellShadingTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingComics"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Comics")]
    DrawingComics,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingCrosshatch"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Crosshatch")]
    DrawingCrosshatch,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingCurve"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Curve")]
    DrawingCurve,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingEnhancedComics"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_EnhancedComics")]
    DrawingEnhancedComics,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingHalftone"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Halftone")]
    DrawingHalftone,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingLaplacian"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Laplacian")]
    DrawingLaplacian,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingLines"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Lines")]
    DrawingLines,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingManga"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga")]
    DrawingManga,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingManga3"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga3")]
    DrawingManga3,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingManga4"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga4")]
    DrawingManga4,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingManga5"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga5")]
    DrawingManga5,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingMangaColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga_Color")]
    DrawingMangaColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingMangaFlash"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga_Flash")]
    DrawingMangaFlash,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingMangaFlashColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga_Flash_Color")]
    DrawingMangaFlashColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingMangaFlashWhite"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga_FlashWhite")]
    DrawingMangaFlashWhite,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingMangaTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Manga2")]
    DrawingMangaTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingNewCellShading"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_NewCellShading")]
    DrawingNewCellShading,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingPaper"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Paper")]
    DrawingPaper,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingPaper3"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Paper3")]
    DrawingPaper3,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingPaperTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Paper2")]
    DrawingPaperTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.DrawingToon"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Drawing_Toon")]
    DrawingToon,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EdgeBlackLine"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Edge_BlackLine")]
    EdgeBlackLine,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EdgeEdgeFilter"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Edge_Edge_filter")]
    EdgeEdgeFilter,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EdgeGolden"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Edge_Golden")]
    EdgeGolden,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EdgeNeon"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Edge_Neon")]
    EdgeNeon,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EdgeSigmoid"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Edge_Sigmoid")]
    EdgeSigmoid,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EdgeSobel"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Edge_Sobel")]
    EdgeSobel,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.ExtraRotation"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_EXTRA_Rotation")]
    ExtraRotation,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EyesVision1"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_EyesVision_1")]
    EyesVision1,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.EyesVisionTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_EyesVision_2")]
    EyesVisionTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FilmColorPerfection"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Film_ColorPerfection")]
    FilmColorPerfection,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FilmGrain"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Film_Grain")]
    FilmGrain,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FlipScreen"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FlipScreen")]
    FlipScreen,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FlyVision"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Fly_Vision")]
    FlyVision,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.Fx8Bits"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_8bits")]
    Fx8Bits,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.Fx8BitsGb"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_8bits_gb")]
    Fx8BitsGb,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxAscii"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Ascii")]
    FxAscii,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxDarkMatter"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_DarkMatter")]
    FxDarkMatter,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxDigitalMatrix"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_DigitalMatrix")]
    FxDigitalMatrix,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxDigitalMatrixDistortion"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_DigitalMatrixDistortion")]
    FxDigitalMatrixDistortion,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxDotCircle"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Dot_Circle")]
    FxDotCircle,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxDrunk"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Drunk")]
    FxDrunk,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxDrunkTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Drunk2")]
    FxDrunkTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxEarthQuake"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_EarthQuake")]
    FxEarthQuake,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxFunk"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Funk")]
    FxFunk,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxGlitch1"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Glitch1")]
    FxGlitch1,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxGlitch3"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Glitch3")]
    FxGlitch3,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxGlitchTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Glitch2")]
    FxGlitchTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxGrid"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Grid")]
    FxGrid,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxHexagon"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Hexagon")]
    FxHexagon,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxHexagonBlack"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Hexagon_Black")]
    FxHexagonBlack,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxHypno"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Hypno")]
    FxHypno,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxInverChromiLum"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_InverChromiLum")]
    FxInverChromiLum,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxMirror"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Mirror")]
    FxMirror,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxPlasma"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Plasma")]
    FxPlasma,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxPsycho"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Psycho")]
    FxPsycho,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxScan"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Scan")]
    FxScan,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxScreens"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Screens")]
    FxScreens,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxSpot"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_Spot")]
    FxSpot,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxSuperDot"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_superDot")]
    FxSuperDot,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.FxZebraColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_FX_ZebraColor")]
    FxZebraColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GlitchMozaic"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Glitch_Mozaic")]
    GlitchMozaic,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GlowGlow"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Glow_Glow")]
    GlowGlow,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GlowGlowColor"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Glow_Glow_Color")]
    GlowGlowColor,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsAnsi"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Ansi")]
    GradientsAnsi,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsDesert"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Desert")]
    GradientsDesert,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsElectricGradient"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_ElectricGradient")]
    GradientsElectricGradient,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsFireGradient"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_FireGradient")]
    GradientsFireGradient,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsHue"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Hue")]
    GradientsHue,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsNeonGradient"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_NeonGradient")]
    GradientsNeonGradient,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsRainbow"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Rainbow")]
    GradientsRainbow,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsStripe"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Stripe")]
    GradientsStripe,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsTech"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Tech")]
    GradientsTech,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.GradientsTherma"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Gradients_Therma")]
    GradientsTherma,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.LightRainbow"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Light_Rainbow")]
    LightRainbow,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.LightRainbowTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Light_Rainbow2")]
    LightRainbowTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.LightWater"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Light_Water")]
    LightWater,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.LightWaterTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Light_Water2")]
    LightWaterTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.NightVision4"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_NightVision_4")]
    NightVision4,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.NightVisionFX"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_NightVisionFX")]
    NightVisionFX,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.NoiseTv"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Noise_TV")]
    NoiseTv,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.NoiseTv3"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Noise_TV_3")]
    NoiseTv3,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.NoiseTvTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Noise_TV_2")]
    NoiseTvTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OculusNightVision1"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Oculus_NightVision1")]
    OculusNightVision1,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OculusNightVision3"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Oculus_NightVision3")]
    OculusNightVision3,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OculusNightVision5"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Oculus_NightVision5")]
    OculusNightVision5,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OculusNightVisionTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Oculus_NightVision2")]
    OculusNightVisionTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OculusThermaVision"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Oculus_ThermaVision")]
    OculusThermaVision,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OldFilmCutting1"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_OldFilm_Cutting1")]
    OldFilmCutting1,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.OldFilmCuttingTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_OldFilm_Cutting2")]
    OldFilmCutting2,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.PixelisationDot"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Pixelisation_Dot")]
    PixelisationDot,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.PixelisationOilPaint"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Pixelisation_OilPaint")]
    PixelisationOilPaint,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.PixelisationOilPaintHQ"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Pixelisation_OilPaintHQ")]
    PixelisationOilPaintHQ,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.PixelPixelisation"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Pixel_Pixelisation")]
    PixelPixelisation,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.RealVhs"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Real_VHS")]
    RealVhs,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.RetroLoading"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Retro_Loading")]
    RetroLoading,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.SharpenSharpen"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Sharpen_Sharpen")]
    SharpenSharpen,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.SpecialBubble"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Special_Bubble")]
    SpecialBubble,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.Tv50"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_50")]
    Tv50,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.Tv80"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_80")]
    Tv80,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvArcade"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_ARCADE")]
    TvArcade,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvArcade3"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_ARCADE_3")]
    TvArcade3,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvArcadeFast"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_ARCADE_Fast")]
    TvArcadeFast,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvArcadeTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_ARCADE_2")]
    TvArcadeTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvArtefact"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Artefact")]
    TvArtefact,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvBrokenGlass"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_BrokenGlass")]
    TvBrokenGlass,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvBrokenGlassTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_BrokenGlass2")]
    TvBrokenGlassTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvChromatical"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Chromatical")]
    TvChromatical,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvChromaticalTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Chromatical2")]
    TvChromaticalTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvCompressionFX"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_CompressionFX")]
    TvCompressionFX,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvDistorted"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Distorted")]
    TvDistorted,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvHorror"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Horror")]
    TvHorror,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvLed"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_LED")]
    TvLed,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvNoise"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Noise")]
    TvNoise,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvOld"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Old")]
    TvOld,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvOldMovie"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Old_Movie")]
    TvOldMovie,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvOldMovieTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Old_Movie_2")]
    TvOldMovieTo,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvPlanetMars"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_PlanetMars")]
    TvPlanetMars,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvPosterize"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Posterize")]
    TvPosterize,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvRgb"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Rgb")]
    TvRgb,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvTiles"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Tiles")]
    TvTiles,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvVcr"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Vcr")]
    TvVcr,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvVhs"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_VHS")]
    TvVhs,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvVhsRewind"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_VHS_Rewind")]
    TvVhsRewind,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvVideo3D"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Video3D")]
    TvVideo3D,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvVideoflip"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Videoflip")]
    TvVideoflip,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvVintage"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_Vintage")]
    TvVintage,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvWideScreenCircle"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_WideScreenCircle")]
    TvWideScreenCircle,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvWideScreenHorizontal"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_WideScreenHorizontal")]
    TvWideScreenHorizontal,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvWideScreenHV"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_WideScreenHV")]
    TvWideScreenHV,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.TvWideScreenVertical"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_TV_WideScreenVertical")]
    TvWideScreenVertical,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VhsTracking"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_VHS_Tracking")]
    VhsTracking,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionAura"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Aura")]
    VisionAura,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionAuraDistortion"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_AuraDistortion")]
    VisionAuraDistortion,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionCrystal"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Crystal")]
    VisionCrystal,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionDrost"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Drost")]
    VisionDrost,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionPlasma"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Plasma")]
    VisionPlasma,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionPsycho"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Psycho")]
    VisionPsycho,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionRainbow"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Rainbow")]
    VisionRainbow,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionTunnel"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Tunnel")]
    VisionTunnel,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionWarp"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Warp")]
    VisionWarp,
    /// <summary>
    /// The filter type of <see cref="RhythmBase.Adofal.Components.Filters.VisionWarpTo"/>.
    /// </summary>
    [JsonAlias("CameraFilterPack_Vision_Warp2")]
    VisionWarpTo,
}
