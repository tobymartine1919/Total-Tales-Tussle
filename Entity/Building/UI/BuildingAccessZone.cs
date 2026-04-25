using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Attach to a child GameObject with a CircleCollider2D (isTrigger = true).
/// Handles both resource delivery and construction worker arrival.
/// Per-unit identity checks prevent accidental triggering when units pass through.
/// </summary>
public class BuildingAccessZone : MonoBehaviour
{
    // ── Refs ──────────────────────────────────────────────────────────────

    [Title("References")]
    [SerializeField, Required] private BuildingCore _building;

    [Title("Settings")]
    [SerializeField] private float _accessRadius = 1.5f;

    // ── Unity ─────────────────────────────────────────────────────────────

    private void OnValidate()
    {
        if (TryGetComponent<CircleCollider2D>(out var col))
            col.radius = _accessRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<UnitController>(out var unit)) return;

        // Resource delivery: unit must be returning to THIS hub specifically
        if (unit.RuntimeData.IsDeliveringResource && unit.RuntimeData.CurrentHub == _building)
        {
            _building.ReceiveResource(unit);
            return;
        }

        // Construction arrival: unit must be assigned to construct THIS building
        if (unit.RuntimeData.CurrentBuilding == _building)
        {
            _building.NotifyWorkerArrivedForConstruction(unit);
        }
    }

    // ── Debug ─────────────────────────────────────────────────────────────

    [Button("Fetch References"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchReferences()
    {
        _building = GetComponentInParent<BuildingCore>(true);
    }

    [Button("Setup Collider"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void SetupCollider()
    {
        var col = gameObject.GetComponent<CircleCollider2D>();
        if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = _accessRadius;
        Debug.Log($"[BuildingAccessZone] Collider set up on {gameObject.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.25f);
        Gizmos.DrawSphere(transform.position, _accessRadius);
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