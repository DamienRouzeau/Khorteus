using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField] private int crystalsQuantity;
    [SerializeField] private TextMeshProUGUI crystalsQuantityTxt;
    [SerializeField] private List<UpgradeData> upgrades = new();

    private void Start()
    {
        GameData gameData = SaveSystem.Load();
        foreach (UpgradeData data in upgrades)
        {
            data.isUnlocked = false;
        }
        if (gameData == null)
        {
            Debug.LogError("Can't load save data");
        }
        else
        {
            foreach (UpgradeDataSave upgradeSaved in gameData.upgradesUnlocked)
            {
                UpgradeData upgrade = null;
                foreach(UpgradeData upgdata in upgrades)
                {
                    if (upgdata.upgradeID == upgradeSaved.upgradeID) upgrade = upgdata;
                }
                upgrade.isUnlocked = upgradeSaved.isUnlocked;
            }
            crystalsQuantity = gameData.crystalQuantity;
        }
        UpdateCrystalQuantity();
    }

    public void AddACrystal()
    {
        crystalsQuantity++;
        UpdateCrystalQuantity();
        SaveSystem.SetCrystalQuantity(crystalsQuantity);
    }

    private void UpdateCrystalQuantity()
    {
        crystalsQuantityTxt.text = crystalsQuantity.ToString();
    }

    public void BuyUpgrade(UpgradeData data)
    {
        if(crystalsQuantity >= data.cost && !data.isUnlocked) // check if player have enought crystals and not already unlock this upgrade
        {
            print("enought crystals");
            if (upgrades[data.previousUpgradeID].isUnlocked || data.upgradeID == 0) // avoid the player buy the last upgrade first
            {
                print("Buy succeed");
                data.isUnlocked = true;
                crystalsQuantity -= data.cost;
                SaveSystem.SetCrystalQuantity(crystalsQuantity);
                UpdateCrystalQuantity();
                SaveSystem.AddUpgrade(data);
            }
        }
    }
}
