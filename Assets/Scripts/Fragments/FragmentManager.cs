using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentManager : MonoBehaviour
{
    private static FragmentManager Instance { get; set; }
    public static FragmentManager instance => Instance;

    private List<FragmentBehaviour> crystals = new List<FragmentBehaviour>();
    [SerializeField]
    private List<Transform> crystalSpawns = new List<Transform>();
    private List<bool> spawnDisponibilities = new List<bool>();

    [Header("Spawn")]
    [SerializeField]
    private int maxCrystal;
    [SerializeField]
    private float crystalSpawnCooldown;
    private float currentWaitingTime;
    [SerializeField]
    private FragmentBehaviour crystalToSpawn;

    [Header("Crystal Type")]
    [SerializeField]
    private FragmentType littleCrystal, mediumCrystal, bigCrystal;

    private void Start()
    {
        foreach(Transform trans in crystalSpawns)
        {
            spawnDisponibilities.Add(true);
        }
    }


    private void FixedUpdate()
    {
        if(crystals.Count < maxCrystal) currentWaitingTime += Time.fixedDeltaTime;
        if(currentWaitingTime >= crystalSpawnCooldown && crystals.Count < maxCrystal)
        {
            SpawnCrystal();
            currentWaitingTime = 0;
        }
    }

    private void SpawnCrystal()
    {
        List<FragmentType> type = new List<FragmentType>();
        int i = 0;
        int y = 0;
        int z = 0;
        while (i < littleCrystal.probability)
        {
            type.Add(littleCrystal);
            i++;
        }
        while (y < mediumCrystal.probability)
        {
            type.Add(mediumCrystal);
            y++;
        }
        while (z < bigCrystal.probability)
        {
            type.Add(bigCrystal);
            z++;
        }
        bool canSpawn = false;
        Transform trans;
        do
        {
            trans = crystalSpawns[Random.Range(0, crystalSpawns.Count)];
            if (spawnDisponibilities[crystalSpawns.IndexOf(trans)])
            {
                canSpawn = true;
                spawnDisponibilities[crystalSpawns.IndexOf(trans)] = false;
            }
        }
        while (!canSpawn);

        FragmentBehaviour newFragment = Instantiate(crystalToSpawn, trans.position, trans.rotation, trans);
        newFragment.Init(this);
        newFragment.SetEvolution(type[Random.Range(0, type.Count)]);
        crystals.Add(newFragment);
    }

    public void DestroyCrystal(FragmentBehaviour crystalToDestroy)
    {
        spawnDisponibilities[crystalSpawns.IndexOf(crystalToDestroy.transform.parent)] = true;
        crystals.Remove(crystalToDestroy);
        Destroy(crystalToDestroy.gameObject);
    }
}
