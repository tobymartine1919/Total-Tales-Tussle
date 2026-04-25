using UnityEngine;

/// <summary>
/// Moves the unit toward the target building.
/// Arrival is signalled by BuildingCore.OnWorkerArrivedForConstructionEvent
/// (fired by BuildingAccessZone when this unit enters the zone).
/// </summary>
public class WalkingToSiteState : MinorState<UnitController>
{
    public WalkingToSiteState(StateMachine<UnitController> sm, UnitController c, MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.WalkingToSite) { }

    public override void Enter()
    {
        var building = character.RuntimeData.CurrentBuilding;
        if (building == null)
        {
            Debug.LogWarning($"[{character.name}] WalkingToSite: no building cached.");
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }

        building.OnWorkerArrivedForConstructionEvent += OnArrived;
        character.Movement.MoveTo(building.transform.position);
    }

    public override void Exit()
    {
        var building = character.RuntimeData.CurrentBuilding;
        if (building != null)
            building.OnWorkerArrivedForConstructionEvent -= OnArrived;

        character.Movement.Stop();
    }

    public override void Update() { }
    public override void CheckTransitions() { }

    // ── Callbacks ─────────────────────────────────────────────────────────

    private void OnArrived(UnitController arrived)
    {
        if (arrived != character) return;
        (parentState as ConstructionUnitState)?.ChangeMinorState(MinorStateEnum.Building);
    }
}