using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Concrete ability: enqueues a unit spawn on the building that owns this ability.
/// The building handles the timer, instantiation, and rally-point move.
/// Create via: FairyTaleRTS/Ability/Spawn Unit
/// </summary>
[CreateAssetMenu(fileName = "NewSpawnUnitAbility", menuName = "FairyTaleRTS/Ability/Spawn Unit")]
public class SpawnUnitAbilitySO : AbilityBaseSO
{
    [Title("Spawn Settings")]
    [SerializeField, Required] private GameObject _unitPrefab;
    [SerializeField, Min(0.1f)] private float _spawnDuration = 5f;

    [Title("Resource Cost")]
    [SerializeField] private List<ResourceCost> _costs = new();

    public override void Execute(ISelectable entity)
    {
        if (_unitPrefab == null)
        {
            Debug.LogWarning($"[{DisplayName}] Unit prefab is not assigned.");
            return;
        }

        if (entity is not BuildingCore building)
        {
            Debug.LogWarning($"[{DisplayName}] Execute called on a non-building entity.");
            return;
        }

        if (!building.IsComplete)
        {
            Debug.Log($"[{DisplayName}] Building is not complete yet.");
            return;
        }

        if (!CanAfford(building.Team))
        {
            Debug.Log($"[{DisplayName}] {building.Team} cannot afford this unit.");
            return;
        }

        SpendCost(building.Team);
        building.EnqueueSpawn(_unitPrefab, _spawnDuration);
    }

    private bool CanAfford(Team team)
    {
        foreach (ResourceCost cost in _costs)
        {
            if (!ResourceManager.I.HasEnough(team, cost.Type, cost.Amount))
                return false;
        }
        return true;
    }

    private void SpendCost(Team team)
    {
        foreach (ResourceCost cost in _costs)
            ResourceManager.I.SpendResource(team, cost.Type, cost.Amount);
    }
}