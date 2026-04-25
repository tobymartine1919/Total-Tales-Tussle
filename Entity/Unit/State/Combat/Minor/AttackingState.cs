using UnityEngine;

public class AttackingState : MinorState<UnitController>
{
    private CombatState _combatState;
    private IDamageable _target;
    private Timer _attackTimer;

    public AttackingState(StateMachine<UnitController> sm, UnitController c, CombatState parent)
        : base(sm, c, parent, MinorStateEnum.Attacking)
    {
        _combatState = parent;
    }

    public override void Enter()
    {
        _target = _combatState.GetTarget();
        _attackTimer = new Timer(character.Setting.attackInterval);
        _attackTimer.Finish();
    }

    public override void Exit()
    {
        _target = null;
    }

    public override void Update()
    {
        if (_attackTimer.IsReady)
        {
            _target.TakeDamage(character.Setting.atk);
            _attackTimer.Start();
        }
    }

    public override void CheckTransitions()
    {
        if (_target == null || _target.IsDead)
        {
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }

        float dist = Vector2.Distance(character.transform.position, _target.Position);
        if (dist > character.Setting.attackRange)
            _combatState.ChangeMinorState(MinorStateEnum.Chasing);
    }
}