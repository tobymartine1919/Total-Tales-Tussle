using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingSetting", menuName = "FairyTaleRTS/Building Setting")]
public class BuildingSO : ScriptableObject
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
    [SerializeField] public float maxHp = 500f;

    [VerticalGroup("Split/Right")]
    [LabelWidth(120)]
    [SerializeField] public float constructionDuration = 10f;

    [Title("Abilities Configuration")]
    [AssetsOnly]
    public List<AbilityBaseSO> Abilities = new List<AbilityBaseSO>();

    public string DisplayName => displayName;
    public Sprite Icon => icon;
}