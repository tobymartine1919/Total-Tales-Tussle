using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class UnitHealthBarUI : MonoBehaviour
{
    [SerializeField] private UnitHealth _health;
    [SerializeField] private Transform _fill;

    [Header("Tween Settings")]
    [SerializeField] private float _tweenDuration = 0.2f;
    [SerializeField] private Ease _ease = Ease.OutQuart;

    private void OnEnable() => _health.OnDamaged += UpdateBar;
    private void OnDisable() => _health.OnDamaged -= UpdateBar;

    private void UpdateBar()
    {
        float percent = _health.CurrentHp / _health.MaxHp;
        float targetX = percent * 100f;
        _fill.DOScaleX(targetX, _tweenDuration).SetEase(_ease);
    }
}