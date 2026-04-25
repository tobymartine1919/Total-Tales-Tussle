using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class UnitHealth : MonoBehaviour, IDamageable
{
    private UnitSetting _setting;

    public float CurrentHp { get; private set; }
    public float MaxHp { get; private set; }
    public bool IsDead => CurrentHp <= 0;
    public Vector2 Position => transform.position;

    public event Action OnDamaged;
    public event Action OnDeath;

    public void Init(UnitSetting setting)
    {
        _setting = setting;
        MaxHp = _setting.hp;
        CurrentHp = MaxHp;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        OnDamaged?.Invoke();
        if (IsDead) OnDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
        OnDamaged?.Invoke();
    }

    // ── Debug ────────────────────────────────────────────────────────────

    [FoldoutGroup("Debug")]
    [Range(0f, 100f)]
    [SerializeField] private float _debugDamage = 10f;

    [FoldoutGroup("Debug")]
    [Button("Deal Damage")]
    private void DebugDealDamage() => TakeDamage(_debugDamage);

    [FoldoutGroup("Debug")]
    [Button("Full Heal")]
    private void DebugHeal() => Heal(MaxHp);

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
}