using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
    public float radius;
    public Killable playerKillable;
    public LayerMask touchDamage;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Update()
    {
        var colliders = Physics.OverlapSphere(transform.position, radius, touchDamage);

        if (colliders.Count() > 0)
        {
            var collidersList = new List<Collider>(colliders);

            collidersList.Sort(ColliderComparer);

            var collider = collidersList[0];

            var damageOnTouch = collider.GetComponent<DamagePlayerOnTouch>();

            playerKillable.TakeDamage(new TouchDamage(damageOnTouch.damage));
        }
    }

    private int ColliderComparer(Collider x, Collider y)
    {
        var xMagnitude = (transform.position - x.transform.position).magnitude * 10;
        var yMagnitude = (transform.position - y.transform.position).magnitude * 10;

        var diff = xMagnitude - yMagnitude;

        return diff > 0 ? 1 : -1;
    }
}

class TouchDamage : BaseDamage
{
    private int damage;

    public TouchDamage(int damage)
    {
        this.damage = damage;
    }
    public int CalculateDamage()
    {
        return damage;
    }
}