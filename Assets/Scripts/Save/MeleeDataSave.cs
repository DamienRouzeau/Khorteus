using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeleeDataSave
{
    public UpgradeType type;
    public int cost;
    public int upgradeID; // should be the same number as the index in UpgradeManager List
    public bool isUnlocked;


    // Constructeur pour remplir la classe avec les données de UpgradeData
    public MeleeDataSave(MeleeData upgradeData)
    {
        cost = upgradeData.cost;
        upgradeID = upgradeData.upgradeID;
        isUnlocked = upgradeData.isUnlocked;
    }
}
