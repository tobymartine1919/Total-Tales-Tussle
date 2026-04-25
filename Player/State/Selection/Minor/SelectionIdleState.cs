using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionIdleState : MinorState<PlayerController>
{
    private readonly List<ISelectable> _selected = new();

    private Vector2 _dragStartWorld;
    private Vector2 _dragStartScreen;
    private bool _isDragging;
    private bool _dragStartedOnWorld;
    private const float DragThreshold = 0.2f;

    private const int FormationColumnLimit = 5;
    private const float FormationSpacing = 1.2f;

    public static event System.Action<IReadOnlyList<ISelectable>> OnSelectionChanged;

    private static readonly int SelectionMask = LayerMask.GetMask("Selection");

    public SelectionIdleState(StateMachine<PlayerController> sm, PlayerController c, MajorState<PlayerController> parent)
        : base(sm, c, parent, MinorStateEnum.SelectionIdle) { }

    public override void Update()
    {
        if (character.Input.LeftClickPressed)
        {
            if (!IsPointerOverUI())
            {
                _dragStartWorld = GetMouseWorldPos();
                _dragStartScreen = GetMouseScreenPos();
                _dragStartedOnWorld = true;
            }
            else
            {
                _dragStartedOnWorld = false;
            }
        }

        if (character.Input.LeftClickHeld)
        {
            if (_dragStartedOnWorld)
            {
                if (!_isDragging && Vector2.Distance(_dragStartWorld, GetMouseWorldPos()) > DragThreshold)
                    _isDragging = true;

                if (_isDragging)
                    character.SelectionBox.UpdateBox(_dragStartScreen, GetMouseScreenPos());
            }
        }

        if (character.Input.LeftClickReleased)
        {
            character.SelectionBox.Hide();
            bool wasDragging = _isDragging;
            _isDragging = false;

            if (_dragStartedOnWorld)
            {
                if (wasDragging)
                    BoxSelect();
                else
                    SingleSelect();
            }
        }

        if (character.Input.RightClickPressed && !IsPointerOverUI())
            HandleOrder();
    }

    private void SingleSelect()
    {
        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPos(), Vector2.zero, Mathf.Infinity, SelectionMask);
        DeselectAll();

        if (hit.collider != null)
        {
            var selectable = hit.collider.GetComponent<ISelectable>();
            if (selectable != null)
                Select(selectable);
        }
    }

    private void BoxSelect()
    {
        DeselectAll();

        Vector2 end = GetMouseWorldPos();
        Vector2 center = (_dragStartWorld + end) / 2f;
        Vector2 size = new Vector2(
            Mathf.Abs(_dragStartWorld.x - end.x),
            Mathf.Abs(_dragStartWorld.y - end.y)
        );

        foreach (var col in Physics2D.OverlapBoxAll(center, size, 0f, SelectionMask))
        {
            var selectable = col.GetComponent<ISelectable>();
            if (selectable != null)
                Select(selectable);
        }
    }

    private void HandleOrder()
    {
        if (_selected.Count == 0) return;

        RaycastHit2D hit = Physics2D.Raycast(GetMouseWorldPos(), Vector2.zero, Mathf.Infinity, SelectionMask);

        var playerUnits = _selected
            .OfType<UnitController>()
            .Where(u => u.Team == character.PlayerTeam)
            .ToList();

        // ── Rally point: only a building is selected, right-click on empty ground ──
        if (playerUnits.Count == 0)
        {
            var playerBuilding = _selected
                .OfType<BuildingCore>()
                .FirstOrDefault(b => b.Team == character.PlayerTeam);

            if (playerBuilding != null && hit.collider == null)
            {
                playerBuilding.SetRallyPoint(GetMouseWorldPos());
                return;
            }
        }

        // ── Unit orders ───────────────────────────────────────────────────────────
        var selectable = hit.collider?.GetComponent<ISelectable>();

        if (selectable == null)
        {
            IssueMoveFormation(playerUnits, GetMouseWorldPos());
        }
        else if (selectable.Team != character.PlayerTeam && selectable.Team != Team.None)
        {
            var damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                foreach (var unit in playerUnits)
                    unit.IssueAttack(damageable);
            }
        }
        else if (hit.collider.GetComponent<ResourceNodeCore>() is ResourceNodeCore node && !node.IsDepleted)
        {
            foreach (var unit in playerUnits)
                unit.IssueGather(node);
        }
        else if (hit.collider.GetComponent<BuildingCore>() is BuildingCore building && !building.IsComplete)
        {
            foreach (var unit in playerUnits)
                unit.IssueConstruct(building);
        }
        else
        {
            IssueMoveFormation(playerUnits, GetMouseWorldPos());
        }
    }

    private void IssueMoveFormation(List<UnitController> units, Vector2 destination)
    {
        if (units.Count == 0) return;

        if (units.Count == 1)
        {
            units[0].IssueMove(destination);
            return;
        }

        var slotAssignments = FormationGrid.Assign(
            units,
            destination,
            FormationColumnLimit,
            FormationSpacing
        );

        foreach (var (unit, slot) in slotAssignments)
            unit.IssueMove(slot);
    }

    private void Select(ISelectable selectable)
    {
        _selected.Add(selectable);
        selectable.Select();
        OnSelectionChanged?.Invoke(_selected);
    }

    private void Deselect(ISelectable selectable)
    {
        _selected.Remove(selectable);
        selectable.Deselect();
        OnSelectionChanged?.Invoke(_selected);
    }

    private void DeselectAll()
    {
        foreach (var s in _selected)
            s.Deselect();
        _selected.Clear();
        OnSelectionChanged?.Invoke(_selected);
    }

    private bool IsPointerOverUI()
        => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    private Vector2 GetMouseWorldPos()
        => Camera.main.ScreenToWorldPoint(character.Input.MousePosition);

    private Vector2 GetMouseScreenPos()
        => character.Input.MousePosition;
}