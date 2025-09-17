using Prowl.PaperUI;
using Prowl.Vector;

namespace Prowl.PaperUI.LayoutEngine
{
    public struct Rounding
    {
        public AbsoluteUnit TopLeft;
        public AbsoluteUnit TopRight;
        public AbsoluteUnit BottomRight;
        public AbsoluteUnit BottomLeft;

        public Rounding(AbsoluteUnit topLeft, AbsoluteUnit topRight, AbsoluteUnit bottomRight, AbsoluteUnit bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }

        public Vector4 ToPx(ScalingSettings scalingSettings)
        {
            return new Vector4(
                TopLeft.ToPx(scalingSettings), TopRight.ToPx(scalingSettings),
                BottomRight.ToPx(scalingSettings), BottomLeft.ToPx(scalingSettings));
        }
    }

    public struct ScalingSettings
    {
        /// <summary>
        /// The scaling factor applied to point units.
        /// Eg: A value of 2 means that each point is equal to 2 pixels.
        /// </summary>
        public double PointUnitScale = 1;

        public ScalingSettings() { }
    }

    /// <summary>
    /// Defines measurement units for element dimensions and positioning.
    /// </summary>
    public enum AbsoluteUnits
    {
        /// <summary>
        /// Fixed-sized unit with each pixel corresponding to a physical pixel on the screen.
        /// Useful for precise element sizing.
        /// </summary>
        Pixels,

        /// <summary>
        /// Variable-sized unit based on the device's pixel density.
        /// Useful for automatic element sizing based on the device's pixel density.
        /// </summary>
        Points
    }

    /// <summary>
    /// Defines measurement units for element dimensions and positioning.
    /// </summary>
    public enum RelativeUnits
    {
        /// <inheritdoc cref="AbsoluteUnits.Pixels"/>
        Pixels,

        /// <inheritdoc cref="AbsoluteUnits.Points"/>
        Points,

        /// <summary>
        /// Percentage of parent container's corresponding dimension.
        /// </summary>
        Percentage,

        /// <summary>
        /// Flexible sizing that distributes available space based on stretch factors.
        /// </summary>
        Stretch,

        /// <summary>
        /// Size is determined automatically based on content or other constraints.
        /// </summary>
        Auto
    }

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

    /// <summary>
    /// Represents a value with a unit type for UI layout measurements.
    /// Supports pixels and points.
    /// </summary>
    public struct AbsoluteUnit : IEquatable<AbsoluteUnit>
    {
        /// <summary>The unit type of this value</summary>
        public AbsoluteUnits Type { get; set; } = AbsoluteUnits.Points;

        /// <summary>The numeric value in the specified units</summary>
        public double Value { get; set; } = 0f;

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
            return Type switch {
                AbsoluteUnits.Pixels => Value,
                AbsoluteUnits.Points => Value * scalingSettings.PointUnitScale,
                _ => throw new ArgumentOutOfRangeException()
            };
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
            return Type == other.Type && Value.Equals(other.Value);
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

    /// <summary>
    /// Represents a layout relative value with a unit type for UI layout measurements.
    /// Supports pixels, points, percentages, auto-sizing, and stretch units with interpolation capabilities.
    /// </summary>
    public struct RelativeUnit : IEquatable<RelativeUnit>
    {
        /// <summary>
        /// Helper class for interpolation between two RelativeUnit instances.
        /// Using a simplified class approach to avoid struct cycles.
        /// </summary>
        private class LerpData
        {
            public readonly RelativeUnit Start;
            public readonly RelativeUnit End;
            public readonly double Progress;

            public LerpData(RelativeUnit start, RelativeUnit end, double progress)
            {
                Start = start;
                End = end;
                Progress = progress;
            }
        }

        /// <summary>The unit type of this value</summary>
        public RelativeUnits Type { get; set; } = RelativeUnits.Auto;

        /// <summary>The numeric value in the specified units</summary>
        public double Value { get; set; } = 0f;

        /// <summary>Additional pixel offset when using percentage units</summary>
        public double PercentPixelOffset { get; set; } = default;

        /// <summary>Data for interpolation between two RelativeUnits (null when not interpolating)</summary>
        private LerpData? _lerpData = null;

        /// <summary>
        /// Creates a default RelativeUnit with Auto units.
        /// </summary>
        public RelativeUnit() { }

        /// <summary>
        /// Creates a RelativeUnit with the specified type and value.
        /// </summary>
        /// <param name="type">The unit type</param>
        /// <param name="value">The numeric value</param>
        /// <param name="offset">Additional pixel offset for percentage units</param>
        public RelativeUnit(RelativeUnits type, double value = 0f, double offset = 0f)
        {
            Type = type;
            Value = value;
            PercentPixelOffset = offset;
        }

        #region Type Checking Properties

        /// <summary>Returns true if this value is using Pixel units</summary>
        public bool IsPixels => Type == RelativeUnits.Pixels;

        /// <summary>Returns true if this value is using Point units</summary>
        public bool IsPoints => Type == RelativeUnits.Points;

        /// <summary>Returns true if this value is using Auto units</summary>
        public bool IsAuto => Type == RelativeUnits.Auto;

        /// <summary>Returns true if this value is using Stretch units</summary>
        public bool IsStretch => Type == RelativeUnits.Stretch;

        /// <summary>Returns true if this value is using Percentage units</summary>
        public bool IsPercentage => Type == RelativeUnits.Percentage;

        #endregion

