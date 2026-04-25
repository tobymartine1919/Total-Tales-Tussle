using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class AbilityPanel : MonoBehaviour
{
    [Title("Button Children")]
    [SerializeField] private List<Button> _buttons;

    [Title("Optional Sub-References (per button)")]
    [SerializeField] private List<Image> _buttonIcons;
    [SerializeField] private List<TextMeshProUGUI> _buttonLabels;

    private ISelectable _currentEntity;

    private void OnEnable() => SelectionIdleState.OnSelectionChanged += HandleSelectionChanged;
    private void OnDisable() => SelectionIdleState.OnSelectionChanged -= HandleSelectionChanged;

    private void HandleSelectionChanged(IReadOnlyList<ISelectable> selected)
    {
        _currentEntity = null;
        List<AbilityBaseSO> abilities = null;

        if (selected.Count == 1 && selected[0] is IHasAbilities hasAbilities)
        {
            _currentEntity = selected[0];
            abilities = hasAbilities.Abilities;
        }

        Refresh(abilities);
    }

    private void Refresh(List<AbilityBaseSO> abilities)
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            bool hasAbility = abilities != null && i < abilities.Count;

            _buttons[i].gameObject.SetActive(hasAbility);

            if (!hasAbility) continue;

            AbilityBaseSO ability = abilities[i];

            _buttons[i].interactable = true;

            _buttons[i].onClick.RemoveAllListeners();
            _buttons[i].onClick.AddListener(() =>
            {
                ability.Execute(_currentEntity);
            });

            // Optional icon
            if (_buttonIcons != null && i < _buttonIcons.Count && _buttonIcons[i] != null)
            {
                _buttonIcons[i].sprite = ability.Icon;
                _buttonIcons[i].color = ability.Icon == null ? Color.clear : Color.white;
            }

            // Optional label
            if (_buttonLabels != null && i < _buttonLabels.Count && _buttonLabels[i] != null)
                _buttonLabels[i].text = ability.DisplayName;
        }
    }

    [Button("Fetch Buttons"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchButtons()
    {
        _buttons = new List<Button>(GetComponentsInChildren<Button>(true));
    }
}