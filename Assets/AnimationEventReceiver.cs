using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnAnimationEvent : UnityEvent {}

[System.Serializable]
public struct AnimationEventHandler
{
    public string eventString;
    public OnAnimationEvent onEvent;
}

public class AnimationEventReceiver : MonoBehaviour
{
    public List<AnimationEventHandler> handlers;

    public void Event(string str)
    {
        for (int i = 0; i < handlers.Count; i++)
        {
            if (handlers[i].eventString == str)
            {
                handlers[i].onEvent.Invoke();
            }
        }
    }
}
