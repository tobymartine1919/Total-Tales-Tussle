using UnityEngine;

public class ConstructionUnitState : MajorState<UnitController>
{
    public ConstructionUnitState(StateMachine<UnitController> sm, UnitController c)
        : base(sm, c, MajorStateEnum.Construction_Unit, MinorStateEnum.WalkingToSite) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.WalkingToSite, new WalkingToSiteState(stateMachine, character, this));
        AddMinorState(MinorStateEnum.Building, new BuildingState(stateMachine, character, this));
    }

    public override void Enter()
    {
        // Pull ConstructAction from queue and seed RuntimeData before minor states start
        if (character.UnitActionQueue.HasAction &&
            character.UnitActionQueue.Peek() is ConstructAction constructAction)
        {
            character.UnitActionQueue.Dequeue();
            character.RuntimeData.CurrentBuilding = constructAction.TargetBuilding;
        }
        else
        {
            Debug.LogWarning($"[{character.name}] ConstructionUnitState entered with no ConstructAction in queue.");
        }

        base.Enter(); // triggers default minor state Enter()
    }

    public override void Exit()
    {
        character.RuntimeData.CurrentBuilding = null;
    }

    public override void CheckTransitions() { }
}