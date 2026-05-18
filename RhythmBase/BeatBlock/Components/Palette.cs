using System;
using System.Collections.Generic;
using System.Text;

namespace RhythmBase.BeatBlock.Components
{
    /// <summary>
    /// Represents a color palette containing up to 8 colors.
    /// </summary>
    public struct Palette()
    {
        private readonly RDColor[] colors = new RDColor[8];
        /// <summary>
        /// Gets or sets the color at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the color.</param>
        /// <returns>The <see cref="RDColor"/> at the specified index.</returns>
        public RDColor this[int index]
        {
            get => colors[index];
            set => colors[index] = value;
        }
        /// <summary>
        /// Gets the default palette.
        /// </summary>
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
        /// <summary>
        /// Gets the black color.
        /// </summary>
        public static RDColor Black => 0xFF000000;
        /// <summary>
        /// Gets the red color.
        /// </summary>
        public static RDColor Red => 0xFFFF0000;
        /// <summary>
        /// Gets the blue color.
        /// </summary>
        public static RDColor Blue => 0xFF0000FF;
        /// <summary>
        /// Gets the grey color.
        /// </summary>
        public static RDColor Grey => 0xFFD4D4D4;
        /// <summary>
        /// Gets the gray color.
        /// </summary>
        public static RDColor Gray => 0xFFD4D4D4;
        /// <summary>
        /// Gets the white color.
        /// </summary>
        public static RDColor White => 0xFFFFFFFF;
        /// <summary>
        /// Gets the pink color.
        /// </summary>
        public static RDColor Pink => 0xFFFF00FF;
        /// <summary>
        /// Gets the magenta color.
        /// </summary>
        public static RDColor Magenta => 0xFFFF00FF;
        /// <summary>
        /// Gets the green color.
        /// </summary>
        public static RDColor Green => 0xFF00FF00;
        /// <summary>
        /// Gets the yellow color.
        /// </summary>
        public static RDColor Yellow => 0xFFFFFF00;
        /// <summary>
        /// Gets the cyan color.
        /// </summary>
        public static RDColor Cyan => 0xFF00FFFF;
    }
}
