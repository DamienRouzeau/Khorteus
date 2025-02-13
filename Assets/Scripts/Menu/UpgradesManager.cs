using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using TMPro;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField] private int crystalsQuantity;
    [SerializeField] private TextMeshProUGUI crystalsQuantityTxt;
    [SerializeField] private List<UpgradeData> upgrades = new();

    private void Start()
    {
        UpdateCrystalQuantity();
        foreach(UpgradeData data in upgrades)
        {
            data.isUnlocked = false; // Rmove this line for avoid reset of upgrades
        }
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
                UpdateCrystalQuantity();
            }
        }
    }
}
