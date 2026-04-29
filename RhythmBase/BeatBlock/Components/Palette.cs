using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Components
{
    public struct Palette()
    {
        private readonly RDColor[] colors = new RDColor[8];
        public RDColor this[int index]
        {
            get => colors[index];
            set => colors[index] = value;
        }
        public static Palette Default => new()
        {
            [0] = White,
            [1] = Black,
            [2] = Red,
            [3] = Blue,
            [4] = Green,
            [5] = Yellow,
            [6] = Pink,
            [7] = Cyan,
        };
        public static RDColor Black => 0xFF000000;
        public static RDColor Red => 0xFFFF0000;
        public static RDColor Blue => 0xFF0000FF;
        public static RDColor Grey => 0xFFD4D4D4;
        public static RDColor Gray => 0xFFD4D4D4;
        public static RDColor White => 0xFFFFFFFF;
        public static RDColor Pink => 0xFFFF00FF;
        public static RDColor Magenta => 0xFFFF00FF;
        public static RDColor Green => 0xFF00FF00;
        public static RDColor Yellow => 0xFFFFFF00;
        public static RDColor Cyan => 0xFF00FFFF;
    }
}
