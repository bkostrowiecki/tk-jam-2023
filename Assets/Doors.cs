using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Lever lever;
    public MMF_Player onFeedbacks;
    public MMF_Player offFeedbacks;

    void Start()
    {
        lever.MadeOn.AddListener(HandleMadeOn);
        lever.MadeOff.AddListener(HandleMadeOff);
    }

    void HandleMadeOff()
    {
        offFeedbacks?.PlayFeedbacks();
    }

    void HandleMadeOn()
    {
        onFeedbacks?.PlayFeedbacks();
    }
}
