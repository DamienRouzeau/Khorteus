using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UpgradesManager : MonoBehaviour
{
    private static UpgradesManager Instance { get; set; }
    public static UpgradesManager instance => Instance;

    [SerializeField] private int crystalsQuantity;
    [SerializeField] private TextMeshProUGUI crystalsQuantityTxt;
    [SerializeField] private List<UpgradeData> upgrades = new();
    [SerializeField] private List<ButtonData> upgradeButton;

    [Header("Description")]
    [SerializeField] private TextMeshProUGUI upgradeTitle;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI upgradeCost;
    [SerializeField] private Animator descriptionAnim;

    [Header("Description")]

    private float maxHealth = 100;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

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

        foreach(ButtonData button in upgradeButton)
        {
            if(button.data.isUnlocked)
            {
                button.Unlocked();
            }
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

    public void BuyUpgrade(UpgradeData data, ButtonData button)
    {
        AudioManager.instance.PlayAudio(transform, "Clic", 0.2f);
        if (crystalsQuantity >= data.cost && !data.isUnlocked) // check if player have enought crystals and not already unlock this upgrade
        {
            if (upgrades[data.previousUpgradeID].isUnlocked || data.upgradeID == 0) // avoid the player buy the last upgrade first
            {
                data.isUnlocked = true;
                crystalsQuantity -= data.cost;
                SaveSystem.SetCrystalQuantity(crystalsQuantity);
                UpdateCrystalQuantity();
                SaveSystem.AddUpgrade(data);
                Button background = button.GetComponent<Button>();
                background.image.color = new Color(background.image.color.r, background.image.color.g, background.image.color.b, 1);
                button.Unlocked();
            }
        }
    }

    public void ShowDescription(string title, string description, int cost)
    {
        descriptionAnim.SetTrigger("show");
        upgradeTitle.text = title;
        upgradeDescription.text = description;
        upgradeCost.text = cost.ToString();
    }

    public void HideDescription()
    {
        Debug.Log("Hide");
        descriptionAnim.SetTrigger("hide");
    }
}