        /// <summary>
        /// Converts this unit value to pixels based on the parent's size.
        /// </summary>
        /// <param name="parentValue">The parent element's size in pixels</param>
        /// <param name="defaultValue">Default value to use for Auto and Stretch units</param>
        /// <param name="scalingSettings">Settings to use for scaling calculations</param>
        /// <returns>Size in pixels</returns>
        public readonly double ToPx(double parentValue, double defaultValue, in ScalingSettings scalingSettings)
        {
            // Handle interpolation if active
            if (_lerpData != null)
            {
                var startPx = _lerpData.Start.ToPx(parentValue, defaultValue, scalingSettings);
                var endPx = _lerpData.End.ToPx(parentValue, defaultValue, scalingSettings);
                return startPx + (endPx - startPx) * _lerpData.Progress;
            }

            // Convert based on unit type
            return Type switch {
                RelativeUnits.Pixels => Value,
                RelativeUnits.Points => Value * scalingSettings.PointUnitScale,
                RelativeUnits.Percentage => ((Value / 100f) * parentValue) + PercentPixelOffset,
                _ => defaultValue
            };
        }

        /// <summary>
        /// Converts this unit value to pixels and clamps it between minimum and maximum values.
        /// </summary>
        /// <param name="parentValue">The parent element's size in pixels</param>
        /// <param name="defaultValue">Default value to use for Auto and Stretch units</param>
        /// <param name="min">Minimum allowed value</param>
        /// <param name="max">Maximum allowed value</param>
        /// <param name="scalingSettings">Settings to use for scaling calculations</param>
        /// <returns>Size in pixels, clamped between min and max</returns>
        public readonly double ToPxClamped(double parentValue, double defaultValue, in RelativeUnit min, in RelativeUnit max, in ScalingSettings scalingSettings)
        {
            double minValue = min.ToPx(parentValue, double.MinValue, scalingSettings);
            double maxValue = max.ToPx(parentValue, double.MaxValue, scalingSettings);
            double value = ToPx(parentValue, defaultValue, scalingSettings);

            return Math.Min(maxValue, Math.Max(minValue, value));
        }

        /// <summary>
        /// Linearly interpolates between two RelativeUnit instances.
        /// In reality, it creates a new RelativeUnit with special interpolation data which is calculated when ToPx is called.
        /// </summary>
        /// <param name="a">Starting value</param>
        /// <param name="b">Ending value</param>
        /// <param name="blendFactor">Interpolation factor (0.0 to 1.0)</param>
        /// <returns>Interpolated RelativeUnit</returns>
        public static RelativeUnit Lerp(in RelativeUnit a, in RelativeUnit b, double blendFactor)
        {
            // Ensure blend factor is between 0 and 1
            blendFactor = Math.Clamp(blendFactor, 0f, 1f);

            // If units are the same, we can blend directly
            if (a.Type == b.Type)
            {
                return new RelativeUnit(
                    a.Type,
                    a.Value + (b.Value - a.Value) * blendFactor,
                    a.PercentPixelOffset + (b.PercentPixelOffset - a.PercentPixelOffset) * blendFactor
                );
            }

            // If units are different, use interpolation data
            var result = new RelativeUnit {
                Type = a.Type,
                Value = a.Value,
                PercentPixelOffset = a.PercentPixelOffset,
                _lerpData = new LerpData(a, b, blendFactor)
            };
            return result;
        }

        /// <summary>
        /// Creates a deep copy of this RelativeUnit.
        /// </summary>
        /// <returns>A new RelativeUnit with the same properties</returns>
        public readonly RelativeUnit Clone() => new RelativeUnit {
            Type = Type,
            Value = Value,
            PercentPixelOffset = PercentPixelOffset,
            _lerpData = _lerpData != null ? new LerpData(_lerpData.Start, _lerpData.End, _lerpData.Progress) : null
        };

        #region Implicit Conversions

        /// <summary>
        /// Implicitly converts an integer to a point unit RelativeUnit.
        /// </summary>
        public static implicit operator RelativeUnit(int value)
        {
            return new RelativeUnit(RelativeUnits.Points, value);
        }

        /// <summary>
        /// Implicitly converts a double to a point unit RelativeUnit.
        /// </summary>
        public static implicit operator RelativeUnit(double value)
        {
            return new RelativeUnit(RelativeUnits.Points, value);
        }

        #endregion

        #region Equality and Hashing

        public static bool operator ==(RelativeUnit left, RelativeUnit right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RelativeUnit left, RelativeUnit right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this RelativeUnit with another object for equality.
        /// </summary>
        public override readonly bool Equals(object? obj)
        {
            return obj is RelativeUnit other && Equals(other);
        }

        public readonly bool Equals(RelativeUnit other)
        {
            // First, check the basic properties
            bool basicPropertiesEqual = Type == other.Type &&
                                        Value.Equals(other.Value) &&
                                        PercentPixelOffset.Equals(other.PercentPixelOffset);

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
        /// Returns a hash code for this RelativeUnit.
        /// </summary>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(_lerpData, (int)Type, Value, PercentPixelOffset);
        }

        #endregion

        /// <summary>
        /// Returns a string representation of this RelativeUnit.
        /// </summary>
        public override readonly string ToString() => Type switch {
            RelativeUnits.Pixels => $"{Value}px",
            RelativeUnits.Points => $"{Value}pt",
            RelativeUnits.Percentage => $"{Value}% + {PercentPixelOffset}",
            RelativeUnits.Stretch => $"Stretch({Value})",
            RelativeUnits.Auto => "Auto",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
