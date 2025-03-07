using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoTrigger : MonoBehaviour
{
    public int index;
    private bool isAlreadyTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isAlreadyTriggered)
        {
            TutorielManager.instance.TriggerDetected(this);
            isAlreadyTriggered = true;
        }
    }

    public void ActiveTrigger()
    {
        if (!isAlreadyTriggered)
        {
            TutorielManager.instance.TriggerDetected(this);
            isAlreadyTriggered = true;
        }
    }
}
