using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider self, orange, yellow;

    public void UpdateHealthBar()
    {
        orange.value = self.value;
        yellow.value = self.value;
    }

}
