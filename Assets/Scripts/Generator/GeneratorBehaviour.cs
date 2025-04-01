using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GeneratorBehaviour : MonoBehaviour
{
    private static GeneratorBehaviour Instance { get; set; }
    public static GeneratorBehaviour instance => Instance;

    [Header("Energy")]
    [SerializeField]
    private float maxEnergy;
    [SerializeField]
    private float energyDrain;
    private float additionnalEnergyDrain;
    private float currentEnergy;
    [SerializeField]
    private float fragmentConvertion = 10;
    UnityEvent outOfPowerEvent;
    UnityEvent powerBack;
    UnityEvent criticalPower;
    UnityEvent addPower;

    [Header("UI")]
    [SerializeField]
    private Slider energyGauge;
    [SerializeField]
    private TMPro.TextMeshProUGUI timerText;
    [SerializeField]
    private TMPro.TextMeshProUGUI energyText;
    [SerializeField] 
    private TMPro.TextMeshProUGUI drainText;


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

    [Header("Audio")]
    private Audio ambiantAudio;
    private Audio alarmAudio;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        ambiantAudio = AudioManager.instance.PlayAudio(transform, "GeneratorAmbiant");
        currentEnergy = maxEnergy;
        outOfPowerEvent = new UnityEvent();
        powerBack = new UnityEvent();
        criticalPower = new UnityEvent();
        addPower = new UnityEvent();
        foreach (Animator light in lights)
        {
            light.SetBool("Power", true);
        }
        UpdateDrain();
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
        int truncateEnergy = (int)currentEnergy +1;
        energyText.text = truncateEnergy + " W";
    }

    private void OutOfEnergy()
    {
        ambiantAudio.Stop();
        AudioManager.instance.PlayAudio(transform, "ShutDown");
        ambiantAudio = null;
        outOfEnergy = true;
        alarmAudio.Stop();
        outOfPowerEvent.Invoke();
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
        ambiantAudio = AudioManager.instance.PlayAudio(transform, "GeneratorAmbiant");
        powerBack.Invoke();
    }

    #region change energy
    public void AddFragment(float nbFragment)
    {
        addPower.Invoke();
        AudioManager.instance.PlayAudio(transform, "CrystalSave");
        if (currentEnergy <= 0)
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
            if(alarmAudio != null)
            {
                alarmAudio.Stop();
            }
        }
    }

    public void RemoveEnergie(float energieToRemove)
    {
        currentEnergy -= energieToRemove;
        if (currentEnergy <= 0)
        {
            currentEnergy = 0;
            if (!outOfEnergy) OutOfEnergy();
        }
        else if(currentEnergy < energyCriticalThreshold)
        {
            // Event Critical Energy
            criticalPower.Invoke();
            lightAlarm.SetBool("LowEnergy", true);
            if(alarmAudio == null)alarmAudio = AudioManager.instance.PlayAudio(transform, "GeneratorAlarm", 1);
        }
    }
    #endregion

    #region drain
    public void AddDrain(float drain)
    {
        additionnalEnergyDrain += drain;
        UpdateDrain();
    }

    private void UpdateDrain()
    {
        float drain = additionnalEnergyDrain + energyDrain;
        drain *= 100;
        int drainTrunc = (int)drain;
        drain = drainTrunc;
        drain /= 100;
        drainText.text = drain.ToString();
    }

    public void RemoveDrain(float drain)
    {
        additionnalEnergyDrain -= drain;
        UpdateDrain();
    }
    #endregion

    #region Getter

    public float GetEnergy() { return currentEnergy; }

    #endregion

    #region SUB event
    public void SubOutOfPower(UnityAction action)
    {
        outOfPowerEvent.AddListener(action);
    }
    public void SubPowerBack(UnityAction action)
    {
        powerBack.AddListener(action);
    }
    public void SubCriticalEnergy(UnityAction action)
    {
        criticalPower.AddListener(action);
    }

    public void UnsubCriticalEnergy(UnityAction action)
    {
        criticalPower.RemoveListener(action);
    }
    public void SubAddEnergy(UnityAction action)
    {
        addPower.AddListener(action);
    }

    public void UnsubAddEnergy(UnityAction action)
    {
        addPower.RemoveListener(action);
    }


    public void UnsubOutOfPower(UnityAction action)
    {
        outOfPowerEvent.RemoveListener(action);
    }
    #endregion
}
