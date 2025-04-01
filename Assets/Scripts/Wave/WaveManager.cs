using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private static WaveManager Instance { get; set; }
    public static WaveManager instance => Instance;

    [Header("Wave data")]
    public int currentWave = 0;
    [SerializeField]
    private List<Wave> wave;
    [SerializeField]
    private float preparationPhaseDuration;

    [Header("Doors")]
    [SerializeField]
    private List<Animator> doorAlerte;
    [SerializeField]
    private List<Transform> door;
    [SerializeField]
    private float alarmeDuration;

    [Header("Enemies prefabs")]
    [SerializeField] private GameObject critter;
    [SerializeField] private GameObject howler;

    [Header("References")]
    [SerializeField]
    private Transform playerRef;
    [SerializeField]
    private Transform generatorRef;
    [SerializeField]
    private Transform generatorAttackPosition;
    [SerializeField]
    private List<Transform> hidingSpots = new List<Transform>();
    private List<Audio> alarmsAudio = new();
    public Transform howlerTarget;
    


    private List<Transform> doorToSpawn = new List<Transform>();
    private List<GameObject> ennemies = new List<GameObject>();
    private bool spawnFinished;



    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        StartCoroutine(PreparationPhase());
        GeneratorBehaviour.instance.SubOutOfPower(ShutDownAlarms);
    }

    public void StartNewWave()
    {
        currentWave++;
        if (wave.Count < currentWave)
            currentWave = wave.Count;
        ChooseRandomDoor(wave[currentWave].doorsOpen);
        spawnFinished = false;
        foreach (PairEnemyNB pair in wave[currentWave].waveComposition)
        {
            for (int i = pair.numberToSpawn; i > 0; i--)
            {
                StartCoroutine(SpawnWithDelay(pair, i));
            }
        }
        spawnFinished = true;
    }

    public void RestartCurrentWave()
    {
        ChooseRandomDoor(10);
        spawnFinished = false;
        foreach (PairEnemyNB pair in wave[currentWave].waveComposition)
        {
            if (pair.enemyType.type == monsterTypes.critter)
            {
                for (int i = pair.numberToSpawn; i > 0; i--)
                {
                    StartCoroutine(SpawnWithDelay(pair, i));
                }
            }
        }
        spawnFinished = true;
    }

    public IEnumerator SpawnWithDelay(PairEnemyNB pair, int delay)
    {
        yield return new WaitForSeconds((pair.numberToSpawn - delay) * pair.intervalSpawn);
        Transform whereSpawn = doorToSpawn[Random.Range(0, doorToSpawn.Count)];
        var enemy = Instantiate(pair.enemyType, whereSpawn);
        enemy.SetPlayerRef(playerRef);
        enemy.SetGeneratorRef(generatorRef, generatorAttackPosition);
        enemy.SetHidingSpots(hidingSpots);
        ennemies.Add(enemy.gameObject);
        if(enemy.type == monsterTypes.howler)
        {
            enemy.priorityTarget = howlerTarget;
        }
        if (enemy.type == monsterTypes.spitter)
        {
            enemy.priorityTarget = playerRef;
        }
    }


    public void EnemyDied(GameObject enemy)
    {
        ennemies.Remove(enemy);
        if (ennemies.Count == 0 && spawnFinished)
        {
            StartCoroutine(PreparationPhase());
        }
    }

    public void ShutDownAlarms()
    {
        foreach (Audio audio in alarmsAudio)
        {
            audio.Stop();
        }
        alarmsAudio.Clear();
    }

    private IEnumerator PreparationPhase()
    {
        yield return new WaitForSecondsRealtime(preparationPhaseDuration);
        StartNewWave();
    }

    public void ChooseRandomDoor(int nbDoor)
    {
        if (doorToSpawn.Count > 0) doorToSpawn.Clear();
        for (int i = nbDoor; i > 0; i--)
        {
            Transform _door = door[Random.Range(0, door.Count)];
            doorToSpawn.Add(_door);
            doorAlerte[door.IndexOf(_door)].SetBool("Alerte", true);
            if (GeneratorBehaviour.instance.GetEnergy() > 0)
            {
                Audio doorAlarmAudio = AudioManager.instance.PlayAudio(doorAlerte[door.IndexOf(_door)].gameObject.transform, "DoorAlarm", 0.5f);
                alarmsAudio.Add(doorAlarmAudio);
                StartCoroutine(StopAlarme(doorAlerte[door.IndexOf(_door)], doorAlarmAudio));
            }
        }
    }

    private IEnumerator StopAlarme(Animator doorToStop, Audio audio)
    {
        yield return new WaitForSeconds(alarmeDuration);
        doorToStop.SetBool("Alerte", false);
        alarmsAudio.Remove(audio);
        if (audio != null) audio.Stop();
    }
}
