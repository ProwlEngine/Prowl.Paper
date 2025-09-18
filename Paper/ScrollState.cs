﻿using Prowl.PaperUI.LayoutEngine;
using Prowl.Vector;

namespace Prowl.PaperUI
{
    /// <summary>
    /// Represents the scrolling state of an element.
    /// </summary>
    public struct ScrollState
    {
        // Basic state properties
        public Vector2 Position;
        public Vector2 ContentSize;
        public Vector2 ViewportSize;

        // Interaction state
        public bool IsDraggingVertical;
        public bool IsDraggingHorizontal;
        public Vector2 DragStartPosition;
        public Vector2 ScrollStartPosition;

        // UI interaction state
        public bool IsVerticalScrollbarHovered;
        public bool IsHorizontalScrollbarHovered;

        // Constants for scrollbar rendering
        public static readonly AbsoluteUnit ScrollbarSize = 12;
        public static readonly AbsoluteUnit ScrollbarMinSize = 20;
        public static readonly AbsoluteUnit ScrollbarPadding = 2;

        /// <summary>
        /// Gets the maximum scroll position.
        /// </summary>
        public Vector2 MaxScroll => new Vector2(
            Math.Max(0, ContentSize.x - ViewportSize.x),
            Math.Max(0, ContentSize.y - ViewportSize.y)
        );

        /// <summary>
        /// Determines if horizontal scrolling is needed for the given flags.
        /// </summary>
        public bool NeedsHorizontalScroll(Scroll flags) =>
            ContentSize.x > ViewportSize.x && (flags & Scroll.ScrollX) != 0;

        /// <summary>
        /// Determines if vertical scrolling is needed for the given flags.
        /// </summary>
        public bool NeedsVerticalScroll(Scroll flags) =>
            ContentSize.y > ViewportSize.y && (flags & Scroll.ScrollY) != 0;

        /// <summary>
        /// Determines if scrollbars should be visible for the given flags.
        /// </summary>
        public bool AreScrollbarsHidden(Scroll flags) => (flags & Scroll.Hidden) != 0;

        /// <summary>
        /// Clamps the scroll position to valid values.
        /// </summary>
        public void ClampScrollPosition()
        {
            Vector2 max = MaxScroll;
            Position = new Vector2(
                Math.Clamp(Position.x, 0, max.x),
                Math.Clamp(Position.y, 0, max.y)
            );
        }

        /// <summary>
        /// Calculates the vertical scrollbar dimensions based on the element rect.
        /// </summary>
        public (double x, double y, double width, double height, double thumbY, double thumbHeight) CalculateVerticalScrollbar(Rect rect, Scroll flags, in ScalingSettings scalingSettings)
        {
            bool hasHorizontal = NeedsHorizontalScroll(flags);
            double scrollbarSize = ScrollbarSize.ToPx(scalingSettings);
            double scrollbarMinSize = ScrollbarMinSize.ToPx(scalingSettings);

            // Calculate track dimensions
            double trackHeight = rect.height;
            if (hasHorizontal)
                trackHeight -= scrollbarSize;

            double trackX = rect.x + rect.width - scrollbarSize;
            double trackY = rect.y;

            // Calculate thumb dimensions
            double thumbHeight = Math.Max(scrollbarMinSize,
                (ViewportSize.y / ContentSize.y) * trackHeight);

            double thumbY = trackY;
            if (MaxScroll.y > 0)
                thumbY += (Position.y / MaxScroll.y) * (trackHeight - thumbHeight);

            return (trackX, trackY, scrollbarSize, trackHeight, thumbY, thumbHeight);
        }

