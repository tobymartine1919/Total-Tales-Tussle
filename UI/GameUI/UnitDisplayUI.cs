using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UnitDisplayUI : MonoBehaviour
{
    [Title("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;

    private void OnEnable() => SelectionIdleState.OnSelectionChanged += HandleSelectionChanged;
    private void OnDisable() => SelectionIdleState.OnSelectionChanged -= HandleSelectionChanged;

    private void HandleSelectionChanged(IReadOnlyList<ISelectable> selected)
    {
        if (selected.Count == 0)
        {
            Clear();
            return;
        }

        if (selected.Count == 1)
        {
            Display(selected[0].DisplayName, selected[0].Icon);
            return;
        }

        // multiple — check if all same name
        bool allSame = selected.All(s => s.DisplayName == selected[0].DisplayName);
        if (allSame)
            Display($"{selected[0].DisplayName} x{selected.Count}", selected[0].Icon);
        else
            Display($"Mixed ({selected.Count})", null);
    }

    private void Display(string label, Sprite icon)
    {
        if (nameText != null)
            nameText.text = string.IsNullOrEmpty(label) ? "null" : label;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.color = icon == null ? Color.red : Color.white;
        }
    }

    private void Clear()
    {
        if (nameText != null) nameText.text = "";
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = Color.white;
        }
    }
}