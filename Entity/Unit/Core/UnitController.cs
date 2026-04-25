using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class UnitController : MonoBehaviour, ISelectable, IHasAbilities
{
    // ── Settings ──────────────────────────────────────────────────────────

    [Title("Settings")]
    [SerializeField, Required] private UnitSetting _setting;
    public UnitSetting Setting => _setting;

    [SerializeField] private Team _team;
    public Team Team => _team;
    public string DisplayName => _setting.UnitName;
    public Sprite Icon => _setting.UnitIcon;

    // ── IHasAbilities ─────────────────────────────────────────────────────

    public List<AbilityBaseSO> Abilities => _setting.Abilities;

    // ── Components ────────────────────────────────────────────────────────

    [Title("References")]
    [SerializeField, ReadOnly] public UnitMovement Movement;
    [SerializeField, ReadOnly] public UnitHealth Health;
    [SerializeField, ReadOnly] public UnitSensor Sensor;

    [Button("Fetch References"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchReferences()
    {
        Movement = GetComponentInChildren<UnitMovement>(true);
        Health = GetComponentInChildren<UnitHealth>(true);
        Sensor = GetComponentInChildren<UnitSensor>(true);
    }

    // ── Runtime Data ──────────────────────────────────────────────────────

    public UnitRuntimeData RuntimeData { get; private set; }

    // ── Internal Systems ──────────────────────────────────────────────────

    public UnitActionQueue UnitActionQueue { get; private set; }
    private UnitStateMachine _stateMachine;
    private UnitOrderHandler _orderHandler;

    // ── Events ────────────────────────────────────────────────────────────

    public event Action<Vector2> OnMoveOrdered;
    public event Action<IDamageable> OnAttackOrdered;
    public event Action<ResourceNodeCore> OnGatherOrdered;
    public event Action<BuildingCore> OnConstructOrdered;
    public event Action OnStopOrdered;
    public event Action OnSelected;
    public event Action OnDeselected;

    // ── Unity ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        RuntimeData = new UnitRuntimeData(_setting);

        Movement.Init(_setting);
        Health.Init(_setting);
        Sensor.Init(this, _setting);

        UnitActionQueue = new UnitActionQueue();
        _stateMachine = new UnitStateMachine(this);
        _orderHandler = new UnitOrderHandler(this, _stateMachine);
    }

    private void OnEnable()
    {
        OnMoveOrdered += HandleMove;
        OnAttackOrdered += HandleAttack;
        OnGatherOrdered += HandleGather;
        OnConstructOrdered += HandleConstruct;
        OnStopOrdered += HandleStop;
        OnSelected += HandleSelected;
        OnDeselected += HandleDeselected;
        Health.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        OnMoveOrdered -= HandleMove;
        OnAttackOrdered -= HandleAttack;
        OnGatherOrdered -= HandleGather;
        OnConstructOrdered -= HandleConstruct;
        OnStopOrdered -= HandleStop;
        OnSelected -= HandleSelected;
        OnDeselected -= HandleDeselected;
        Health.OnDeath -= HandleDeath;
    }

    private void OnDestroy() => _stateMachine.Dispose();

    // ── Event Handlers ────────────────────────────────────────────────────

    private void HandleMove(Vector2 pos) => _orderHandler.IssueMove(pos);
    private void HandleAttack(IDamageable d) => _orderHandler.IssueAttack(d);
    private void HandleGather(ResourceNodeCore r) => _orderHandler.IssueCollect(r);
    private void HandleConstruct(BuildingCore b) => _orderHandler.IssueBuild(b);
    private void HandleStop() => _orderHandler.IssueStop();
    private void HandleSelected() => Debug.Log($"[{name}] Selected");
    private void HandleDeselected() => Debug.Log($"[{name}] Deselected");
    private void HandleDeath() => _stateMachine.ChangeMajorState(MajorStateEnum.Death);

    // ── Public API ────────────────────────────────────────────────────────

    public void IssueMove(Vector2 pos) => OnMoveOrdered?.Invoke(pos);
    public void IssueAttack(IDamageable d) => OnAttackOrdered?.Invoke(d);
    public void IssueGather(ResourceNodeCore r) => OnGatherOrdered?.Invoke(r);
    public void IssueConstruct(BuildingCore b) => OnConstructOrdered?.Invoke(b);
    public void IssueStop() => OnStopOrdered?.Invoke();

    // ISelectable
    public void Select() => OnSelected?.Invoke();
    public void Deselect() => OnDeselected?.Invoke();

    // ── Resource Helpers ──────────────────────────────────────────────────

    public void DropResource() => RuntimeData.DropResource();

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
    // ═══════════════════════════════════════════
}