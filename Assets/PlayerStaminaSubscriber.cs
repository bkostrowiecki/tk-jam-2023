using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class PlayerStaminaSubscriber : MonoBehaviour
{
    public PlayerController playerController;
    public Slider slider;
    public MMF_Player updateFeedbacks;
    int currentStamina;
    int maxStamina;

    void Start()
    {
        playerController.CurrentStaminaObservable.Subscribe((x) =>
        {
            currentStamina = x;
            UpdateSlider();
        });

        playerController.MaxStaminaObservable.Subscribe((x) =>
        {
            maxStamina = x;
            UpdateSlider();
        });
    }

    void UpdateSlider()
    {
        slider.maxValue = maxStamina;
        slider.value = currentStamina;

        updateFeedbacks?.PlayFeedbacks();
    }
}
