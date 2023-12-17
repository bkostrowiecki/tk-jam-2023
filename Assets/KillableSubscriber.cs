using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using MoreMountains.Feedbacks;

public class KillableSubscriber : MonoBehaviour
{
    public Killable killable;
    public Slider slider;

    int current;
    int max;

    public MMF_Player updateFeedbacks;

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

        updateFeedbacks?.PlayFeedbacks();
    }
}
