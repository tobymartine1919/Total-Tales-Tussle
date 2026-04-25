public class SelectionState : MajorState<PlayerController>
{
    public SelectionState(StateMachine<PlayerController> sm, PlayerController c)
        : base(sm, c, MajorStateEnum.Selection, MinorStateEnum.SelectionIdle) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.SelectionIdle, new SelectionIdleState(stateMachine, character, this));
    }

    public override void CheckTransitions()
    {
        // Switch major states here, e.g. fall → Airborne
    }
    
}