using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum UnitCapability
{
    Fight = 1 << 0,
    Collect = 1 << 1,
    Build = 1 << 2,
    Heal = 1 << 3
}

[CreateAssetMenu(fileName = "NewUnitSetting", menuName = "FairyTaleRTS/Unit Setting")]
public class UnitSetting : ScriptableObject
{
    [HorizontalGroup("Split", 64)]
    [PreviewField(64, ObjectFieldAlignment.Left)]
    [HideLabel]
    [SerializeField] private Sprite unitIcon;

    [VerticalGroup("Split/Right")]
    [LabelWidth(80)]
    [SerializeField] private string unitName;

    [VerticalGroup("Split/Right")]
    [LabelWidth(80)]
    [EnumToggleButtons]
    [SerializeField] private UnitCapability capabilities;

    [Title("Movement & Health")]
    [SerializeField] public float speed = 3f;
    [SerializeField] public float hp = 100f;

    [ShowIf("CanFight")]
    [BoxGroup("Combat Stats")]
    [SerializeField] public float atk = 10f;

    [ShowIf("CanFight")]
    [BoxGroup("Combat Stats")]
    [SerializeField] public float attackRange = 1.5f;

    [ShowIf("CanFight")]
    [BoxGroup("Combat Stats")]
    [SerializeField] public float attackInterval = 1f;

    [ShowIf("CanFight")]
    [BoxGroup("Combat Stats")]
    [SerializeField] public float detectionRange = 5f;

    [ShowIf("CanCollect")]
    [BoxGroup("Utility Stats")]
    [SerializeField] public float collectingSpeed = 2f;

    [ShowIf("CanCollect")]
    [BoxGroup("Utility Stats")]
    [SerializeField] public float carryCapacity = 10f;

    [ShowIf("CanBuild")]
    [BoxGroup("Utility Stats")]
    [SerializeField] public float constructionSpeed = 1f;

    // ── Abilities ─────────────────────────────────────────────────────────

    [Title("Abilities Configuration")]
    [AssetsOnly]
    public List<AbilityBaseSO> Abilities = new List<AbilityBaseSO>();

    // ── Accessors ─────────────────────────────────────────────────────────

    public string UnitName => unitName;
    public Sprite UnitIcon => unitIcon;

    public bool Has(UnitCapability cap) => (capabilities & cap) != 0;

    private bool CanFight => Has(UnitCapability.Fight);
    private bool CanCollect => Has(UnitCapability.Collect);
    private bool CanBuild => Has(UnitCapability.Build);
}