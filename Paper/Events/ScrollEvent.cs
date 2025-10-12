﻿// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;
using Prowl.Vector.Geometry;

namespace Prowl.PaperUI.Events;

public class ScrollEvent : ElementEvent
{
    public double Delta { get; }

    public ScrollEvent(ElementHandle source, Rect elementRect, Double2 pointerPos, double delta)
        : base(source, elementRect, pointerPos)
    {
        Delta = delta;
    }
}
