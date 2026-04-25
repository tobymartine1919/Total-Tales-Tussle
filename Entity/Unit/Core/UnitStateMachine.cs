public class UnitStateMachine : StateMachine<UnitController>
{
    private IdleState _idleState;

    public UnitStateMachine(UnitController character) : base(character) { }

    protected override void InitializeStates()
    {
        _idleState = new IdleState(this, character);

        AddMajorState(MajorStateEnum.Idle, _idleState);
        AddMajorState(MajorStateEnum.Collecting, new CollectingState(this, character));
        AddMajorState(MajorStateEnum.Construction_Unit, new ConstructionUnitState(this, character));
        AddMajorState(MajorStateEnum.Combat, new CombatState(this, character));
        AddMajorState(MajorStateEnum.Death, new DeathState(this, character));

        ChangeMajorState(MajorStateEnum.Idle);
    }
}