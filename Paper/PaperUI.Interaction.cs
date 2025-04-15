﻿using Prowl.PaperUI;
using Prowl.PaperUI;
using Prowl.PaperUI.LayoutEngine;
using System.Numerics;

namespace Prowl.PaperUI
{
    public static partial class Paper
    {
        #region State Query Methods

        /// <summary>
        /// Checks if an element is currently hovered.
        /// </summary>
        public static bool IsElementHovered(ulong id) => _elementsInBubblePath.Contains(id);

        /// <summary>
        /// Checks if an element is currently active (pressed).
        /// </summary>
        public static bool IsElementActive(ulong id) => _activeElementId == id;

        /// <summary>
        /// Checks if an element has input focus.
        /// </summary>
        public static bool IsElementFocused(ulong id) => _focusedElementId == id;

        /// <summary>
        /// Checks if an element is currently being dragged.
        /// </summary>
        public static bool IsElementDragging(ulong id) =>
            _isDragging.TryGetValue(id, out bool isDragging) && isDragging;

        /// <summary>
        /// Checks if the current parent element is hovered.
        /// </summary>
        public static bool IsParentHovered => IsElementHovered(CurrentParent.ID);

        /// <summary>
        /// Checks if the current parent element is active.
        /// </summary>
        public static bool IsParentActive => _activeElementId == CurrentParent.ID;

        /// <summary>
        /// Checks if the current parent element has input focus.
        /// </summary>
        public static bool IsParentFocused => _focusedElementId == CurrentParent.ID;

        /// <summary>
        /// Checks if the current parent element is being dragged.
        /// </summary>
        public static bool IsParentDragging => IsElementDragging(CurrentParent.ID);

        #endregion

        #region Interaction State

        // Element interaction state tracking
        private static ulong _theHoveredElementId = 0;  // The ID of the element directly hovered by the pointer
        private static ulong _activeElementId = 0;      // Currently active (pressed) element
        private static ulong _focusedElementId = 0;     // Element with input focus

        // State tracking collections
        private static Dictionary<ulong, bool> _wasHoveredState = new Dictionary<ulong, bool>();
        private static Dictionary<ulong, Vector2> _dragStartPos = new Dictionary<ulong, Vector2>();
        private static HashSet<ulong> _elementsInBubblePath = new HashSet<ulong>();
        private static Dictionary<ulong, bool> _isDragging = new Dictionary<ulong, bool>();

        // Public access to interaction state
        public static ulong HoveredElementId => _theHoveredElementId;
        public static ulong ActiveElementId => _activeElementId;
        public static ulong FocusedElementId => _focusedElementId;

        #endregion

        #region Interaction Processing

        /// <summary>
        /// Main interaction handling method called each frame.
        /// </summary>
        private static void HandleInteractions()
        {
            // Reset hover state
            _elementsInBubblePath.Clear();
            ulong previousHoveredElementId = _theHoveredElementId;
            _theHoveredElementId = 0;

            // Find the topmost element under the pointer
            var t = Transform.Identity;
            Element? topmostInteractable = FindTopmostInteractableElement(RootElement, t);

            if (topmostInteractable != null)
            {
                _theHoveredElementId = topmostInteractable.ID;
                BuildBubblePath(topmostInteractable);
            }

            // Process hover events
            HandleHoverEvents(previousHoveredElementId);

            // Process scroll events
            if (_theHoveredElementId != 0 && PointerWheel != 0)
            {
                Element? hoveredElement = FindElementByID(_theHoveredElementId);
                if (hoveredElement != null)
                {
                    hoveredElement.OnScroll?.Invoke(PointerWheel, hoveredElement.LayoutRect);
                    BubbleEventToParents(hoveredElement, parent => parent.OnScroll?.Invoke(PointerWheel, hoveredElement.LayoutRect));
                }
            }

            // Process mouse button events
            HandleMouseEvents();
        }

        #endregion

        #region Element Hit Testing

