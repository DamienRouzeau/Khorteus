using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MeleeWeaponsType
{
    Wrench,
    Axe,
    BigHammer,
    Blade,
    CircularSaw,
    Dagger,
    Hammer,
    Mace,
    Pipe,
    SpikeMace
}

[CreateAssetMenu(fileName = "MeleeWeapon", menuName = "ScriptableObject/MeleeWeapon", order = 5)]
public class MeleeData : ScriptableObject
{
    public MeleeWeaponsType type;
    public int cost;
    public int upgradeID; // should be the same number as the index in UpgradeManager List
    public bool isUnlocked;
}
