using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

[Serializable]
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
    public UnityEvent onDied = new UnityEvent();
    public OnDamageEvent onDamage = new OnDamageEvent();

    BehaviorSubject<int> currentHealthPointsSubject;

    public IObservable<int> CurrentHealthPointsObservable => currentHealthPointsSubject.AsObservable();

    BehaviorSubject<int> maxHealthPointsSubject;

    public IObservable<int> MaxHealthPointsObservable => maxHealthPointsSubject.AsObservable();

    void Awake()
    {
        currentHealthPoints = maxHealthPoints;
        currentHealthPointsSubject = new BehaviorSubject<int>(currentHealthPoints);
        maxHealthPointsSubject = new BehaviorSubject<int>(maxHealthPoints);
    }

    public void TakeDamage(BaseDamage damage)
    {
        var damageHealthPoints = damage.CalculateDamage();
        var cachedHealthPoints = currentHealthPoints;
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damageHealthPoints, 0, maxHealthPoints);

        currentHealthPointsSubject.OnNext(currentHealthPoints);

        onDamage?.Invoke(currentHealthPoints, cachedHealthPoints, damageHealthPoints);

        if (currentHealthPoints == 0)
        {
            Die();
        }
    }

    void MaxHealthIncrease(int increment)
    {
        maxHealthPoints += increment;
        maxHealthPointsSubject.OnNext(maxHealthPoints);
    }

    void SetMaxHealth(int maxHealth)
    {
        maxHealthPoints = maxHealth;
        maxHealthPointsSubject.OnNext(maxHealthPoints);
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
