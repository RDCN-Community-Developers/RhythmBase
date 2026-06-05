using RhythmBase.RhythmDoctor.Events;

namespace RhythmBase.RhythmDoctor.Converters;

/// <summary>  
/// Utility class for converting between event types and enumerations.  
/// </summary>  
partial class EventTypeRegistry
{
    /// <summary>
    /// All event types.
    /// </summary>
    public static readonly ReadOnlyEnumCollection<EventType> Types = ToEnums<IBaseEvent>();
    /// <summary>  
    /// Event types that inherit from <see cref="T:RhythmBase.Events.BaseRowAction" />.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> RowTypes = ToEnums<BaseRowAction>();
    /// <summary>  
    /// Event types that inherit from <see cref="T:RhythmBase.Events.BaseDecorationAction" />.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> DecorationTypes = ToEnums<BaseDecorationAction>();
    /// <summary>  
    /// Custom event types.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> CustomTypes = new(
        EventType.ForwardEvent,
        EventType.ForwardRowEvent,
        EventType.ForwardDecorationEvent
    );
    /// <summary>  
    /// Event types for gameplay.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForGameplay = new(
        EventType.HideRow,
        EventType.ChangePlayersRows,
        EventType.FinishLevel,
        EventType.ShowHands,
        EventType.SetHandOwner,
        EventType.SetPlayStyle
    );
    /// <summary>  
    /// Event types for environment.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForEnvironment = new(
        EventType.SetTheme,
        EventType.SetBackgroundColor,
        EventType.SetForeground,
        EventType.SetSpeed,
        EventType.Flash,
        EventType.CustomFlash
        );
    /// <summary>  
    /// Event types for row effects.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForRowFX = new(
        EventType.HideRow,
        EventType.MoveRow,
        EventType.PlayExpression,
        EventType.TintRows
    );
    /// <summary>  
    /// Event types for camera effects.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForCameraFX = new(
        EventType.MoveCamera,
        EventType.ShakeScreen,
        EventType.FlipScreen,
        EventType.PulseCamera
    );
    /// <summary>  
    /// Event types for visual effects.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForVisualFX = new(
        EventType.SetVFXPreset,
        EventType.SetSpeed,
        EventType.Flash,
        EventType.CustomFlash,
        EventType.BassDrop,
        EventType.InvertColors,
        EventType.Stutter,
        EventType.PaintHands,
        EventType.NewWindowDance
    );
    /// <summary>  
    /// Event types for text effects.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForText = new(
        EventType.TextExplosion,
        EventType.ShowDialogue,
        EventType.ShowStatusSign,
        EventType.FloatingText,
        EventType.AdvanceText
    );
    /// <summary>  
    /// Event types for utility actions.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForUtility = new(
        EventType.Comment,
        EventType.TagAction,
        EventType.CallCustomMethod
    );

    /// <summary>  
    /// Event types that inherit from classic row actions.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForRowClassic = new(
         EventType.AddClassicBeat,
         EventType.AddFreeTimeBeat,
         EventType.PulseFreeTimeBeat,
         EventType.SetRowXs
    );
    /// <summary>  
    /// Event types that inherit from oneshot row actions.  
    /// </summary>  
    public static readonly ReadOnlyEnumCollection<EventType> EventTypeEnumsForRowOneshot = new(
         EventType.AddOneshotBeat,
         EventType.SetOneshotWave
    );
}