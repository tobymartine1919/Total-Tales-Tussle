using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Attach to a child GameObject with a CircleCollider2D (isTrigger = true).
/// Notifies the node when a worker arrives to gather.
/// </summary>
public class ResourceNodeAccessZone : MonoBehaviour
{
    // ── Refs ──────────────────────────────────────────────────────────────

    [Title("References")]
    [SerializeField, Required] private ResourceNodeCore _node;

    [Title("Settings")]
    [SerializeField] private float _accessRadius = 1f;

    // ── Unity ─────────────────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_node.IsDepleted) return;
        if (!other.TryGetComponent<UnitController>(out var unit)) return;
        if (!unit.Setting.Has(UnitCapability.Collect)) return;

        _node.OnWorkerArrived(unit);
    }

    // ── Debug Buttons ─────────────────────────────────────────────────────

    [Button("Fetch References"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchReferences()
    {
        _node = GetComponentInParent<ResourceNodeCore>(true);
    }

    [Button("Setup Collider"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void SetupCollider()
    {
        var col = gameObject.GetComponent<CircleCollider2D>();
        if (col == null) col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = _accessRadius;
        Debug.Log($"[ResourceNodeAccessZone] Collider set up on {gameObject.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.25f);
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