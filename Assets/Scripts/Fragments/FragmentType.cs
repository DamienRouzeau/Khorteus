using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FragmentType", menuName = "ScriptableObject/FragmentType", order = 3)]
public class FragmentType : ScriptableObject
{
    public float maxHealth;
    public float health;
    public float probability;
    public float minTime;
    public float maxTime;
    public GameObject visual;
    public FragmentType nextEvolution;
    public int quantity;
}
