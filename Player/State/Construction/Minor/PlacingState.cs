using UnityEngine;

/// <summary>
/// Player is in placement mode: a ghost preview follows the mouse.
/// Left click  → place the real building, go back to Selection.
/// Right click → cancel, go back to Selection.
/// </summary>
public class PlacingState : MinorState<PlayerController>
{
    private GameObject _ghost;

    public PlacingState(StateMachine<PlayerController> sm, PlayerController c, MajorState<PlayerController> parent)
        : base(sm, c, parent, MinorStateEnum.Placing) { }

    public override void Enter()
    {
        if (character.PendingBuildingPrefab == null)
        {
            Debug.LogWarning("[PlacingState] No pending building prefab — returning to Selection.");
            stateMachine.ChangeMajorState(MajorStateEnum.Selection);
            return;
        }

        // Spawn ghost: semi-transparent copy that follows the mouse
        _ghost = GameObject.Instantiate(character.PendingBuildingPrefab);
        SetGhostAlpha(0.5f);

        // Disable everything on the ghost so it doesn't register as a real building
        if (_ghost.TryGetComponent<BuildingCore>(out var core))
            core.enabled = false;

        foreach (var col in _ghost.GetComponentsInChildren<Collider2D>())
            col.enabled = false;
    }

    public override void Exit()
    {
        // Clean up ghost if it somehow survived (e.g. state machine forced exit)
        if (_ghost != null)
        {
            GameObject.Destroy(_ghost);
            _ghost = null;
        }
    }

    public override void Update()
    {
        if (_ghost == null) return;

        // Ghost follows mouse
        _ghost.transform.position = GetMouseWorldPos();

        // Left click → place
        if (character.Input.LeftClickPressed)
        {
            PlaceBuilding();
            return;
        }

        // Right click → cancel
        if (character.Input.RightClickPressed)
        {
            CancelPlacement();
        }
    }

    public override void CheckTransitions() { }

    // ── Helpers ───────────────────────────────────────────────────────────

    private void PlaceBuilding()
    {
        Vector2 pos = GetMouseWorldPos();

        // Destroy ghost first
        GameObject.Destroy(_ghost);
        _ghost = null;

        // Spawn real building
        GameObject.Instantiate(character.PendingBuildingPrefab, pos, Quaternion.identity);

        character.PendingBuildingPrefab = null;
        stateMachine.ChangeMajorState(MajorStateEnum.Selection);
    }

    private void CancelPlacement()
    {
        GameObject.Destroy(_ghost);
        _ghost = null;

        character.PendingBuildingPrefab = null;
        stateMachine.ChangeMajorState(MajorStateEnum.Selection);
    }

    private void SetGhostAlpha(float alpha)
    {
        foreach (var sr in _ghost.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }

    private Vector2 GetMouseWorldPos()
        => Camera.main.ScreenToWorldPoint(character.Input.MousePosition);
}