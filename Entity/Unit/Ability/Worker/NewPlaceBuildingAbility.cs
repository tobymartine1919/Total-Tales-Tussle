using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Worker ability: clicking the button enters building placement mode.
/// Fires OnPlacementRequested so PlayerController can transition to PlacingState.
///
/// Create via: FairyTaleRTS/Ability/Place Building
/// </summary>
[CreateAssetMenu(fileName = "NewPlaceBuildingAbility", menuName = "FairyTaleRTS/Ability/Place Building")]
public class PlaceBuildingAbilitySO : AbilityBaseSO
{
    [Title("Building Settings")]
    [SerializeField, Required] private GameObject _buildingPrefab;

    [Title("Resource Cost")]
    [SerializeField] private List<ResourceCost> _costs = new();

    /// <summary>
    /// Fired when a worker requests placement.
    /// Params: (buildingPrefab, workerTeam)
    /// PlayerController listens to this and transitions to PlacingState.
    /// </summary>
    public static event Action<GameObject, Team> OnPlacementRequested;

    public override void Execute(ISelectable entity)
    {
        if (_buildingPrefab == null)
        {
            Debug.LogWarning($"[{DisplayName}] Building prefab is not assigned.");
            return;
        }

        // Only workers should have this ability
        if (entity is not UnitController unit)
        {
            Debug.LogWarning($"[{DisplayName}] Execute called on a non-unit entity.");
            return;
        }

        if (!CanAfford(unit.Team))
        {
            Debug.LogWarning($"[{DisplayName}] {unit.Team} cannot afford this building.");
            return;
        }

        SpendCost(unit.Team);
        OnPlacementRequested?.Invoke(_buildingPrefab, unit.Team);
    }

    public bool CanAfford(Team team)
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

[Serializable]
public struct ResourceCost
{
    public ResourceType Type;
    [Min(0)] public int Amount;
}