﻿// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

/*
using System.Collections;
using System.Collections.Generic;

using Prowl.PaperUI.GUI.Layout;

namespace Prowl.PaperUI.GUI;

public partial class Paper
{
    public int CurrentZIndex => CurrentNode.ZIndex;

    private static readonly Dictionary<ulong, Hashtable> _storage = [];

    /// <summary>
    /// Set the ZIndex for the current node
    /// ZIndex is used to determine the order in which nodes are drawn
    /// Think of them as layers in Photoshop
    /// They can also affect the order in which nodes receive input
    /// </summary>
    public void SetZIndex(int index, bool keepClipSpace = false)
    {
        Draw2D.SetZIndex(index, keepClipSpace);
        CurrentNode.ZIndex = index;
    }

    /// <summary> Get a value from the global GUI storage this persists across Nodes </summary>
    public T GetGlobalStorage<T>(string key) where T : unmanaged => GetNodeStorage<T>(_rootNode, key, default);
    /// <summary> Set a value in the global GUI storage this persists across Nodes </summary>
    public void SetGlobalStorage<T>(string key, T value) where T : unmanaged => SetNodeStorage(_rootNode, key, value);

    /// <summary> Get a value from the current node's storage </summary>
    public T GetNodeStorage<T>(string key, T defaultValue = default) where T : unmanaged => GetNodeStorage(CurrentNode, key, defaultValue);

    /// <summary> Get a value from the current node's storage </summary>
    public T GetNodeStorage<T>(LayoutNode node, string key, T defaultValue = default) where T : unmanaged
    {
        if (!_storage.TryGetValue(node.ID, out var storage))
            return defaultValue;

        if (storage.ContainsKey(key))
            return (T)storage[key];

        return defaultValue;
    }

    /// <summary> Set a value in the current node's storage </summary>
    public void SetNodeStorage<T>(string key, T value) where T : unmanaged => SetNodeStorage(CurrentNode, key, value);
    /// <summary> Set a value in the current node's storage </summary>
    public void SetNodeStorage<T>(LayoutNode node, string key, T value) where T : unmanaged
    {
        if (!_storage.TryGetValue(node.ID, out var storage))
            _storage[node.ID] = storage = [];

        storage[key] = value;
    }

    /// <summary>
    /// <para>Push an ID onto the ID stack.</para>
    /// <para>Useful for when you want to use the same string ID for multiple nodes that would otherwise conflict.</para>
    /// Or maybe you dont have control like a List of User-Created Nodes, you PushID(Index) and PopID() when done
    /// </summary>
    /// <param name="id"></param>
    public void PushID(ulong id) => _iDStack.Push(id);
    public void PopID() => _iDStack.Pop();

}
*/