        /// <summary>
        /// Recursively finds the topmost interactable element under the pointer.
        /// </summary>
        /// <param name="element">Current element to check</param>
        /// <param name="parentTransform">Accumulated transform from parent elements</param>
        /// <returns>The topmost interactable element or null if none found</returns>
        private static Element? FindTopmostInteractableElement(Element element, Transform parentTransform)
        {
            if (element == null)
                return null;

            // Calculate the combined transform
            Transform combinedTransform = parentTransform;
            var rect = new Rect(element.X, element.Y, element.LayoutWidth, element.LayoutHeight);
            Transform styleTransform = element._elementStyle.GetTransformForElement(rect);
            combinedTransform.Premultiply(ref styleTransform);

            // Transform pointer position to element's local space
            var inverseTransform = combinedTransform.Inverse();
            inverseTransform.TransformPoint(out float localX, out float localY, PointerPos.X, PointerPos.Y);

            // Check if pointer is over this element
            bool isPointerOverElement = IsPointOverElement(element, localX, localY);

            // If scissor is enabled, only check children if pointer is inside the element
            if (element._scissorEnabled == false || isPointerOverElement)
            {
                // Check children first (front to back, respecting z-order)
                if (element.Children != null && element.Children.Count > 0)
                {
                    var sortedChildren = element.GetSortedChildren;

                    for (int i = sortedChildren.Count - 1; i >= 0; i--)
                    {
                        var interactableChild = FindTopmostInteractableElement(sortedChildren[i], combinedTransform);
                        if (interactableChild != null)
                            return interactableChild;
                    }
                }
            }

            // If pointer is not over element, return null
            if (!isPointerOverElement)
                return null;

            // Return this element if it's interactable
            return element.IsNotInteractable ? null : element;
        }

        /// <summary>
        /// Tests if a point in local coordinates is within an element.
        /// </summary>
        private static bool IsPointOverElement(Element element, float localX, float localY) =>
            localX >= element.X &&
            localX <= element.X + element.LayoutWidth &&
            localY >= element.Y &&
            localY <= element.Y + element.LayoutHeight;

        #endregion

        #region Event Propagation

        /// <summary>
        /// Builds the bubble path from an element to the root.
        /// </summary>
        private static void BuildBubblePath(Element element)
        {
            Element? current = element;
            while (current != null)
            {
                _elementsInBubblePath.Add(current.ID);
                current = current.Parent;
            }
        }

