public class ChoosingBuildingState : MinorState<PlayerController>
{
    public ChoosingBuildingState(StateMachine<PlayerController> sm, PlayerController c, MajorState<PlayerController> parent)
        : base(sm, c, parent, MinorStateEnum.ChoosingBuilding) { }

    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
    public override void CheckTransitions() { }
}
