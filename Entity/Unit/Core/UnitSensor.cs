using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Detects nearby enemy units within a circular area using Physics2D overlap.
/// Attach this component to any unit that needs awareness of surrounding enemies.
/// </summary>
public class UnitSensor : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private LayerMask _unitLayer;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmo = true;

    private UnitController _owner;
    private UnitSetting _setting;

    public void Init(UnitController owner, UnitSetting setting)
    {
        _owner = owner;
        _setting = setting;
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private Collider2D[] Overlap() =>
        Physics2D.OverlapCircleAll(transform.position, _setting.detectionRange, _unitLayer);

    private bool IsEnemy(Collider2D col)
    {
        if (col.gameObject == gameObject) return false;
        UnitController unit = col.GetComponent<UnitController>();
        if (unit == null) return false;
        if (_owner.Team == Team.None || unit.Team == Team.None) return false;
        return unit.Team != _owner.Team;
    }

    // ── Core API ─────────────────────────────────────────────────────────

    /// <summary>Returns true if at least one enemy unit is within detection radius.</summary>
    public bool HasEnemyInRange()
    {
        foreach (Collider2D col in Overlap())
            if (IsEnemy(col)) return true;
        return false;
    }

    /// <summary>Returns the closest enemy UnitController within range, or null if none.</summary>
    public UnitController GetClosestEnemy()
    {
        UnitController closest = null;
        float closestSqrDist = float.MaxValue;

        foreach (Collider2D col in Overlap())
        {
            if (!IsEnemy(col)) continue;

            float sqrDist = ((Vector2)transform.position - (Vector2)col.transform.position).sqrMagnitude;
            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest = col.GetComponent<UnitController>();
            }
        }

        return closest;
    }

    /// <summary>Fills the provided list with all enemy UnitControllers in range. Clears the list first.</summary>
    public void GetAllEnemiesInRange(List<UnitController> results)
    {
        results.Clear();
        foreach (Collider2D col in Overlap())
        {
            if (!IsEnemy(col)) continue;
            UnitController unit = col.GetComponent<UnitController>();
            if (unit != null) results.Add(unit);
        }
    }

    // ── Odin Debug ────────────────────────────────────────────────────────

#if UNITY_EDITOR
    [Title("Debug Tools")]
    [Button(ButtonSizes.Medium), GUIColor(1f, 0.6f, 0f)]
    private void DebugHasEnemyInRange()
    {
        if (_owner == null) { Debug.LogWarning($"[{name}] Not initialised yet."); return; }
        Debug.Log($"[{name}] HasEnemyInRange → {HasEnemyInRange()}");
    }

    [Button(ButtonSizes.Medium), GUIColor(1f, 0.4f, 0.4f)]
    private void DebugGetClosestEnemy()
    {
        if (_owner == null) { Debug.LogWarning($"[{name}] Not initialised yet."); return; }
        UnitController closest = GetClosestEnemy();
        Debug.Log(closest != null
            ? $"[{name}] Closest enemy → {closest.name}"
            : $"[{name}] No enemies in range.");
    }

    [Button(ButtonSizes.Medium), GUIColor(0.4f, 0.8f, 1f)]
    private void DebugGetAllEnemiesInRange()
    {
        if (_owner == null) { Debug.LogWarning($"[{name}] Not initialised yet."); return; }
        var results = new List<UnitController>();
        GetAllEnemiesInRange(results);
        if (results.Count == 0)
            Debug.Log($"[{name}] No enemies in range.");
        else
            Debug.Log($"[{name}] {results.Count} enemies in range: {string.Join(", ", results.ConvertAll(u => u.name))}");
    }

    private void OnDrawGizmosSelected()
    {
        if (_setting == null) return;
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.15f);
        Gizmos.DrawSphere(transform.position, _setting.detectionRange);
    }
#endif
}