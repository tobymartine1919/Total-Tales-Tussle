using UnityEngine;

public class GatheringState : MinorState<UnitController>
{
    private Timer _gatherTimer;

    public GatheringState(StateMachine<UnitController> sm, UnitController c, MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.Gathering) { }

    public override void Enter()
    {
        _gatherTimer = new Timer(character.Setting.collectingSpeed);
        _gatherTimer.Start();
    }

    public override void Exit()
    {
        _gatherTimer = null;
    }

    public override void Update()
    {
        if (_gatherTimer == null || !_gatherTimer.IsReady) return;

        var node = character.RuntimeData.CurrentNode;
        if (node == null || node.IsDepleted)
        {
            // Node gone — return whatever we have or go idle
            if (!character.RuntimeData.IsCarryEmpty)
                (parentState as CollectingState)?.ChangeMinorState(MinorStateEnum.Returning);
            else
                stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }

        int gathered = node.Gather(1);
        character.RuntimeData.PickupResource(gathered);

        _gatherTimer.Start(); // restart for next tick
    }

    public override void CheckTransitions()
    {
        if (character.RuntimeData.IsCarryFull)
            (parentState as CollectingState)?.ChangeMinorState(MinorStateEnum.Returning);
    }
}