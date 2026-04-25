using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Abstract base for every ability in the game.
/// Subclass this to create concrete abilities (SpawnUnitAbilitySO, HealAbilitySO, etc.)
/// and override Execute() with the actual logic.
/// </summary>
public abstract class AbilityBaseSO : ScriptableObject
{
    [HorizontalGroup("Split", 64)]
    [PreviewField(64, ObjectFieldAlignment.Left)]
    [HideLabel]
    [SerializeField] private Sprite _icon;

    [VerticalGroup("Split/Right"), LabelWidth(120)]
    [SerializeField] private string _displayName;

    [VerticalGroup("Split/Right"), LabelWidth(120)]
    [SerializeField, TextArea(2, 3)] private string _tooltip;

    public Sprite Icon => _icon;
    public string DisplayName => _displayName;
    public string Tooltip => _tooltip;

    /// <summary>
    /// Called when the player clicks this ability button.
    /// The entity that owns this ability is passed in as ISelectable.
    /// Cast to the type you need (BuildingCore, UnitController, etc.)
    /// </summary>
    public abstract void Execute(ISelectable entity);
}