using UnityEngine;

public class CollectingState : MajorState<UnitController>
{
    public CollectingState(StateMachine<UnitController> sm, UnitController c)
        : base(sm, c, MajorStateEnum.Collecting, MinorStateEnum.WalkingToResource) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.WalkingToResource, new WalkingToResourceState(stateMachine, character, this));
        AddMinorState(MinorStateEnum.Gathering, new GatheringState(stateMachine, character, this));
        AddMinorState(MinorStateEnum.Returning, new ReturningState(stateMachine, character, this));
    }

    public override void Enter()
    {
        // Pull GatherAction from queue and seed RuntimeData before minor states start
        if (character.UnitActionQueue.HasAction &&
            character.UnitActionQueue.Peek() is GatherAction gatherAction)
        {
            character.UnitActionQueue.Dequeue();
            character.RuntimeData.CurrentNode = gatherAction.TargetNode;
            character.RuntimeData.CurrentHub = gatherAction.TargetHub;
        }
        else
        {
            Debug.LogWarning($"[{character.name}] CollectingState entered with no GatherAction in queue.");
        }

        base.Enter(); // triggers default minor state Enter()
    }

    public override void CheckTransitions() { }
}