using UnityEngine;

/// <summary>
/// Minor state inside IdleState.
/// Handles pure, task-free movement — no resource, no combat, no construction intent.
/// Transitions back to SelectionIdle once the unit reaches its destination.
/// </summary>
public class NormalMovementState : MinorState<UnitController>
{
    private UnitAction action;
    private readonly IdleState _idleState;

    public NormalMovementState(
    StateMachine<UnitController> sm,
    UnitController c,
    IdleState parent)
    : base(sm, c, parent, MinorStateEnum.NormalMovement)
    {
        _idleState = parent;
    }

    public override void Enter()
    {
        var move = character.UnitActionQueue.Dequeue() as MoveAction;
        if (move == null) return;
        character.Movement.MoveTo(move.Destination);
    }

    public override void Update()
    {
        // Let UnitMovement drive the transform every frame.
        // CheckTransitions handles arrival.
    }

    public override void CheckTransitions()
    {
        if (character.Movement.ReachedDestination())
        {
            character.Movement.Stop();
            _idleState.ChangeMinorState(MinorStateEnum.SelectionIdle);
        }
    }

    public override void Exit()
    {
        character.Movement.Stop();
    }
}