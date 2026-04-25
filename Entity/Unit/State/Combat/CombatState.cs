using UnityEngine;

public class CombatState : MajorState<UnitController>
{
    private IDamageable _target;

    public CombatState(StateMachine<UnitController> sm, UnitController c)
        : base(sm, c, MajorStateEnum.Combat, MinorStateEnum.Chasing) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.Chasing, new ChasingState(stateMachine, character, this));
        AddMinorState(MinorStateEnum.Attacking, new AttackingState(stateMachine, character, this));
    }

    public override void CheckTransitions() { }

    public override void Enter()
    {
        if (!character.UnitActionQueue.HasAction ||
            character.UnitActionQueue.Dequeue() is not AttackAction action)
        {
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }
        _target = action.Target;
        ChangeMinorState(MinorStateEnum.Chasing);
    }

    public IDamageable GetTarget() => _target;
}