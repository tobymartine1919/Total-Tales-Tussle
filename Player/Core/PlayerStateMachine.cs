public class PlayerStateMachine : StateMachine<PlayerController>
{
    public PlayerStateMachine(PlayerController character) : base(character) { }

    protected override void InitializeStates()
    {
        AddMajorState(MajorStateEnum.Selection,    new SelectionState(this, character));
        AddMajorState(MajorStateEnum.Construction, new ConstructionState(this, character));

        ChangeMajorState(MajorStateEnum.Selection);
    }
}
