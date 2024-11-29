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

    [Header("Miscelaneous")]
    [SerializeField]
    private Transform playerRef;


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
    }

    public void StartNewWave()
    {
        currentWave++;
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

    public IEnumerator SpawnWithDelay(PairEnemyNB pair, int delay)
    {
        yield return new WaitForSeconds((pair.numberToSpawn - delay) * pair.intervalSpawn);
        Transform whereSpawn = doorToSpawn[Random.Range(0, doorToSpawn.Count)];
        var enemy = Instantiate(pair.enemyType, whereSpawn);
        enemy.SetPlayerRef(playerRef);
        ennemies.Add(enemy.gameObject);
    }


    public void EnemyDied(GameObject enemy)
    {
        ennemies.Remove(enemy);
        if(ennemies.Count == 0 && spawnFinished)
        {
            StartCoroutine(PreparationPhase());
        }
    }

    private IEnumerator PreparationPhase()
    {
        yield return new WaitForSecondsRealtime(preparationPhaseDuration);
        StartNewWave();
    }

    public void ChooseRandomDoor(int nbDoor)
    {
        if(doorToSpawn.Count > 0) doorToSpawn.Clear();
        for (int i = nbDoor; i > 0; i--)
        {
            Transform _door = door[Random.Range(0, door.Count)];
            doorToSpawn.Add(_door);
            doorAlerte[door.IndexOf(_door)].SetBool("Alerte", true);
            StartCoroutine(StopAlarme(doorAlerte[door.IndexOf(_door)]));
        }
    }

    private IEnumerator StopAlarme(Animator doorToStop)
    {
        yield return new WaitForSeconds(alarmeDuration);
        doorToStop.SetBool("Alerte", false);
    }
}
