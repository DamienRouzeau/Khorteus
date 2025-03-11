using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class TutorielManager : MonoBehaviour
{
    private static TutorielManager Instance { get; set; }
    public static TutorielManager instance => Instance;

    [Header("Tuto")]
    [SerializeField] private IntroBehaviour intro;
    [SerializeField] private List<string> tutoTxt;
    [SerializeField] private List<TutoTrigger> tutoTriggers;
    [SerializeField] private Light[] lights;
    [SerializeField] private MeshRenderer[] lightGO;
    [SerializeField] private Material noEmission, yellowEmission, whiteEmission;
    [SerializeField] private InputToString I2S;
    [SerializeField] private Image spriteCommand;


    [Header("Miscelaneous")]
    [SerializeField] private GeneratorBehaviour generator;
    [SerializeField] private Player.FirstPersonController player;
    [SerializeField] private Animator doorAlarmAnim;
    private Audio alarmSound;
    private InventorySystem inventory;

    [Header("Enemy")]
    [SerializeField] PairEnemyNB enemy;
    [SerializeField] Transform enemySpawn;
    private EnemyBehaviour monster;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < tutoTxt.Count; i++)
        {
            tutoTxt[i] = tutoTxt[i].Replace("\\n", "\n"); // Allow back to line
        }
        generator.SubCriticalEnergy(TriggerOOPActivation);
        generator.RemoveEnergie(78.75f);
        player.SubGetCrystal(TriggerCrystalActivation);
        player.canInteractWithDesktop = false;
        player.canInteractWithGenerator = false;
        player.canInteractWithTransfert = false;
        player.canInteractWithHeal = false;
        generator.SubAddEnergy(TriggerAddEnergyActivation);
        inventory = player.GetInventory();
        inventory.SubAddItem(TriggerCraftTurret);
        player.TakeDamage(25);
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
        spriteCommand.sprite = I2S.InputToImage(Key.E);
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
        lights[0].intensity = 1.5f;
        lights[0].color = Color.yellow;
        lightGO[0].material = yellowEmission;
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
        spriteCommand.sprite = I2S.InputToImage(MouseButton.Left);
        tutoTriggers[3].ActiveTrigger();
    }

    private void TriggerPlaceTurret()
    {
        Debug.Log("[TUTO] Trigger Turret placed");
        inventory.UnsubRemoveItem(TriggerPlaceTurret);
        player.canInteractWithDesktop = false;
        monster = Instantiate(enemy.enemyType, enemySpawn);
        monster.SetPlayerRef(player.transform);
        monster.SetGeneratorRef(generator.transform, player.transform);
        monster.SetHidingSpots(new List<Transform> { player.transform });
        lights[0].color = Color.white;
        lights[0].intensity = 1f;
        lightGO[0].material = whiteEmission;
        monster.SubDie(TriggerEnemyDie);
        StartCoroutine(StopAlarm());
    }

    private IEnumerator StopAlarm()
    {
        yield return new WaitForSeconds(2.5f);
        doorAlarmAnim.SetBool("Alerte", false);
        alarmSound.Stop();
    }

    private void TriggerEnemyDie()
    {
        monster.UnsubDie(TriggerEnemyDie);
        Debug.Log("[TUTO] Trigger Monster killed");
        player.canInteractWithHeal = true;
        tutoTriggers[4].ActiveTrigger();
        lights[1].intensity = 1.5f;
        lights[1].color = Color.yellow;
        lightGO[1].material = yellowEmission;
        spriteCommand.sprite = I2S.InputToImage(Key.E);
        player.SubGetHealed(TriggerHeal);
    }

    private void TriggerHeal()
    {
        player.UnsubGetHealed(TriggerHeal);
        Debug.Log("[TUTO] Trigger Healed");
        player.canInteractWithHeal = false;
        player.canInteractWithTransfert = true;
        lights[1].color = Color.white;
        lights[1].intensity = 1f;
        lightGO[1].material = whiteEmission;
        lights[2].intensity = 1.5f;
        lights[2].color = Color.yellow;
        lightGO[2].material = yellowEmission;
        player.SubSaveCrystal(TriggerCrystalSaved);
        tutoTriggers[5].ActiveTrigger();
    }

    private void TriggerCrystalSaved()
    {
        player.UnsubSaveCrystal(TriggerCrystalSaved);
        Debug.Log("[TUTO] Trigger Healed");
        lights[2].color = Color.white;
        lights[2].intensity = 1f;
        lightGO[2].material = whiteEmission;
        player.canInteractWithTransfert = false;
        StartCoroutine(SpawnEnnemies());
        tutoTriggers[6].ActiveTrigger();
    }

    private IEnumerator SpawnEnnemies()
    {
        yield return new WaitForSeconds(2);
        doorAlarmAnim.SetBool("Alerte", true);
        spriteCommand.sprite = I2S.InputToImage(MouseButton.Left);
        alarmSound = AudioManager.instance.PlayAudio(doorAlarmAnim.gameObject.transform, "DoorAlarm", 0.5f);
        yield return new WaitForSeconds(1);
        tutoTriggers[6].ActiveTrigger();
        yield return new WaitForSeconds(1);
        StartCoroutine(StopAlarm());
        for(int i = 0; i<100; i++)
        {
            var monsterToSpawn = Instantiate(enemy.enemyType, enemySpawn);
            monsterToSpawn.SetPlayerRef(player.transform);
            monsterToSpawn.SetGeneratorRef(generator.transform, player.transform);
            monsterToSpawn.SetHidingSpots(new List<Transform> { player.transform });
            yield return new WaitForSeconds(0.75f);
        }
    }

    #endregion
}
