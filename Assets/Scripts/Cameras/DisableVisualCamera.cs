using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVisualCamera : MonoBehaviour
{
    [SerializeField] GameObject visual;
    [SerializeField] GameObject light;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            light.SetActive(true);
            visual.SetActive(false);
        }
    }
}
