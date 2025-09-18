// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI.LayoutEngine
{
    /// <summary>
    /// Contains constants and helper methods for creating AbsoluteUnits and RelativeUnits.
    /// </summary>
    public static class UnitValue
    {
        public static readonly AbsoluteUnit ZeroPixels = new AbsoluteUnit(AbsoluteUnits.Pixels, 0);
        public static readonly RelativeUnit Auto = new RelativeUnit(RelativeUnits.Auto);
        public static readonly RelativeUnit StretchOne = new RelativeUnit(RelativeUnits.Stretch, 1);

        /// <summary>
        /// Creates a Pixel unit value.
        /// </summary>
        /// <param name="value">Size in pixels</param>
        public static AbsoluteUnit Pixels(double value) => new AbsoluteUnit(AbsoluteUnits.Pixels, value);

        /// <summary>
        /// Creates a Points unit value.
        /// </summary>
        /// <param name="value">Size in points</param>
        public static AbsoluteUnit Points(double value) => new AbsoluteUnit(AbsoluteUnits.Points, value);

        /// <summary>
        /// Creates a Stretch unit value with the specified factor.
        /// </summary>
        /// <param name="factor">Stretch factor (relative to other stretch elements)</param>
        public static RelativeUnit Stretch(double factor = 1f) => new RelativeUnit(RelativeUnits.Stretch, factor);

        /// <summary>
        /// Creates a Percentage unit value.
        /// </summary>
        /// <param name="value">Percentage value (0-100)</param>
        /// <param name="offset">Additional pixel offset</param>
        public static RelativeUnit Percentage(double value, double offset = 0f) => new RelativeUnit(RelativeUnits.Percentage, value, offset);
    }
}
