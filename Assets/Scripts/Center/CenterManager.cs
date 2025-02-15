using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterManager : MonoBehaviour
{
    private Audio audio = new();
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && audio != null) audio.FadeOut();
        audio = null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && audio == null) audio = AudioManager.instance.FadeIn(transform, "CaveAmbiant", 0.15f);
    }
}
