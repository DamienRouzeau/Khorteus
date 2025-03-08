using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorielManager : MonoBehaviour
{
    private static TutorielManager Instance { get; set; }
    public static TutorielManager instance => Instance;

    [Header("Tuto")]
    [SerializeField] private IntroBehaviour intro;
    [SerializeField] private List<string> tutoTxt;
    [SerializeField] private List<TutoTrigger> tutoTriggers;

    [Header("Miscelaneous")]
    [SerializeField] private GeneratorBehaviour generator;
    [SerializeField] private Player.FirstPersonController player;
    [SerializeField] private Animator doorAlarmAnim;
    private Audio alarmSound;
    private InventorySystem inventory;

    [Header("Enemy")]
    [SerializeField] PairEnemyNB enemy;
    [SerializeField] Transform enemySpawn;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        for(int i = 0; i< tutoTxt.Count; i++)
        {
            tutoTxt[i] = tutoTxt[i].Replace("\\n", "\n"); // Allow back to line
        }
        generator.SubCriticalEnergy(TriggerOOPActivation);
        generator.RemoveEnergie(78.75f);
        player.SubGetCrystal(TriggerCrystalActivation);
        player.canInteractWithDesktop = false;
        player.canInteractWithGenerator = false;
        player.canInteractWithTransfert = false;
        generator.SubAddEnergy(TriggerAddEnergyActivation);
        inventory = player.GetInventory();
        inventory.SubAddItem(TriggerCraftTurret);

    }

    #region Triggers

    public void TriggerDetected(TutoTrigger trigger)
    {
        intro.SetText(tutoTxt[trigger.index]);
    }

    private void TriggerOOPActivation()
    {
        Debug.Log("[TUTO] Trigger POO");
        tutoTriggers[0].ActiveTrigger();
        generator.UnsubCriticalEnergy(TriggerOOPActivation);
    }

    private void TriggerCrystalActivation()
    {
        Debug.Log("[TUTO] Trigger Crystal");
        tutoTriggers[1].ActiveTrigger();
        player.UnsubGetCrystal(TriggerCrystalActivation);
        player.canInteractWithGenerator = true;
    }

    private void TriggerAddEnergyActivation()
    {
        Debug.Log("[TUTO] Trigger Add energy");
        player.UnsubGetCrystal(TriggerCrystalActivation);
        player.canInteractWithGenerator = false;
        player.canInteractWithDesktop = true;
        generator.UnsubAddEnergy(TriggerAddEnergyActivation);
        StartCoroutine(StartAlarm());
    }

    private IEnumerator StartAlarm()
    {
        yield return new WaitForSeconds(2.5f);
        doorAlarmAnim.SetBool("Alerte", true);
        alarmSound = AudioManager.instance.PlayAudio(doorAlarmAnim.gameObject.transform, "DoorAlarm", 0.5f);
        yield return new WaitForSeconds(1.25f);
        tutoTriggers[2].ActiveTrigger();
    }

    private void TriggerCraftTurret()
    {
        Debug.Log("[TUTO] Trigger Turret crafted");
        inventory.UnsubAddItem(TriggerCraftTurret);
        player.canInteractWithDesktop = false;
        inventory.SubRemoveItem(TriggerPlaceTurret);
        tutoTriggers[3].ActiveTrigger();
    }

    private void TriggerPlaceTurret()
    {
        Debug.Log("[TUTO] Trigger Turret placed");
        inventory.UnsubRemoveItem(TriggerPlaceTurret);
        player.canInteractWithDesktop = false;
        var monster = Instantiate(enemy.enemyType, enemySpawn);
        monster.SetPlayerRef(player.transform);
        monster.SetGeneratorRef(generator.transform, player.transform);
        monster.SetHidingSpots(new List<Transform>{player.transform});
        StartCoroutine(StopAlarm());
    }

    private IEnumerator StopAlarm()
    {
        yield return new WaitForSeconds(2.5f);
        doorAlarmAnim.SetBool("Alerte", false);
        alarmSound.Stop();
    }

    #endregion
}
