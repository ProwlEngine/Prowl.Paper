// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

namespace Prowl.PaperUI.LayoutEngine
{
    /// <summary>
    /// Represents a value with a unit type for UI layout measurements.
    /// Supports pixels and points with interpolation capabilities.
    /// </summary>
    public struct AbsoluteUnit : IEquatable<AbsoluteUnit>
    {
        /// <summary>
        /// Helper class for interpolation between two AbsoluteUnit instances.
        /// Using a simplified class approach to avoid struct cycles.
        /// </summary>
        private class LerpData
        {
            public readonly AbsoluteUnit Start;
            public readonly AbsoluteUnit End;
            public readonly double Progress;

            public LerpData(AbsoluteUnit start, AbsoluteUnit end, double progress)
            {
                Start = start;
                End = end;
                Progress = progress;
            }
        }

        /// <summary>The unit type of this value</summary>
        public AbsoluteUnits Type { get; set; } = AbsoluteUnits.Points;

        /// <summary>The numeric value in the specified units</summary>
        public double Value { get; set; } = 0f;

        /// <summary>Data for interpolation between two AbsoluteUnits (null when not interpolating)</summary>
        private LerpData? _lerpData = null;

        /// <summary>
        /// Creates a default AbsoluteUnit with Points units.
        /// </summary>
        public AbsoluteUnit() { }

        /// <summary>
        /// Creates a AbsoluteUnit with the specified type and value.
        /// </summary>
        /// <param name="type">The unit type</param>
        /// <param name="value">The numeric value</param>
        public AbsoluteUnit(AbsoluteUnits type, double value = 0f)
        {
            Type = type;
            Value = value;
        }

        #region Type Checking Properties

        /// <summary>Returns true if this value is using Pixel units</summary>
        public bool IsPixels => Type == AbsoluteUnits.Pixels;

        /// <summary>Returns true if this value is using Point units</summary>
        public bool IsPoints => Type == AbsoluteUnits.Points;

        #endregion

        /// <summary>
        /// Converts this unit value to pixels.
        /// </summary>
        /// <param name="scalingSettings">Settings to use for scaling calculations</param>
        /// <returns>Size in pixels</returns>
        public readonly double ToPx(in ScalingSettings scalingSettings)
        {
            // Handle interpolation if active
            if (_lerpData != null)
            {
                var startPx = _lerpData.Start.ToPx(scalingSettings);
                var endPx = _lerpData.End.ToPx(scalingSettings);
                return startPx + (endPx - startPx) * _lerpData.Progress;
            }

            return Type switch {
                AbsoluteUnits.Pixels => Value,
                AbsoluteUnits.Points => Value * scalingSettings.ContentScale,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Linearly interpolates between two AbsoluteUnit instances.
        /// In reality, it creates a new AbsoluteUnit with special interpolation data which is calculated when ToPx is called.
        /// </summary>
        /// <param name="a">Starting value</param>
        /// <param name="b">Ending value</param>
        /// <param name="blendFactor">Interpolation factor (0.0 to 1.0)</param>
        /// <returns>Interpolated AbsoluteUnit</returns>
        public static AbsoluteUnit Lerp(in AbsoluteUnit a, in AbsoluteUnit b, double blendFactor)
        {
            // Ensure blend factor is between 0 and 1
            blendFactor = Math.Clamp(blendFactor, 0f, 1f);

            // If units are the same, we can blend directly
            if (a.Type == b.Type)
            {
                return new AbsoluteUnit(
                    a.Type,
                    a.Value + (b.Value - a.Value) * blendFactor
                );
            }

            // If units are different, use interpolation data
            var result = new AbsoluteUnit {
                Type = a.Type,
                Value = a.Value,
                _lerpData = new LerpData(a, b, blendFactor)
            };
            return result;
        }

        #region Implicit Conversions

        /// <summary>
        /// Implicitly converts an integer to a point unit AbsoluteUnit.
        /// </summary>
        public static implicit operator AbsoluteUnit(int value)
        {
            return new AbsoluteUnit(AbsoluteUnits.Points, value);
        }

        /// <summary>
        /// Implicitly converts a double to a point unit AbsoluteUnit.
        /// </summary>
        public static implicit operator AbsoluteUnit(double value)
        {
            return new AbsoluteUnit(AbsoluteUnits.Points, value);
        }

        /// <summary>
        /// Implicitly converts an AbsoluteUnit to a RelativeUnit.
        /// </summary>
        public static implicit operator RelativeUnit(AbsoluteUnit value)
        {
            var relativeUnitType = value.Type switch
            {
                AbsoluteUnits.Pixels => RelativeUnits.Pixels,
                AbsoluteUnits.Points => RelativeUnits.Points,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new RelativeUnit(relativeUnitType, value.Value);
        }

        #endregion

        #region Equality and Hashing

        public static bool operator ==(AbsoluteUnit left, AbsoluteUnit right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AbsoluteUnit left, AbsoluteUnit right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this AbsoluteUnit with another object for equality.
        /// </summary>
        public override readonly bool Equals(object? obj)
        {
            return obj is AbsoluteUnit other && Equals(other);
        }

        public readonly bool Equals(AbsoluteUnit other)
        {
            // First, check the basic properties
            bool basicPropertiesEqual = Type == other.Type &&
                                        Value.Equals(other.Value);

            // If either value isn't interpolating, they're equal only if both aren't
            if (_lerpData is null || other._lerpData is null)
                return basicPropertiesEqual && _lerpData is null && other._lerpData is null;

            // Both values are interpolating – compare their interpolation data safely
            var thisLerp = _lerpData;
            var otherLerp = other._lerpData;
            bool lerpPropsEqual = thisLerp.Start.Equals(otherLerp.Start) &&
                                  thisLerp.End.Equals(otherLerp.End) &&
                                  thisLerp.Progress.Equals(otherLerp.Progress);

            return basicPropertiesEqual && lerpPropsEqual;
        }

        /// <summary>
        /// Returns a hash code for this AbsoluteUnit.
        /// </summary>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine((int)Type, Value);
        }

        #endregion

        /// <summary>
        /// Returns a string representation of this AbsoluteUnit.
        /// </summary>
        public override readonly string ToString() => Type switch {
            AbsoluteUnits.Pixels => $"{Value}px",
            AbsoluteUnits.Points => $"{Value}pt",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
