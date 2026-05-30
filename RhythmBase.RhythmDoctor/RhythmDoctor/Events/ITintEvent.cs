using RhythmBase.RhythmDoctor.Components;

namespace RhythmBase.RhythmDoctor.Events;

internal interface ITintEvent : IColorEvent
{
    Border? Border { get; set; }
    PaletteColorWithAlpha BorderColor { get; set; }
    bool BorderPulse { get; set; }
    float BorderPulseMin { get; set; }
    float BorderPulseMax { get; set; }
    int? Opacity { get; set; }
    bool? IsTint { get; set; }
    PaletteColorWithAlpha TintColor { get; set; }
}
