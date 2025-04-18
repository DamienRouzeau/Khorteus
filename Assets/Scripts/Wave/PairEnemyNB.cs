using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PairEnemyNB", menuName = "ScriptableObject/PairEnemyNB", order = 2)]
public class PairEnemyNB : ScriptableObject
{
    public Enemy enemyType;
    public int numberToSpawn;
    public float intervalSpawn;
}