        /// <summary>
        /// Propagates an event up the element hierarchy.
        /// </summary>
        private static void BubbleEventToParents(Element element, Action<Element> eventHandler)
        {
            Element? current = element.Parent;
            while (current != null)
            {
                eventHandler(current);
                current = current.Parent;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles hover, enter, and leave events.
        /// </summary>
        private static void HandleHoverEvents(ulong previousHoveredElementId)
        {
            // Find elements that were previously hovered but are no longer in bubble path
            var leftElements = new HashSet<ulong>(_wasHoveredState.Keys);
            leftElements.ExceptWith(_elementsInBubblePath);

            // Trigger leave events
            foreach (var leftElementId in leftElements)
            {
                Element? leftElement = FindElementByID(leftElementId);
                if (leftElement != null && leftElement.OnLeave != null)
                {
                    leftElement.OnLeave(leftElement.LayoutRect);
                }
                _wasHoveredState[leftElementId] = false;
            }

            // Trigger enter and hover events
            foreach (var hoveredId in _elementsInBubblePath)
            {
                Element? hoveredElement = FindElementByID(hoveredId);
                if (hoveredElement != null)
                {
                    bool wasHovered = _wasHoveredState.TryGetValue(hoveredId, out bool hoveredState) && hoveredState;

                    // Only trigger enter event if element wasn't hovered before
                    if (!wasHovered && hoveredElement.OnEnter != null)
                        hoveredElement.OnEnter(hoveredElement.LayoutRect);

                    // Always trigger hover event
                    hoveredElement.OnHover?.Invoke(hoveredElement.LayoutRect);

                    _wasHoveredState[hoveredId] = true;
                }
            }
        }

        /// <summary>
        /// Handles mouse button events including clicks, drags, and releases.
        /// </summary>
        private static void HandleMouseEvents()
        {
            // Handle double-click
            if (IsPointerDoubleClick(PaperMouseBtn.Left))
            {
                if (_theHoveredElementId != 0)
                {
                    Element? hoveredElement = FindElementByID(_theHoveredElementId);
                    if (hoveredElement != null)
                    {
                        // Direct event
                        hoveredElement.OnDoubleClick?.Invoke(hoveredElement.LayoutRect);

                        // Bubble event
                        BubbleEventToParents(hoveredElement, parent => parent.OnDoubleClick?.Invoke(parent.LayoutRect));
                    }
                }
            }

            // Handle press
            if (IsPointerPressed(PaperMouseBtn.Left))
            {
                if (_theHoveredElementId != 0)
                {
                    _activeElementId = _theHoveredElementId;
                    _dragStartPos[_activeElementId] = PointerPos;
                    _isDragging[_activeElementId] = false;

                    Element? activeElement = FindElementByID(_activeElementId);
                    if (activeElement != null)
                    {
                        // Direct event
                        activeElement.OnPress?.Invoke(activeElement.LayoutRect);

                        // Bubble event
                        BubbleEventToParents(activeElement, parent => parent.OnPress?.Invoke(parent.LayoutRect));
                    }
                }
            }
            // Handle release
            else if (IsPointerReleased(PaperMouseBtn.Left))
            {
                if (_activeElementId != 0)
                {
                    Element? activeElement = FindElementByID(_activeElementId);

                    if (activeElement != null)
                    {
                        // Handle drag end if element was being dragged
                        if (_isDragging.TryGetValue(_activeElementId, out bool isDragging) && isDragging)
                        {
                            Vector2 startPos = _dragStartPos[_activeElementId];
                            Vector2 endPos = PointerPos;
                            Vector2 delta = endPos - startPos;

                            // Direct event
                            activeElement.OnDragEnd?.Invoke(startPos, delta, activeElement.LayoutRect);

                            // Bubble event
                            BubbleEventToParents(activeElement, parent => parent.OnDragEnd?.Invoke(startPos, delta, parent.LayoutRect));

                            _isDragging[_activeElementId] = false;
                        }

                        // If released over the same element that was pressed, it's a click
                        if (_theHoveredElementId == _activeElementId)
                        {
                            // Direct click
                            activeElement.OnClick?.Invoke(activeElement.LayoutRect);

                            // Bubble click event
                            BubbleEventToParents(activeElement, parent => parent.OnClick?.Invoke(parent.LayoutRect));

                            // Update focus state
                            if (activeElement.IsFocusable)
                                _focusedElementId = _activeElementId;
                        }

                        // Direct release event
                        activeElement.OnRelease?.Invoke(activeElement.LayoutRect);

                        // Bubble release event
                        BubbleEventToParents(activeElement, parent => parent.OnRelease?.Invoke(parent.LayoutRect));
                    }

                    _activeElementId = 0;
                }
            }
            // Handle right-click
            else if (IsPointerPressed(PaperMouseBtn.Right))
            {
                if (_theHoveredElementId != 0)
                {
                    Element? hoveredElement = FindElementByID(_theHoveredElementId);
                    if (hoveredElement != null)
                    {
                        // Direct event
                        hoveredElement.OnRightClick?.Invoke(hoveredElement.LayoutRect);

                        // Bubble event
                        BubbleEventToParents(hoveredElement, parent => parent.OnRightClick?.Invoke(parent.LayoutRect));
                    }
                }
            }

            // Handle dragging
            if (IsPointerDown(PaperMouseBtn.Left) && _activeElementId != 0)
            {
                Element? activeElement = FindElementByID(_activeElementId);
                if (activeElement != null)
                {
                    // Direct event
                    activeElement.OnHeld?.Invoke(activeElement.LayoutRect);

                    // Bubble event
                    BubbleEventToParents(activeElement, parent => parent.OnHeld?.Invoke(parent.LayoutRect));

                    if (IsPointerMoving)
                    {
                        bool wasDragging = _isDragging.TryGetValue(_activeElementId, out bool isDragging) && isDragging;
                        Vector2 startPos = _dragStartPos[_activeElementId];

                        // Handle drag start
                        if (!wasDragging)
                        {
                            activeElement.OnDragStart?.Invoke(activeElement.LayoutRect);
                            BubbleEventToParents(activeElement, parent => parent.OnDragStart?.Invoke(parent.LayoutRect));

                            _isDragging[_activeElementId] = true;
                        }

                        // Handle continuous dragging
                        activeElement.OnDragging?.Invoke(startPos, activeElement.LayoutRect);
                        BubbleEventToParents(activeElement, parent => parent.OnDragging?.Invoke(startPos, parent.LayoutRect));
                    }
                }
            }
        }

        #endregion
    }
}