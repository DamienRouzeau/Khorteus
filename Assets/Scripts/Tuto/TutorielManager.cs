using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorielManager : MonoBehaviour
{
    private static TutorielManager Instance { get; set; }
    public static TutorielManager instance => Instance;

    [SerializeField] private GeneratorBehaviour generator;
    [SerializeField] private IntroBehaviour intro;
    [SerializeField] private List<string> tutoTxt;
    [SerializeField] private List<TutoTrigger> tutoTriggers;
    [SerializeField] private Player.FirstPersonController player;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i< tutoTxt.Count; i++)
        {
            tutoTxt[i] = tutoTxt[i].Replace("\\n", "\n");
        }
        generator.SubCriticalEnergy(TriggerOOPActivation);
        generator.RemoveEnergie(78.25f);
        player.SubGetCrystal(TriggerCrystalActivation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerDetected(TutoTrigger trigger)
    {
        intro.SetText(tutoTxt[trigger.index]);
    }

    private void TriggerOOPActivation()
    {
        tutoTriggers[0].ActiveTrigger();
        generator.UnsubCriticalEnergy(TriggerOOPActivation);
    }

    private void TriggerCrystalActivation()
    {
        tutoTriggers[1].ActiveTrigger();
        player.UnsubGetCrystal(TriggerCrystalActivation);
    }
}
