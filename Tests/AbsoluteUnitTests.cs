// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

namespace Tests;

public class AbsoluteUnitTests
{
    [Fact]
    public void Equals_BasicProperties_ReturnsTrue()
    {
        var a = new AbsoluteUnit(AbsoluteUnits.Pixels, 10);
        var b = new AbsoluteUnit(AbsoluteUnits.Pixels, 10);

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
    }

    [Fact]
    public void Equals_InterpolatingVsNonInterpolating_ReturnsFalse()
    {
        var interpolating = AbsoluteUnit.Lerp(new AbsoluteUnit(AbsoluteUnits.Pixels, 10), new AbsoluteUnit(AbsoluteUnits.Pixels, 30), 0.5);
        var plain = new AbsoluteUnit(AbsoluteUnits.Pixels, 10);

        Assert.False(interpolating.Equals(plain));
        Assert.False(plain.Equals(interpolating));
    }

    [Fact]
    public void Equals_InterpolatingBothWithSameData_ReturnsTrue()
    {
        var first = AbsoluteUnit.Lerp(new AbsoluteUnit(AbsoluteUnits.Pixels, 10), new AbsoluteUnit(AbsoluteUnits.Pixels, 30), 0.5);
        var second = AbsoluteUnit.Lerp(new AbsoluteUnit(AbsoluteUnits.Pixels, 10), new AbsoluteUnit(AbsoluteUnits.Pixels, 30), 0.5);

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(first));
    }

    [Fact]
    public void Equals_InterpolatingDifferentProgress_ReturnsFalse()
    {
        var first = AbsoluteUnit.Lerp(new AbsoluteUnit(AbsoluteUnits.Pixels, 10), new AbsoluteUnit(AbsoluteUnits.Pixels, 30), 0.5);
        var second = AbsoluteUnit.Lerp(new AbsoluteUnit(AbsoluteUnits.Pixels, 10), new AbsoluteUnit(AbsoluteUnits.Pixels, 30), 0.25);

        Assert.False(first.Equals(second));
        Assert.False(second.Equals(first));
    }

    [Fact]
    public void ToPx_WithInterpolation_ComputesExpectedValue()
    {
        var scalingSettings = new ScalingSettings(3);
        var uv = AbsoluteUnit.Lerp(new AbsoluteUnit(AbsoluteUnits.Pixels, 10), new AbsoluteUnit(AbsoluteUnits.Points, 50), 0.5);

        double result = uv.ToPx(scalingSettings);

        Assert.Equal(80, result, 5);
    }
}
