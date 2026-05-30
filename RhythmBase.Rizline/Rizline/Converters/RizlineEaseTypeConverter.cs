using RhythmBase.Global.Components.Easing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhythmBase.Rizline.Converters;

internal class RizlineEaseTypeConverter : JsonConverter<EaseType>
{
    public override EaseType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int rizlineEaseType = reader.GetInt32();
        return ToEaseType(rizlineEaseType);
    }
    public override void Write(Utf8JsonWriter writer, EaseType value, JsonSerializerOptions options)
    {
        int rizlineEaseType = ToRizlineEaseType(value);
        writer.WriteNumberValue(rizlineEaseType);
    }
    private static int ToRizlineEaseType(EaseType easeType) => Array.IndexOf(_easeTypesMap, easeType);
    private static EaseType ToEaseType(int rizlineEaseType) =>
        rizlineEaseType >= 0 && rizlineEaseType < _easeTypesMap.Length
            ? _easeTypesMap[rizlineEaseType]
            : throw new ArgumentOutOfRangeException(nameof(rizlineEaseType));
    private static readonly EaseType[] _easeTypesMap =
    [
        EaseType.Linear,
        EaseType.InQuad,
        EaseType.OutQuad,
        EaseType.InOutQuad,
        EaseType.InCubic,
        EaseType.OutCubic,
        EaseType.InOutCubic,
        EaseType.InQuart,
        EaseType.OutQuart,
        EaseType.InOutQuart,
        EaseType.InQuint,
        EaseType.OutQuint,
        EaseType.InOutQuint,
        EaseType.Unset,
        EaseType.One,
        EaseType.InCirc,
        EaseType.OutCirc,
        EaseType.OutSine,
        EaseType.InSine,
    ];
}