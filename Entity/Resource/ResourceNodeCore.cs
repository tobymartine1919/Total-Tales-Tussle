using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceNodeCore : MonoBehaviour, ISelectable, IGatherable
{
    // ── Settings ──────────────────────────────────────────────────────────

    [Title("Settings")]
    [SerializeField, Required] private ResourceNodeSO _setting;
    public ResourceNodeSO Setting => _setting;
    public string DisplayName => _setting.DisplayName;
    public Sprite Icon => _setting.Icon;
    // ── Runtime State ─────────────────────────────────────────────────────

    [Title("State (Runtime)")]
    [SerializeField, ReadOnly] private int _currentAmount;

    public int CurrentAmount => _currentAmount;
    public bool IsDepleted => _currentAmount <= 0;

    // ISelectable
    public Team Team => Team.None;

    // IGatherable
    public ResourceType ResourceType => _setting.resourceType;

    public event Action OnDepleted;
    public event Action<UnitController> OnWorkerArrivedEvent;

    // ── Unity ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _currentAmount = _setting.maxAmount;
    }

    // ── IGatherable ───────────────────────────────────────────────────────

    public int Gather(int amount)
    {
        if (IsDepleted) return 0;

        int gathered = Mathf.Min(amount, _currentAmount);
        _currentAmount -= gathered;

        if (IsDepleted)
            OnDepleted?.Invoke();

        return gathered;
    }

    // ── Access ────────────────────────────────────────────────────────────

    /// Called by ResourceNodeAccessZone when a collector unit enters.
    public void OnWorkerArrived(UnitController worker)
    {
        if (IsDepleted) return;

        OnWorkerArrivedEvent?.Invoke(worker);
    }

    // ── ISelectable ───────────────────────────────────────────────────────

    public void Select() => Debug.Log($"[{name}] Resource selected");
    public void Deselect() => Debug.Log($"[{name}] Resource deselected");

    // ── Debug Buttons ─────────────────────────────────────────────────────

    [Title("Debug")]

    [Button("Deplete All"), GUIColor(1f, 0.3f, 0.3f)]
    private void DebugDeplete()
    {
        _currentAmount = 0;
        OnDepleted?.Invoke();
    }

    [Button("Reset Amount"), GUIColor(1f, 0.8f, 0f)]
    private void DebugReset() => _currentAmount = _setting.maxAmount;

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