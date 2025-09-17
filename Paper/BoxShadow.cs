using System.Drawing;

using Prowl.PaperUI.LayoutEngine;

namespace Prowl.PaperUI
{
    /// <summary>
    /// Represents a simple box shadow similar to CSS box-shadow.
    /// </summary>
    public struct BoxShadow
    {
        public AbsoluteUnit OffsetX;
        public AbsoluteUnit OffsetY;
        public AbsoluteUnit Blur;
        public AbsoluteUnit Spread;
        public Color Color;

        public BoxShadow(AbsoluteUnit offsetX, AbsoluteUnit offsetY, AbsoluteUnit blur, AbsoluteUnit spread, Color color)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Blur = blur;
            Spread = spread;
            Color = color;
        }

        /// <summary>Default empty shadow.</summary>
        public static readonly BoxShadow None = new BoxShadow(0, 0, 0, 0, Color.Transparent);

        /// <summary>Returns true if the shadow has a visible effect.</summary>
        public bool IsVisible => Color.A > 0;

        /// <summary>
        /// Interpolates between two box shadows.
        /// </summary>
        public static BoxShadow Lerp(BoxShadow start, BoxShadow end, double t)
        {
            Color LerpColor(Color a, Color b)
            {
                int r = (int)(a.R + (b.R - a.R) * t);
                int g = (int)(a.G + (b.G - a.G) * t);
                int bVal = (int)(a.B + (b.B - a.B) * t);
                int aVal = (int)(a.A + (b.A - a.A) * t);
                return Color.FromArgb(aVal, r, g, bVal);
            }

            return new BoxShadow(
                AbsoluteUnit.Lerp(start.OffsetX, end.OffsetX, t),
                AbsoluteUnit.Lerp(start.OffsetY, end.OffsetY, t),
                AbsoluteUnit.Lerp(start.Blur, end.Blur, t),
                AbsoluteUnit.Lerp(start.Spread, end.Spread, t),
                LerpColor(start.Color, end.Color)
            );
        }
    }
}
