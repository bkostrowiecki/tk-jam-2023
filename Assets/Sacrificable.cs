using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sacrificable : MonoBehaviour
{
    public Animator animator;
    public string layDownTrigger;
    public string layDeadTrigger;
    public UnityEvent onStartSacrifice = new ();

    public float destroyAfterDeadTime = 1f;

    public void StartSacrifice()
    {
        onStartSacrifice?.Invoke();
        gameObject.layer = LayerMask.NameToLayer("Corpse");
    }

    public void MakeLayDown()
    {
        animator.SetTrigger(layDownTrigger);
    }
    public void MakeLayDead()
    {
        animator.SetTrigger(layDeadTrigger);
        Destroy(gameObject, destroyAfterDeadTime);
    }
}
