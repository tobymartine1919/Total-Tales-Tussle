using UnityEngine;

public class ChasingState : MinorState<UnitController>
{
    private IDamageable _target;
    private readonly CombatState _parent;

    public ChasingState(StateMachine<UnitController> sm, UnitController c, CombatState parent)
        : base(sm, c, parent, MinorStateEnum.Chasing)
    {
        _parent = parent;
    }

    public override void Enter()
    {
        _target = _parent.GetTarget();
        ChaseTarget();
    }

    public override void Exit()
    {
        character.Movement.Stop();
    }

    public override void Update()
    {
        ChaseTarget();
    }

    public override void CheckTransitions()
    {
        if (_target == null || _target.IsDead)
        {
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }

        float dist = Vector2.Distance(character.transform.position, _target.Position);
        if (dist <= character.Setting.attackRange)
        {
            character.Movement.Stop();
            _parent.ChangeMinorState(MinorStateEnum.Attacking);
        }
    }

    private void ChaseTarget()
    {
        if (_target != null && !_target.IsDead)
            character.Movement.MoveTo(_target.Position);
    }
}