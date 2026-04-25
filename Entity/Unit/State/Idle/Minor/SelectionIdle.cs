public class SelectionIdle : MinorState<UnitController>
{
    public SelectionIdle(StateMachine<UnitController> sm, UnitController c, MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.SelectionIdle) { }

    public override void Enter() { }
    public override void Exit() { }
    public override void Update() { }
    public override void CheckTransitions() { }
}
