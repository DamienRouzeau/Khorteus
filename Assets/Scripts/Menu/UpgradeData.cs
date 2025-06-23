using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    Heal,
    Damage,
    Speed
}

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObject/Upgrade", order = 4)]
public class UpgradeData : ScriptableObject
{
    public UpgradeType type;
    public string title;
    public string description;
    public int cost;
    public int upgradeID; // should be the same number as the index in UpgradeManager List
    public bool isUnlocked;
    public int previousUpgradeID;
    public float incrementValue;
}
