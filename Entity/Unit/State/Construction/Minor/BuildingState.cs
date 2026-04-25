using UnityEngine;

/// <summary>
/// Unit has arrived at the build site and is actively constructing.
/// Ticks ApplyConstruction every frame scaled by the unit's constructionSpeed.
/// Transitions to Idle when the building is complete.
/// </summary>
public class BuildingState : MinorState<UnitController>
{
    public BuildingState(StateMachine<UnitController> sm, UnitController c, MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.Building) { }

    public override void Enter()
    {
        var building = character.RuntimeData.CurrentBuilding;
        if (building == null || building.IsComplete)
        {
            Debug.LogWarning($"[{character.name}] BuildingState: building null or already complete.");
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }

        building.OnCompleted += OnBuildingCompleted;
    }

    public override void Exit()
    {
        var building = character.RuntimeData.CurrentBuilding;
        if (building != null)
            building.OnCompleted -= OnBuildingCompleted;
    }

    public override void Update()
    {
        var building = character.RuntimeData.CurrentBuilding;
        if (building == null || building.IsComplete) return;

        // constructionSpeed scales how fast this unit contributes
        building.ApplyConstruction(Time.deltaTime * character.Setting.constructionSpeed);
    }

    public override void CheckTransitions() { }

    // ── Callbacks ─────────────────────────────────────────────────────────

    private void OnBuildingCompleted()
    {
        Debug.Log($"[{character.name}] BuildingState: building complete → Idle");
        stateMachine.ChangeMajorState(MajorStateEnum.Idle);
    }
}