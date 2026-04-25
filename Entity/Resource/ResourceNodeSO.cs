using Sirenix.OdinInspector;
using UnityEngine;

public enum ResourceType { Mana, Hope, Paint  }

[CreateAssetMenu(fileName = "NewResourceNodeSetting", menuName = "FairyTaleRTS/Resource Node Setting")]
public class ResourceNodeSO : ScriptableObject
{
    [HorizontalGroup("Split", 64)]
    [PreviewField(64, ObjectFieldAlignment.Left)]
    [HideLabel]
    [SerializeField] private Sprite icon;

    [VerticalGroup("Split/Right")]
    [LabelWidth(120)]
    [SerializeField] private string displayName;

    [VerticalGroup("Split/Right")]
    [LabelWidth(120)]
    [SerializeField] public ResourceType resourceType = ResourceType.Mana;

    [VerticalGroup("Split/Right")]
    [LabelWidth(120)]
    [SerializeField] public int maxAmount = 1000;

    public string DisplayName => displayName;
    public Sprite Icon => icon;
}