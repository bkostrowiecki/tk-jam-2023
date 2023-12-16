using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ApplyParentForInstantiateFeedback : MonoBehaviour
{
    public MMF_Player player;
    public string parentName;

    public string feedbackName = "Instantiate Blood Splat";

    void OnEnable()
    {
        var parent = GameObject.Find(parentName);

        var feedback = player.FeedbacksList.Find((item) => {
            return item.Label == feedbackName;
        });

        var casted = (MMF_InstantiateObject)feedback;

        casted.ParentTransform = parent.transform;
    }
}
