using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BuildingHealthBarUI : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required] private BuildingCore _building;
    [SerializeField, Required] private Transform _fill;
    [SerializeField, Required] private CanvasGroup _canvasGroup;

    [Title("Tween Settings")]
    [SerializeField] private float _tweenDuration = 0.2f;
    [SerializeField] private Ease _ease = Ease.OutQuart;

    [Title("Auto-Hide Settings")]
    [SerializeField] private float _hideDelay = 2f;
    [SerializeField] private float _fadeDuration = 0.4f;

    private Tween _hideTween;

    private void OnEnable()
    {
        _building.OnDamaged += ShowBar;
    }

    private void OnDisable()
    {
        _building.OnDamaged -= ShowBar;
        _hideTween?.Kill();
    }

    private void ShowBar()
    {
        UpdateBar();
        _hideTween?.Kill();
        _canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
        _hideTween = DOVirtual.DelayedCall(_hideDelay, FadeOut);
    }

    private void UpdateBar()
    {
        float percent = _building.CurrentHp / _building.MaxHp;
        _fill.DOScaleX(percent * 100f, _tweenDuration).SetEase(_ease);
    }

    private void FadeOut()
    {
        _hideTween = _canvasGroup.DOFade(0f, _fadeDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => gameObject.SetActive(false));
    }

    [Button("Fetch References"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchReferences()
    {
        _fill = transform.Find("Fill");
        _canvasGroup = GetComponentInChildren<CanvasGroup>(true);
    }
}