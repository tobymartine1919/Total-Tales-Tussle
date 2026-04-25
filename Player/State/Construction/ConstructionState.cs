public class ConstructionState : MajorState<PlayerController>
{
    // Default minor state is now Placing — ChoosingBuilding is handled by the UI (ability buttons)
    public ConstructionState(StateMachine<PlayerController> sm, PlayerController c)
        : base(sm, c, MajorStateEnum.Construction, MinorStateEnum.Placing) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.ChoosingBuilding, new ChoosingBuildingState(stateMachine, character, this));
        AddMinorState(MinorStateEnum.Placing, new PlacingState(stateMachine, character, this));
        AddMinorState(MinorStateEnum.Confirming, new ConfirmingState(stateMachine, character, this));
    }

    public override void CheckTransitions() { }
}