// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;

namespace Prowl.PaperUI
{
    /// <summary>
    /// Defines how much rounding is applied to an element.
    /// </summary>
    public struct Rounding
    {
        public AbsoluteUnit TopLeft;
        public AbsoluteUnit TopRight;
        public AbsoluteUnit BottomRight;
        public AbsoluteUnit BottomLeft;

        public Rounding(in AbsoluteUnit topLeft, in AbsoluteUnit topRight, in AbsoluteUnit bottomRight, in AbsoluteUnit bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        public Vector4 ToPx(in ScalingSettings scalingSettings)
        {
            return new Vector4(
                TopLeft.ToPx(scalingSettings), TopRight.ToPx(scalingSettings),
                BottomRight.ToPx(scalingSettings), BottomLeft.ToPx(scalingSettings));
        }

        /// <summary>
        /// Linearly interpolates between two Rounding instances.
        /// </summary>
        /// <param name="a">Starting value</param>
        /// <param name="b">Ending value</param>
        /// <param name="blendFactor">Interpolation factor (0.0 to 1.0)</param>
        /// <returns>Interpolated Rounding</returns>
        public static Rounding Lerp(in Rounding a, in Rounding b, double blendFactor)
        {
            return new Rounding(
                AbsoluteUnit.Lerp(a.TopLeft, b.TopLeft, blendFactor),
                AbsoluteUnit.Lerp(a.TopRight, b.TopRight, blendFactor),
                AbsoluteUnit.Lerp(a.BottomRight, b.BottomRight, blendFactor),
                AbsoluteUnit.Lerp(a.BottomLeft, b.BottomLeft, blendFactor));
        }
    }
}
