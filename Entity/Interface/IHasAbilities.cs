using System.Collections.Generic;

/// <summary>
/// Implemented by any entity that exposes abilities to the HUD.
/// BuildingCore, UnitController, etc. implement this by delegating to their SO.
/// </summary>
public interface IHasAbilities
{
    List<AbilityBaseSO> Abilities { get; }
}