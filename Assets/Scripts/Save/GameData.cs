using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<UpgradeDataSave> upgradesUnlocked;
    public int crystalQuantity;

    public GameData(int crystal)
    {
        crystalQuantity = crystal;
        upgradesUnlocked = new List<UpgradeDataSave>();
    }
}

