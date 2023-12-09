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
    [Header("Damage")]
    public int maxHealthPoints = 100;
    int currentHealthPoints = 0;
    public bool canTakeDamage = true;
    public OnDamageEvent onDamage = new OnDamageEvent();

    [Header("Death")]
    public bool shouldDestroyOnDeath;
    public float destroyDelayOnDeath;
    public GameObject corpsePrefab;
    public UnityEvent onDied = new UnityEvent();

    BehaviorSubject<int> currentHealthPointsSubject;

    public IObservable<int> CurrentHealthPointsObservable => currentHealthPointsSubject.AsObservable();

    BehaviorSubject<int> maxHealthPointsSubject;

    public IObservable<int> MaxHealthPointsObservable => maxHealthPointsSubject.AsObservable();
    bool isDead = false;

    void Awake()
    {
        currentHealthPoints = maxHealthPoints;
        currentHealthPointsSubject = new BehaviorSubject<int>(currentHealthPoints);
        maxHealthPointsSubject = new BehaviorSubject<int>(maxHealthPoints);
    }

    public void TakeDamage(BaseDamage damage)
    {
        if (!canTakeDamage || isDead)
        {
            return;
        }

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
        isDead = true;
        if (corpsePrefab != null)
        {
            Instantiate(corpsePrefab, transform.position, transform.rotation);
        }

        onDied?.Invoke();

        if (shouldDestroyOnDeath)
        {
            Destroy(gameObject, destroyDelayOnDeath);
        }
    }

    public void MakeImmortal()
    {
        this.canTakeDamage = false;
    }

    public void MakeMortal()
    {
        this.canTakeDamage = true;
    }
}

public interface BaseDamage
{
    int CalculateDamage();
}
