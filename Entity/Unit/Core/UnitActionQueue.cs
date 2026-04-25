using System.Collections.Generic;
using UnityEngine;

public class UnitActionQueue
{
    private readonly Queue<UnitAction> _queue = new();
    public bool HasAction => _queue.Count > 0;

    public void Enqueue(UnitAction action) => _queue.Enqueue(action);
    public UnitAction Dequeue() => _queue.Dequeue();
    public UnitAction Peek() => _queue.Peek();
    public void Clear() => _queue.Clear();

    private static string ActionLabel(UnitAction action) => action switch
    {
        MoveAction m => $"Move({m.Destination})",
        GatherAction g => $"Gather({g.TargetNode?.name ?? "null"} → {g.TargetHub?.name ?? "null"})",
        ConstructAction c => $"Construct({c.TargetBuilding?.name ?? "null"})",
        _ => action.GetType().Name
    };

    private string QueueContents()
    {
        if (_queue.Count == 0) return "empty";
        return string.Join(" → ", System.Linq.Enumerable.Select(_queue, ActionLabel));
    }
}

public abstract class UnitAction { }

public class MoveAction : UnitAction
{
    public Vector2 Destination { get; }
    public MoveAction(Vector2 destination) => Destination = destination;
}

public class AttackAction : UnitAction
{
    public IDamageable Target { get; }
    public AttackAction(IDamageable target) => Target = target;
}

/// <summary>
/// Stores everything the collecting loop needs upfront.
/// Hub is resolved once at order time — no mid-loop searching.
/// </summary>
public class GatherAction : UnitAction
{
    public ResourceNodeCore TargetNode { get; }
    public BuildingCore TargetHub { get; }

    public GatherAction(ResourceNodeCore node, BuildingCore hub)
    {
        TargetNode = node;
        TargetHub = hub;
    }
}

/// <summary>
/// Stores the building the unit has been ordered to construct.
/// Resolved once at order time — no mid-walk searching.
/// </summary>
public class ConstructAction : UnitAction
{
    public BuildingCore TargetBuilding { get; }
    public ConstructAction(BuildingCore building) => TargetBuilding = building;
}