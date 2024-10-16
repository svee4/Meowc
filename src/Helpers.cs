using Godot;
using Meowc.Components;
using System;
using System.Collections.Generic;
using Meowc.ECS;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Meowc;

public static class Helpers
{
    /// <summary>
    /// Gets a node with <see cref="Godot.Node.GetNode{T}(NodePath)"/><br/>
    /// Throws a <see cref="Meowc.NodeNotFoundException" /> if the node is not found
    /// </summary>
    /// <exception cref="NodeNotFoundException"></exception>
    public static TNode GetNodeOrThrow<TNode>(this Node node, NodePath nodePath) where TNode : Node
    {
        var result = node.GetNode<TNode>(nodePath);
        if (result is null) ThrowNodeNotFound(nodePath);
        return result;

        [DoesNotReturn]
        static void ThrowNodeNotFound(NodePath nodePath) =>
            throw new NodeNotFoundException(nodePath);
    }


    /// <summary>
    /// Try get component by its name
    /// </summary>
    public static bool TryGetComponent<TComponent>(
        this Node node, 
        string componentName,
        [NotNullWhen(true)] out TComponent? component) 
        where TComponent : class, IComponent
    {
        var comp = node.GetNodeOrNull(componentName);

        Helpers.DebugAssert(
            comp is null or TComponent, // actual name will not be null if the assert is hit
            $"Component '{componentName}' for node '{node.Name}' was found with wrong type, " +
            $"expected '{typeof(TComponent).Name}', actual '{comp?.GetType().Name}'"); 

        if (comp is TComponent comp2)
        {
            component = comp2;
            return true;
        }

        component = null;
        return false;
    }

    /// <summary>
    /// Try get a component thats name is the same as its type
    /// </summary>
    public static bool TryGetComponent<TComponent>(this Node node, [NotNullWhen(true)] out TComponent? component) 
            where TComponent : class, IComponent =>
        TryGetComponent(node, typeof(TComponent).Name, out component);


    /// <summary>
    /// Try get component by its name.<br/>
    /// Throws <see cref="ComponentNotFoundException"/> if component is not found
    /// </summary>
    /// <exception cref="ComponentNotFoundException"></exception>
    public static TComponent GetComponent<TComponent>(
        this Node node,
        string componentName)
        where TComponent : class, IComponent
    {
        if (!TryGetComponent<TComponent>(node, componentName, out var component))
        {
            ThrowComponentNotFound<TComponent>(node, componentName);
        }

        return component;

        [DoesNotReturn]
        static void ThrowComponentNotFound<T>(Node node, string componentName)
        {
            throw new ComponentNotFoundException(componentName, node.Name);
        }
    }

    /// <summary>
    /// Try get a component thats name is the same as its type.<br/>
    /// Throws <see cref="ComponentNotFoundException"/> if component is not found
    /// </summary>
    /// <exception cref="ComponentNotFoundException"></exception>
    public static TComponent GetComponent<TComponent>(this Node node) 
            where TComponent : class, IComponent =>
        GetComponent<TComponent>(node, typeof(TComponent).Name);


    public static Node GetSceneRoot(this Node node)
    {
        var tempRoot = node;

        // get the root of the block's scene. TODO make this better
        while (true)
        {
            if (tempRoot.Owner is { } parent) tempRoot = parent;
            else break;
        }

        return tempRoot;
    }

    [Conditional("DEBUG")]
    public static void DebugAssert(
        bool condition,
        string message,
        [CallerArgumentExpression(nameof(condition))] string expression = null!)
    {
        if (!condition)
        {
            Debugger.Break();
        }
    }
}

public sealed class NodeNotFoundException(NodePath nodePath) : Exception($"Node not found: '{nodePath}'");
public sealed class ComponentNotFoundException(string componentName, string nodeName) 
    : Exception($"Component '{componentName}' not found in node '{nodeName}'");