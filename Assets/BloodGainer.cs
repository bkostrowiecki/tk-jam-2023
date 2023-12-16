using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodGainer : MonoBehaviour
{
    public float delay;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);

        var player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().GainBlood();
    }
}
