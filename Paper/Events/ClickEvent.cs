﻿// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;
using Prowl.Vector.Geometry;

namespace Prowl.PaperUI.Events;

public class ClickEvent : ElementEvent
{
    public PaperMouseBtn Button { get; }

    public ClickEvent(ElementHandle source, Rect elementRect, Double2 pointerPos, PaperMouseBtn button)
        : base(source, elementRect, pointerPos)
    {
        Button = button;
    }
}
