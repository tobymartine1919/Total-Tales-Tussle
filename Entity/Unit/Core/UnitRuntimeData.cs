using UnityEngine;

/// <summary>
/// All mutable runtime state for a unit. Lives on UnitController.
/// States read and write through this — keeps controller and setting clean.
/// </summary>
public class UnitRuntimeData
{
    // ── Carry ─────────────────────────────────────────────────────────────
    public float CarryAmount { get; set; }
    public bool IsCarryFull => CarryAmount >= CarryCapacity;
    public bool IsCarryEmpty => CarryAmount <= 0f;

    // ── Delivery Intent ───────────────────────────────────────────────────
    public bool IsDeliveringResource { get; set; }

    // ── Cached Targets ────────────────────────────────────────────────────
    public ResourceNodeCore CurrentNode { get; set; }
    public BuildingCore CurrentHub { get; set; }
    public BuildingCore CurrentBuilding { get; set; }

    // ── Capacity (set once from setting) ──────────────────────────────────
    public float CarryCapacity { get; }

    public UnitRuntimeData(UnitSetting setting)
    {
        CarryCapacity = setting.carryCapacity;
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    public void PickupResource(float amount)
    {
        CarryAmount = Mathf.Min(CarryAmount + amount, CarryCapacity);
    }

    public void DropResource()
    {
        CarryAmount = 0f;
        IsDeliveringResource = false;
    }

    public void Reset()
    {
        CarryAmount = 0f;
        IsDeliveringResource = false;
        CurrentNode = null;
        CurrentHub = null;
        CurrentBuilding = null;
    }
}