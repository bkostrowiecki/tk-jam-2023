using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDamageEvent : UnityEvent<int, int, int>
{
    public int currentHealthPoints;
    public int previousHealthPoints;
    public int damageTaken;
}

public class Killable : MonoBehaviour
{
    public int maxHealthPoints = 100;
    int currentHealthPoints = 0;
    public UnityEvent onDied;
    public OnDamageEvent onDamage = new OnDamageEvent();

    void TakeDamage(BaseDamage damage)
    {
        var damageHealthPoints = damage.CalculateDamage();
        var cachedHealthPoints = currentHealthPoints;
        currentHealthPoints -= Mathf.Clamp(damageHealthPoints, 0, maxHealthPoints);

        onDamage?.Invoke(currentHealthPoints, cachedHealthPoints, damageHealthPoints);

        if (currentHealthPoints == 0)
        {
            Die();
        }
    }

    void Die()
    {
        onDied?.Invoke();
    }
}

public interface BaseDamage
{
    int CalculateDamage();
}
