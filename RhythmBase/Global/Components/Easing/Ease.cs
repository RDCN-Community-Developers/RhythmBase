namespace RhythmBase.Global.Components.Easing;

///<summary>
/// The EaseType enumeration represents various types of easing functions.
/// These functions are used to create smooth transitions in animations.
///</summary>
[JsonEnumSerializable]
public enum EaseType
{
    /// <summary>
    /// Unset.
    /// </summary>
    Unset = -1,
    ///<summary>
    ///Ease Linear.
    ///</summary>
    Linear,
    ///<summary>
    ///Ease InSine.
    ///</summary>
    InSine,
    ///<summary>
    ///Ease OutSine.
    ///</summary>
    OutSine,
    ///<summary>
    ///Ease InOutSine.
    ///</summary>
    InOutSine,
    ///<summary>
    ///Ease InQuad.
    ///</summary>
    InQuad,
    ///<summary>
    ///Ease OutQuad.
    ///</summary>
    OutQuad,
    ///<summary>
    ///Ease InOutQuad.
    ///</summary>
    InOutQuad,
    ///<summary>
    ///Ease InCubic.
    ///</summary>
    InCubic,
    ///<summary>
    ///Ease OutCubic.
    ///</summary>
    OutCubic,
    ///<summary>
    ///Ease InOutCubic.
    ///</summary>
    InOutCubic,
    ///<summary>
    ///Ease InQuart.
    ///</summary>
    InQuart,
    ///<summary>
    ///Ease OutQuart.
    ///</summary>
    OutQuart,
    ///<summary>
    ///Ease InOutQuart.
    ///</summary>
    InOutQuart,
    ///<summary>
    ///Ease InQuint.
    ///</summary>
    InQuint,
    ///<summary>
    ///Ease OutQuint.
    ///</summary>
    OutQuint,
    ///<summary>
    ///Ease InOutQuint.
    ///</summary>
    InOutQuint,
    ///<summary>
    ///Ease InExpo.
    ///</summary>
    InExpo,
    ///<summary>
    ///Ease OutExpo.
    ///</summary>
    OutExpo,
    ///<summary>
    ///Ease InOutExpo.
    ///</summary>
    InOutExpo,
    ///<summary>
    ///Ease InCirc.
    ///</summary>
    InCirc,
    ///<summary>
    ///Ease OutCirc.
    ///</summary>
    OutCirc,
    ///<summary>
    ///Ease InOutCirc.
    ///</summary>
    InOutCirc,
    ///<summary>
    ///Ease InElastic.
    ///</summary>
    InElastic,
    ///<summary>
    ///Ease OutElastic.
    ///</summary>
    OutElastic,
    ///<summary>
    ///Ease InOutElastic.
    ///</summary>
    InOutElastic,
    ///<summary>
    ///Ease InBack.
    ///</summary>
    InBack,
    ///<summary>
    ///Ease OutBack.
    ///</summary>
    OutBack,
    ///<summary>
    ///Ease InOutBack.
    ///</summary>
    InOutBack,
    ///<summary>
    ///Ease InBounce.
    ///</summary>
    InBounce,
    ///<summary>
    ///Ease OutBounce.
    ///</summary>
    OutBounce,
    ///<summary>
    ///Ease InOutBounce.
    ///</summary>
    InOutBounce,
    ///<summary>
    ///Ease SmoothStep.
    ///</summary>
    SmoothStep,
    /// <summary>
    /// One.
    /// </summary>
    One,
}
/// <summary>
/// Ease Calculate module.
/// </summary>
public static class Ease
{
    /// <summary>
    /// Calculates the value with the specified ease type.
    /// </summary>
    /// <param name="type">Ease type.</param>
    /// <param name="x">A doubleing-point number in the range of 0 to 1.</param>
    /// <returns>Easing result.</returns>
    public static double Calculate(this EaseType type, double x) => type switch
    {
        EaseType.Unset => EaseFunction.None(x),
        EaseType.One => EaseFunction.One(x),
        EaseType.Linear => EaseFunction.Linear(x),
        EaseType.InSine => EaseFunction.InSine(x),
        EaseType.OutSine => EaseFunction.OutSine(x),
        EaseType.InOutSine => EaseFunction.InOutSine(x),
        EaseType.InQuad => EaseFunction.InQuad(x),
        EaseType.OutQuad => EaseFunction.OutQuad(x),
        EaseType.InOutQuad => EaseFunction.InOutQuad(x),
        EaseType.InCubic => EaseFunction.InCubic(x),
        EaseType.OutCubic => EaseFunction.OutCubic(x),
        EaseType.InOutCubic => EaseFunction.InOutCubic(x),
        EaseType.InQuart => EaseFunction.InQuart(x),
        EaseType.OutQuart => EaseFunction.OutQuart(x),
        EaseType.InOutQuart => EaseFunction.InOutQuart(x),
        EaseType.InQuint => EaseFunction.InQuint(x),
        EaseType.OutQuint => EaseFunction.OutQuint(x),
        EaseType.InOutQuint => EaseFunction.InOutQuint(x),
        EaseType.InExpo => EaseFunction.InExpo(x),
        EaseType.OutExpo => EaseFunction.OutExpo(x),
        EaseType.InOutExpo => EaseFunction.InOutExpo(x),
        EaseType.InCirc => EaseFunction.InCirc(x),
        EaseType.OutCirc => EaseFunction.OutCirc(x),
        EaseType.InOutCirc => EaseFunction.InOutCirc(x),
        EaseType.InElastic => EaseFunction.InElastic(x),
        EaseType.OutElastic => EaseFunction.OutElastic(x),
        EaseType.InOutElastic => EaseFunction.InOutElastic(x),
        EaseType.InBack => EaseFunction.InBack(x),
        EaseType.OutBack => EaseFunction.OutBack(x),
        EaseType.InOutBack => EaseFunction.InOutBack(x),
        EaseType.InBounce => EaseFunction.InBounce(x),
        EaseType.OutBounce => EaseFunction.OutBounce(x),
        EaseType.InOutBounce => EaseFunction.InOutBounce(x),
        EaseType.SmoothStep => EaseFunction.SmoothStep(x),
        _ => 0,
    };
    /// <summary>
    /// Calculates the value with the specified ease type.
    /// </summary>
    /// <param name="Type">Ease type.</param>
    /// <param name="x">A doubleing-point number in the range of 0 to 1.</param>
    /// <param name="from">The starting value of the easing result.</param>
    /// <param name="to">The endding value of the easing result</param>
    /// <returns>Easing result.</returns>
    public static double Calculate(this EaseType Type, double x, double from, double to) => Type.Calculate(x) * (to - from) + from;
    /// <summary>
    /// Ease types.
    /// </summary>
    private static class EaseFunction
    {
        private const double c1 = 1.525;
        private const double c2 = 1.70158;
        public static double None(double _) => 0;
        public static double One(double _) => 1;
        public static double Linear(double x) => x;
        public static double InSine(double x) => 1 - Math.Cos(x * Math.PI / 2);
        public static double OutSine(double x) => Math.Sin(x * Math.PI / 2);
        public static double InOutSine(double x) => -(Math.Cos(x * Math.PI) - 1) / 2;
        public static double InQuad(double x) => Math.Pow(x, 2);
        public static double OutQuad(double x) => 1 - Math.Pow(1 - x, 2);
        public static double InOutQuad(double x) => x < 0.5 ? 2 * Math.Pow(x, 2) : 1 - Math.Pow(-2 * x + 2, 2) / 2;
        public static double InCubic(double x) => Math.Pow(x, 3);
        public static double OutCubic(double x) => 1 - Math.Pow(1 - x, 3);
        public static double InOutCubic(double x) => x < 0.5 ? 4 * Math.Pow(x, 3) : 1 - Math.Pow(-2 * x + 2, 3) / 2;
        public static double InQuart(double x) => Math.Pow(x, 4);
        public static double OutQuart(double x) => 1 - Math.Pow(1 - x, 4);
        public static double InOutQuart(double x) => x < 0.5 ? 8 * Math.Pow(x, 4) : 1 - Math.Pow(-2 * x + 2, 4) / 2;
        public static double InQuint(double x) => Math.Pow(x, 5);
        public static double OutQuint(double x) => 1 - Math.Pow(1 - x, 5);
        public static double InOutQuint(double x) => x < 0.5 ? 16 * Math.Pow(x, 5) : 1 - Math.Pow(-2 * x + 2, 5) / 2;
        public static double InExpo(double x) => x == 0 ? 0 : Math.Pow(2, 10 * x - 10);
        public static double OutExpo(double x) => x == 1 ? 1 : 1 - Math.Pow(2, -10 * x);
        public static double InOutExpo(double x) => x == 0
            ? 0
            : x == 1
            ? 1
            : x < 0.5 ? Math.Pow(2, 20 * x - 10) / 2
            : (2 - Math.Pow(2, -20 * x + 10)) / 2;
        public static double InCirc(double x) => 1 - Math.Sqrt(1 - Math.Pow(x, 2));
        public static double OutCirc(double x) => Math.Sqrt(1 - Math.Pow(x - 1, 2));
        public static double InOutCirc(double x) => x < 0.5
            ? (1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2
            : (Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
        public static double InElastic(double x) => x == 0
            ? 0
            : x == 1
            ? 1
            : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * (2 * Math.PI / 3));
        public static double OutElastic(double x) => x == 0
            ? 0
            : x == 1
            ? 1
            : Math.Pow(2, -10 * x) * Math.Sin((x * 10 - 0.75) * (2 * Math.PI / 3)) + 1;
        public static double InOutElastic(double x) => x == 0
            ? 0
            : x == 1
            ? 1
            : x < 0.5
            ? -(Math.Pow(2, 20 * x - 10) * Math.Sin((20 * x - 11.125) * (2 * Math.PI / 4.5))) / 2
            : Math.Pow(2, -20 * x + 10) * Math.Sin((20 * x - 11.125) * (2 * Math.PI / 4.5)) / 2 + 1;
        public static double InBack(double x) => (c2 + 1) * x * x * x - c2 * x * x;
        public static double OutBack(double x) => 1 + (c2 + 1) * Math.Pow(x - 1, 3) + c2 * Math.Pow(x - 1, 2);
        public static double InOutBack(double x) => x < 0.5
            ? Math.Pow(2 * x, 2) * ((c2 * c1 + 1) * 2 * x - c2 * c1) / 2
            : (Math.Pow(2 * x - 2, 2) * ((c2 * c1 + 1) * (x * 2 - 2) + c2 * c1) + 2) / 2;
        public static double InBounce(double x) =>
          1 - OutBounce(1 - x);
        public static double OutBounce(double x)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;
            return x switch
            {
                < 1 / d1 => n1 * x * x,
                < 2 / d1 => n1 * (x -= 1.5 / d1) * x + 0.75,
                < 2.5 / d1 => n1 * (x -= 2.25 / d1) * x + 0.9375,
                _ => n1 * (x -= 2.625 / d1) * x + 0.984375,
            };
        }
        public static double InOutBounce(double x) =>
          x < 0.5
          ? (1 - OutBounce(1 - 2 * x)) / 2
          : (1 + OutBounce(2 * x - 1)) / 2;
        public static double SmoothStep(double x) =>
          (3 - 2 * x) * Math.Pow(x, 2);
    }
}
