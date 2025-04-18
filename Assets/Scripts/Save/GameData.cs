using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<UpgradeDataSave> upgradesUnlocked;
    public int crystalQuantity;
    public int sinnerNB = 0;
    public float globaleVolume = 100;
    public float monsterVolume = 100;
    public float effectVolume = 100;


    public GameData(int crystal)
    {
        crystalQuantity = crystal;
        upgradesUnlocked = new List<UpgradeDataSave>();
    }
}

