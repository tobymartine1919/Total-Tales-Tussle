using UnityEngine;

public interface ISelectable
{
    Team Team { get; }
    string DisplayName { get; }
    Sprite Icon { get; }
    void Select();
    void Deselect();
}