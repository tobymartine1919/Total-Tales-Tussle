using UnityEngine;

public interface IDamageable
{
    float CurrentHp { get; }
    float MaxHp { get; }
    bool IsDead { get; }
    Vector2 Position { get; }
    void TakeDamage(float amount);
}