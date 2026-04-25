using UnityEngine;

public class IdleState : MajorState<UnitController>
{
    private Vector2 _pendingDestination;
    private bool _hasPendingMove;

    public IdleState(StateMachine<UnitController> sm, UnitController c)
        : base(sm, c, MajorStateEnum.Idle, MinorStateEnum.SelectionIdle) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.SelectionIdle, new SelectionIdle(stateMachine, character, this));
        AddMinorState(MinorStateEnum.NormalMovement, new NormalMovementState(stateMachine, character, this));
    }

    public override void Enter()
    {
        if (character.UnitActionQueue.HasAction)
        {
            ChangeMinorState(MinorStateEnum.NormalMovement);
        }
        else
        {
            ChangeMinorState(MinorStateEnum.SelectionIdle);
        }
    }

    public override void Exit() { }
    public override void CheckTransitions() { }
}