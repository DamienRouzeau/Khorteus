using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<UpgradeDataSave> upgradesUnlocked;
    public List<MeleeDataSave> meleeWeaponsUnlocked;
    public int crystalQuantity;
    public int sinnerNB = 0;
    public float globaleVolume = 100;
    public float monsterVolume = 100;
    public float effectVolume = 100;
    public int meleeSelected = 0;


    public GameData(int crystal, int _meleeSelected)
    {
        crystalQuantity = crystal;
        upgradesUnlocked = new List<UpgradeDataSave>();
        meleeWeaponsUnlocked = new List<MeleeDataSave>();
        meleeSelected = _meleeSelected;
    }
}

