using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCore : MonoBehaviour, ISelectable, IConstructable, IDamageable, IHasAbilities
{
    [Title("Settings")]
    [SerializeField, Required] private BuildingSO _setting;
    [SerializeField] private BuildingType _type;
    [SerializeField] private Team _team;

    public List<AbilityBaseSO> Abilities => _setting.Abilities;

    public BuildingSO Setting => _setting;
    public BuildingType Type => _type;
    public Team Team => _team;
    public Vector2 Position => transform.position;
    public string DisplayName => _setting.DisplayName;
    public Sprite Icon => _setting.Icon;

    [Title("State (Runtime)")]
    [SerializeField, ReadOnly] private float _currentHp;
    [SerializeField, ReadOnly] private float _constructionProgress;
    [SerializeField, ReadOnly] private float _storedResource;

    public float CurrentHp => _currentHp;
    public float MaxHp => _setting.maxHp;
    public bool IsDead => _currentHp <= 0f;
    public float ConstructionProgress => _constructionProgress;
    public bool IsComplete => _constructionProgress >= 1f;
    public float StoredResource => _storedResource;

    public event Action OnCompleted;
    public event Action OnDestroyed;
    public event Action<float> OnResourceReceived;
    public event Action<UnitController> OnWorkerDeliveredEvent;
    public event Action OnDamaged;

    /// <summary>
    /// Fired by BuildingAccessZone when a unit assigned to construct this building enters the zone.
    /// </summary>
    public event Action<UnitController> OnWorkerArrivedForConstructionEvent;

    // ── Construction Color ─────────────────────────────────────────────────

    [Title("Construction Color (Debug)")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _goalColor = Color.green;
    [SerializeField, Range(0.5f, 1f)] private float _colorFillRange = 0.675f;

    // ── Spawn System ───────────────────────────────────────────────────────

    [Title("Spawn")]
    [Tooltip("Child Transform — exact world position where units appear on spawn.")]
    [SerializeField] private Transform _spawnPoint;

    [ShowInInspector, ReadOnly]
    private Vector2 _rallyPoint;

    private readonly Queue<SpawnRequest> _spawnQueue = new();
    private Timer _spawnTimer;
    private bool _isSpawning;

    private struct SpawnRequest
    {
        public GameObject Prefab;
        public float Duration;
    }

    // ── Unity ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _currentHp = _setting.maxHp;
        _constructionProgress = 0f;
        _storedResource = 0f;
    }

    private void Start()
    {
        BuildingManager.I.Register(_type, this);
        RefreshColor();

        // Rally point defaults to spawn point world position (or building center if unassigned)
        _rallyPoint = _spawnPoint != null
            ? (Vector2)_spawnPoint.position
            : (Vector2)transform.position;
    }

    private void Update()
    {
        if (!IsComplete)
            RefreshColor();

        // Tick spawn timer
        if (_isSpawning && _spawnTimer != null && _spawnTimer.IsReady)
            FinishSpawn();
    }

    // ── IConstructable ────────────────────────────────────────────────────

    public void ApplyConstruction(float deltaTime)
    {
        if (IsComplete) return;

        _constructionProgress = Mathf.Clamp01(
            _constructionProgress + deltaTime / _setting.constructionDuration
        );

        if (IsComplete)
        {
            SnapColor();
            OnCompleted?.Invoke();
        }
    }

    // ── IDamageable ───────────────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        _currentHp = Mathf.Max(0f, _currentHp - amount);
        OnDamaged?.Invoke();

        if (IsDead)
            OnDestroyed?.Invoke();
    }

    /// <summary>
    /// Called by BuildingAccessZone — fires the construction arrival event from within the type.
    /// </summary>
    public void NotifyWorkerArrivedForConstruction(UnitController worker)
        => OnWorkerArrivedForConstructionEvent?.Invoke(worker);

    // ── Resource Delivery ─────────────────────────────────────────────────

    public void ReceiveResource(UnitController worker)
    {
        if (!worker.RuntimeData.IsDeliveringResource) return;
        if (worker.RuntimeData.CurrentHub != this) return;

        float amount = worker.RuntimeData.CarryAmount;
        ResourceType resourceType = worker.RuntimeData.CurrentNode?.ResourceType ?? ResourceType.Mana;

        _storedResource += amount;
        worker.DropResource();

        if (_team != Team.None && ResourceManager.IsValid())
            ResourceManager.I.AddResource(_team, resourceType, (int)amount);

        OnWorkerDeliveredEvent?.Invoke(worker);
        OnResourceReceived?.Invoke(amount);
    }

    // ── Spawn System ──────────────────────────────────────────────────────

    /// <summary>
    /// Called by SpawnUnitAbilitySO after cost is paid. Adds unit to the spawn queue.
    /// </summary>
    public void EnqueueSpawn(GameObject unitPrefab, float duration)
    {
        _spawnQueue.Enqueue(new SpawnRequest { Prefab = unitPrefab, Duration = duration });

        if (!_isSpawning)
            StartNextSpawn();
    }

    /// <summary>
    /// Called by SelectionIdleState when the player right-clicks ground with this building selected.
    /// </summary>
    public void SetRallyPoint(Vector2 position)
    {
        _rallyPoint = position;
    }

    private void StartNextSpawn()
    {
        if (_spawnQueue.Count == 0)
        {
            _isSpawning = false;
            return;
        }

        _isSpawning = true;
        var req = _spawnQueue.Peek();
        _spawnTimer = new Timer(req.Duration);
        _spawnTimer.Start();
    }

    private void FinishSpawn()
    {
        var req = _spawnQueue.Dequeue();

        Vector2 spawnPos = _spawnPoint != null
            ? (Vector2)_spawnPoint.position
            : (Vector2)transform.position;

        var go = GameObject.Instantiate(req.Prefab, spawnPos, Quaternion.identity);

        if (go.TryGetComponent<UnitController>(out var unit))
            unit.IssueMove(_rallyPoint);

        StartNextSpawn();
    }

    // ── ISelectable ───────────────────────────────────────────────────────

    public void Select() { }
    public void Deselect() { }

    // ── Color Helpers ─────────────────────────────────────────────────────

    private void RefreshColor()
    {
        if (_spriteRenderer == null) return;
        float t = Mathf.Clamp01(_constructionProgress / _colorFillRange);
        _spriteRenderer.color = Color.Lerp(Color.white, _goalColor, t);
    }

    private void SnapColor()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = _goalColor;
    }

    // ── Gizmos ────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        // Magenta sphere = exact spawn position
        if (_spawnPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_spawnPoint.position, 0.2f);
        }

        // Cyan sphere + line = rally point
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_rallyPoint, 0.15f);

            Vector3 from = _spawnPoint != null ? _spawnPoint.position : transform.position;
            Gizmos.DrawLine(from, _rallyPoint);
        }
    }

    // ── Debug ─────────────────────────────────────────────────────────────

    [Button("Fetch References"), GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchReferences()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }
}