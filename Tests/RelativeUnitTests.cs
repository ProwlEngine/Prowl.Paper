// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;

namespace Tests;

public class RelativeUnitTests
{
    [Fact]
    public void Equals_BasicProperties_ReturnsTrue()
    {
        var a = new RelativeUnit(RelativeUnits.Pixels, 10);
        var b = new RelativeUnit(RelativeUnits.Pixels, 10);

        Assert.True(a.Equals(b));
        Assert.True(b.Equals(a));
    }

    [Fact]
    public void Equals_InterpolatingVsNonInterpolating_ReturnsFalse()
    {
        var interpolating = RelativeUnit.Lerp(new RelativeUnit(RelativeUnits.Pixels, 10), new RelativeUnit(RelativeUnits.Percentage, 50), 0.5);
        var plain = new RelativeUnit(RelativeUnits.Pixels, 10);

        Assert.False(interpolating.Equals(plain));
        Assert.False(plain.Equals(interpolating));
    }

    [Fact]
    public void Equals_InterpolatingBothWithSameData_ReturnsTrue()
    {
        var first = RelativeUnit.Lerp(new RelativeUnit(RelativeUnits.Pixels, 10), new RelativeUnit(RelativeUnits.Percentage, 50), 0.5);
        var second = RelativeUnit.Lerp(new RelativeUnit(RelativeUnits.Pixels, 10), new RelativeUnit(RelativeUnits.Percentage, 50), 0.5);

        Assert.True(first.Equals(second));
        Assert.True(second.Equals(first));
    }

    [Fact]
    public void Equals_InterpolatingDifferentProgress_ReturnsFalse()
    {
        var first = RelativeUnit.Lerp(new RelativeUnit(RelativeUnits.Pixels, 10), new RelativeUnit(RelativeUnits.Percentage, 50), 0.5);
        var second = RelativeUnit.Lerp(new RelativeUnit(RelativeUnits.Pixels, 10), new RelativeUnit(RelativeUnits.Percentage, 50), 0.25);

        Assert.False(first.Equals(second));
        Assert.False(second.Equals(first));
    }

    [Fact]
    public void ToPx_WithInterpolation_ComputesExpectedValue()
    {
        var scalingSettings = new ScalingSettings();
        var uv = RelativeUnit.Lerp(new RelativeUnit(RelativeUnits.Pixels, 10), new RelativeUnit(RelativeUnits.Percentage, 50), 0.5);

        double result = uv.ToPx(200, 0, scalingSettings);

        Assert.Equal(55, result, 5);
    }
}
