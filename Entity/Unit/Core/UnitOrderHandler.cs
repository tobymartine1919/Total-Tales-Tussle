using UnityEngine;

/// <summary>
/// Sits between UnitController (events) and UnitStateMachine (transitions).
/// Owns all order routing logic — decides which major/minor state a given
/// order should resolve to based on context.
/// </summary>
public class UnitOrderHandler
{
    private readonly UnitController _character;
    private readonly UnitStateMachine _sm;
    private UnitSetting Setting => _character.Setting;

    public UnitOrderHandler(UnitController unit, UnitStateMachine stateMachine)
    {
        _character = unit;
        _sm = stateMachine;
    }

    // ── Move ──────────────────────────────────────────────────────────────

    public void IssueMove(Vector2 destination)
    {
        _character.UnitActionQueue.Enqueue(new MoveAction(destination));
        _sm.ChangeMajorState(MajorStateEnum.Idle);
    }

    // ── Attack ────────────────────────────────────────────────────────────

    public void IssueAttack(IDamageable damageable)
    {
        if (damageable == null) return;
        if (!Setting.Has(UnitCapability.Fight))
        {
            Debug.LogWarning($"[{_character.name}] Cannot fight — missing Fight capability.");
            return;
        }

        _character.UnitActionQueue.Enqueue(new AttackAction(damageable));
        _sm.ChangeMajorState(MajorStateEnum.Combat);
    }

    // ── Collect ───────────────────────────────────────────────────────────

    public void IssueCollect(ResourceNodeCore node)
    {
        if (node == null) return;
        if (!Setting.Has(UnitCapability.Collect))
        {
            Debug.LogWarning($"[{_character.name}] Cannot collect — missing Collect capability.");
            return;
        }

        var hub = FindNearestHub(_character.Team, _character.transform.position);
        if (hub == null)
        {
            Debug.LogWarning($"[{_character.name}] No friendly Hub found — cannot issue gather order.");
            return;
        }

        _character.UnitActionQueue.Clear();
        _character.UnitActionQueue.Enqueue(new GatherAction(node, hub));
        _sm.ChangeMajorState(MajorStateEnum.Collecting);
    }

    // ── Build ─────────────────────────────────────────────────────────────

    public void IssueBuild(BuildingCore building)
    {
        if (building == null) return;
        if (!Setting.Has(UnitCapability.Build))
        {
            Debug.LogWarning($"[{_character.name}] Cannot build — missing Build capability.");
            return;
        }
        if (building.IsComplete)
        {
            Debug.LogWarning($"[{_character.name}] Building {building.name} is already complete.");
            return;
        }

        _character.UnitActionQueue.Clear();
        _character.UnitActionQueue.Enqueue(new ConstructAction(building));
        _sm.ChangeMajorState(MajorStateEnum.Construction_Unit);
    }

    // ── Stop ──────────────────────────────────────────────────────────────

    public void IssueStop()
    {
        _character.UnitActionQueue.Clear();
        _sm.ChangeMajorState(MajorStateEnum.Idle);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private BuildingCore FindNearestHub(Team team, Vector2 from)
        => BuildingManager.I.GetFirst(BuildingType.Hub, team);
}