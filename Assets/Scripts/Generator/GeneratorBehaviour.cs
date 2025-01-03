using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorBehaviour : MonoBehaviour
{
    [Header("Energy")]
    [SerializeField]
    private float maxEnergy;
    [SerializeField]
    private float energyDrain;
    private float additionnalEnergyDrain;
    private float currentEnergy;
    [SerializeField]
    private Slider energyGauge;
    [SerializeField]
    private TMPro.TextMeshProUGUI timerText;
    [SerializeField]
    private float fragmentConvertion = 10;

    [Header("Alarme")]
    [SerializeField]
    private float energyCriticalThreshold;
    [SerializeField]
    private Animator lightAlarm;
    private bool outOfEnergy;

    [Header("Lights")]
    [SerializeField]
    private List<Animator> lights = new List<Animator>();
    private List<float> intensities = new List<float>();

    private void Start()
    {
        currentEnergy = maxEnergy;
    }

    private void FixedUpdate()
    {
        if (currentEnergy > 0)
        {
            RemoveEnergie((energyDrain + additionnalEnergyDrain)*Time.fixedDeltaTime);
            UpdateText();
        }
    }

    private void UpdateText()
    {
        energyGauge.value = currentEnergy / maxEnergy;
        int minutes, secondes;
        //calcul minutes left
        minutes = (int)((currentEnergy / (energyDrain + additionnalEnergyDrain))/ 60);
        secondes = (int)((currentEnergy / (energyDrain + additionnalEnergyDrain)) - (minutes * 60));

        // write time in XX:XX format
        if(minutes < 10)
        {
            if(secondes < 10) timerText.text = new string("0" + minutes + ":0" + secondes);
            else timerText.text = new string("0" + minutes + ":" + secondes);
        }
        else
        {
            if (secondes < 10) timerText.text = new string(minutes + ":0" + secondes);
            else timerText.text = new string(minutes + ":" + secondes);
        }
    }

    private void OutOfEnergy()
    {
        outOfEnergy = true;
        foreach(Animator light in lights)
        {
            light.SetBool("Power", false);
        }
    }

    private void EnergyBack()
    {
        outOfEnergy = false;
        foreach (Animator light in lights)
        {
            light.SetBool("Power", true);
        }
    }

    #region change energy
    public void AddFragment(float nbFragment)
    {
        if(currentEnergy <= 0)
        {
            EnergyBack();
        }
        currentEnergy += nbFragment * fragmentConvertion;
        if(currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
        if(currentEnergy > energyCriticalThreshold)
        {
            lightAlarm.SetBool("LowEnergy", false);
        }
    }

    private void RemoveEnergie(float energieToRemove)
    {
        currentEnergy -= energieToRemove;
        if (currentEnergy <= 0)
        {
            currentEnergy = 0;
            if (!outOfEnergy) OutOfEnergy();
        }
        else if(currentEnergy < energyCriticalThreshold)
        {
            lightAlarm.SetBool("LowEnergy", true);
        }
    }
    #endregion

    #region drain
    public void AddDrain(float drain)
    {
        additionnalEnergyDrain += drain;
    }

    public void RemoveDrain(float drain)
    {
        additionnalEnergyDrain -= drain;
    }
    #endregion

    #region Getter

    public float GetEnergy() { return currentEnergy; }

    #endregion
}
