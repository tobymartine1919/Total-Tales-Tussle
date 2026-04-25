using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingManager : Singleton<BuildingManager>
{
    // ── Registry ──────────────────────────────────────────────────────────

    [Title("Registry (Runtime)")]
    [SerializeField, ReadOnly]
    private int _totalRegistered;

    private readonly Dictionary<BuildingType, List<BuildingCore>> _registry = new();

    // ── Registration ──────────────────────────────────────────────────────

    public void Register(BuildingType type, BuildingCore building)
    {
        if (!_registry.ContainsKey(type))
            _registry[type] = new List<BuildingCore>();

        _registry[type].Add(building);
        building.OnDestroyed += () => Unregister(type, building);

        _totalRegistered++;
        Debug.Log($"[BuildingManager] Registered {building.name} as {type} (Team: {building.Team})");
    }

    public void Unregister(BuildingType type, BuildingCore building)
    {
        if (_registry.TryGetValue(type, out var list) && list.Remove(building))
            _totalRegistered--;
    }

    // ── Queries ───────────────────────────────────────────────────────────

    /// All buildings of a type (any team)
    public IReadOnlyList<BuildingCore> GetAll(BuildingType type)
    {
        return _registry.TryGetValue(type, out var list)
            ? list
            : Array.Empty<BuildingCore>();
    }

    /// All buildings of a type belonging to a specific team
    public List<BuildingCore> GetAll(BuildingType type, Team team)
    {
        var result = new List<BuildingCore>();
        if (!_registry.TryGetValue(type, out var list)) return result;

        foreach (var b in list)
            if (b.Team == team) result.Add(b);

        return result;
    }

    /// First completed building of a type + team, or null
    public BuildingCore GetFirst(BuildingType type, Team team)
    {
        if (!_registry.TryGetValue(type, out var list)) return null;

        foreach (var b in list)
            if (b.Team == team && b.IsComplete) return b;

        return null;
    }

    public bool HasAny(BuildingType type, Team team)
        => GetFirst(type, team) != null;

    // ── Debug Buttons ─────────────────────────────────────────────────────

    [Title("Debug")]

    [Button("Log All Registered"), GUIColor(1f, 0.8f, 0f)]
    private void DebugLogAll()
    {
        if (_registry.Count == 0)
        {
            Debug.Log("[BuildingManager] Registry is empty.");
            return;
        }

        foreach (var kvp in _registry)
        {
            var sb = new StringBuilder();
            sb.Append($"[{kvp.Key}] ({kvp.Value.Count}) → ");
            foreach (var b in kvp.Value)
                sb.Append($"{b.name}({b.Team}) ");
            Debug.Log(sb.ToString());
        }
    }

    [Button("Clear Registry"), GUIColor(1f, 0.3f, 0.3f)]
    private void DebugClearRegistry()
    {
        _registry.Clear();
        _totalRegistered = 0;
        Debug.LogWarning("[BuildingManager] Registry forcefully cleared.");
    }

    // ═══════════════════════════════════════════
    // SCRATCH — delete this region when done
    // ═══════════════════════════════════════════
    #region Scratch
    [FoldoutGroup("Scratch", false)][SerializeField] private float tempFloat1, tempFloat2, _tempFloat3;
    [FoldoutGroup("Scratch", false)][SerializeField] private int tempInt1, tempInt2, _tempInt3;
    [FoldoutGroup("Scratch", false)][SerializeField] private bool tempBool1, tempBool2, _tempBool3;
    [FoldoutGroup("Scratch", false)][SerializeField] private Vector2 tempVec1, tempVec2, _tempVec3;
    [FoldoutGroup("Scratch", false)][SerializeField] private string tempStr1, tempStr2, _tempStr3;
    [FoldoutGroup("Scratch", false)][SerializeField] private GameObject tempObj1, tempObj2, _tempObj3;
    [FoldoutGroup("Scratch", false)][Button, GUIColor(1f, 0.8f, 0f)] private void Test1() { }
    [FoldoutGroup("Scratch", false)][Button, GUIColor(1f, 0.8f, 0f)] private void Test2() { }
    [FoldoutGroup("Scratch", false)][Button, GUIColor(1f, 0.8f, 0f)] private void Test3() { }
    [FoldoutGroup("Scratch", false)][Button, GUIColor(1f, 0.8f, 0f)] private void Test4() { }
    [FoldoutGroup("Scratch", false)][Button, GUIColor(1f, 0.8f, 0f)] private void Test5() { }
    #endregion
    // ═══════════════════════════════════════════
}