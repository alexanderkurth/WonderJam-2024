using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEndRound : MonoBehaviour
{
    bool isTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (isTriggered)
            return;

        ChevalierMove chevalier = other.gameObject.GetComponent<ChevalierMove>();
        if(chevalier != null)
        {
            isTriggered = true;
            mMadnessManager.Instance.Reset();
            GameMode.Instance.DayEnd();
        }
    }
}
