using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObject/Wave", order = 1)]
public class Wave : ScriptableObject
{
    public List<PairEnemyNB> waveComposition;
    public int doorsOpen;
}