        /// <summary>
        /// Calculates the horizontal scrollbar dimensions based on the element rect.
        /// </summary>
        public (double x, double y, double width, double height, double thumbX, double thumbWidth) CalculateHorizontalScrollbar(Rect rect, Scroll flags, in ScalingSettings scalingSettings)
        {
            bool hasVertical = NeedsVerticalScroll(flags);
            double scrollbarSize = ScrollbarSize.ToPx(scalingSettings);
            double scrollbarMinSize = ScrollbarMinSize.ToPx(scalingSettings);

            // Calculate track dimensions
            double trackWidth = rect.width;
            if (hasVertical)
                trackWidth -= scrollbarSize;

            double trackX = rect.x;
            double trackY = rect.y + rect.height - scrollbarSize;

            // Calculate thumb dimensions
            double thumbWidth = Math.Max(scrollbarMinSize,
                (ViewportSize.x / ContentSize.x) * trackWidth);

            double thumbX = trackX;
            if (MaxScroll.x > 0)
                thumbX += (Position.x / MaxScroll.x) * (trackWidth - thumbWidth);

            return (trackX, trackY, trackWidth, scrollbarSize, thumbX, thumbWidth);
        }

        /// <summary>
        /// Checks if a point is over the vertical scrollbar.
        /// </summary>
        public bool IsPointOverVerticalScrollbar(Vector2 point, Rect rect, Scroll flags, in ScalingSettings scalingSettings)
        {
            if (!NeedsVerticalScroll(flags))
                return false;

            var (trackX, trackY, trackWidth, trackHeight, _, _) = CalculateVerticalScrollbar(rect, flags, scalingSettings);

            return point.x >= trackX &&
                   point.x <= trackX + trackWidth &&
                   point.y >= trackY &&
                   point.y <= trackY + trackHeight;
        }

        /// <summary>
        /// Checks if a point is over the horizontal scrollbar.
        /// </summary>
        public bool IsPointOverHorizontalScrollbar(Vector2 point, Rect rect, Scroll flags, in ScalingSettings scalingSettings)
        {
            if (!NeedsHorizontalScroll(flags))
                return false;

            var (trackX, trackY, trackWidth, trackHeight, _, _) = CalculateHorizontalScrollbar(rect, flags, scalingSettings);

            return point.x >= trackX &&
                   point.x <= trackX + trackWidth &&
                   point.y >= trackY &&
                   point.y <= trackY + trackHeight;
        }

        /// <summary>
        /// Handles scrollbar dragging for vertical scrollbar.
        /// </summary>
        public void HandleVerticalScrollbarDrag(Vector2 mousePos, Rect rect, Scroll flags, in ScalingSettings scalingSettings)
        {
            if (!IsDraggingVertical)
                return;

            var (_, trackY, _, trackHeight, _, thumbHeight) = CalculateVerticalScrollbar(rect, flags, scalingSettings);

            double dragDelta = mousePos.y - DragStartPosition.y;
            double scrollableHeight = trackHeight - thumbHeight;

            if (scrollableHeight > 0)
            {
                double scrollRatio = dragDelta / scrollableHeight;
                Position = new Vector2(
                    Position.x,
                    ScrollStartPosition.y + (scrollRatio * MaxScroll.y)
                );

                ClampScrollPosition();
            }
        }

        /// <summary>
        /// Handles scrollbar dragging for horizontal scrollbar.
        /// </summary>
        public void HandleHorizontalScrollbarDrag(Vector2 mousePos, Rect rect, Scroll flags, in ScalingSettings scalingSettings)
        {
            if (!IsDraggingHorizontal)
                return;

            var (trackX, _, trackWidth, _, _, thumbWidth) = CalculateHorizontalScrollbar(rect, flags, scalingSettings);

            double dragDelta = mousePos.x - DragStartPosition.x;
            double scrollableWidth = trackWidth - thumbWidth;

            if (scrollableWidth > 0)
            {
                double scrollRatio = dragDelta / scrollableWidth;
                Position = new Vector2(
                    ScrollStartPosition.x + (scrollRatio * MaxScroll.x),
                    Position.y
                );

                ClampScrollPosition();
            }
        }
    }
}
