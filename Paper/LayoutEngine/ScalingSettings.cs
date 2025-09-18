// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI.LayoutEngine
{
    /// <summary>
    /// Defines how the UI is scaled.
    /// </summary>
    /// <remarks>
    /// This is a struct mainly because there are so many other double-type parameters.
    /// </remarks>
    public struct ScalingSettings
    {
        /// <summary>
        /// The scaling factor applied to point units.
        /// Eg: A value of 2 means that each point is equal to 2 pixels.
        /// </summary>
        public double ContentScale = 1;

        public ScalingSettings(double contentScale = 1)
        {
            ContentScale = contentScale;
        }
    }
}
