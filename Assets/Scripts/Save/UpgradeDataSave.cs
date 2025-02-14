using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeDataSave
{
    public int cost;
    public int upgradeID; // should be the same number as the index in UpgradeManager List
    public bool isUnlocked;
    public int previousUpgradeID;

    // Constructeur pour remplir la classe avec les données de UpgradeData
    public UpgradeDataSave(UpgradeData upgradeData)
    {
        cost = upgradeData.cost;
        upgradeID = upgradeData.upgradeID;
        isUnlocked = upgradeData.isUnlocked;
        previousUpgradeID = upgradeData.previousUpgradeID;
    }
}
