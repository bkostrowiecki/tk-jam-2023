using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class KillableSubscriber : MonoBehaviour
{
    public Killable killable;
    public Slider slider;

    int current;
    int max;

    void Start()
    {
        killable.CurrentHealthPointsObservable.Subscribe((hp) =>
        {
            current = hp;
            UpdateSlider();
        });

        killable.MaxHealthPointsObservable.Subscribe((hp) =>
        {
            max = hp;
            UpdateSlider();
        });
    }

    void UpdateSlider()
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
