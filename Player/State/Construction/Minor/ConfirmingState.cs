public class ConfirmingState : MinorState<PlayerController>
{
    public ConfirmingState(StateMachine<PlayerController> sm, PlayerController c, MajorState<PlayerController> parent)
        : base(sm, c, parent, MinorStateEnum.Confirming) { }

    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
    public override void CheckTransitions() { }
}
