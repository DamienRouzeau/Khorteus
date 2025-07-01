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
    private MeleeButton meleeSelected;
    [SerializeField] private List<MeleeButton> meleeButton;
    [SerializeField] private List<MeleeData> meleeData;

    [Header("Menus")]
    [SerializeField] private GameObject skillsMenu;
    [SerializeField] private GameObject meleeWeaponsMenu;

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
                foreach (UpgradeData upgdata in upgrades)
                {
                    if (upgdata.upgradeID == upgradeSaved.upgradeID) upgrade = upgdata;
                }
                upgrade.isUnlocked = upgradeSaved.isUnlocked;
            }
            foreach (MeleeDataSave meleeSaved in gameData.meleeWeaponsUnlocked)
            {
                MeleeData melee = null;
                foreach (MeleeData mlData in meleeData)
                {
                    if (mlData.upgradeID == meleeSaved.upgradeID) melee = mlData;
                }
                melee.isUnlocked = meleeSaved.isUnlocked;
            }
            crystalsQuantity = gameData.crystalQuantity;
        }

        foreach (ButtonData button in upgradeButton)
        {
            if (button.data.isUnlocked)
            {
                button.Unlocked();
            }
        }

        foreach (MeleeButton button in meleeButton)
        {
            if (button.data.isUnlocked)
            {
                button.Unlocked();
            }
        }
        Debug.Log(gameData.meleeSelected);
        meleeButton[gameData.meleeSelected].Selected();
        meleeSelected = meleeButton[gameData.meleeSelected];

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

    public void BuyMeleeWeapon(MeleeData data, MeleeButton button)
    {
        AudioManager.instance.PlayAudio(transform, "Clic", 0.2f);
        Debug.Log("Try to buy");
        if (crystalsQuantity >= data.cost && !data.isUnlocked) // check if player have enought crystals and not already unlock this upgrade
        {
            Debug.Log("1");
            data.isUnlocked = true;
            Debug.Log("2");
            crystalsQuantity -= data.cost;
            Debug.Log("3");
            SaveSystem.SetCrystalQuantity(crystalsQuantity);
            Debug.Log("4");
            UpdateCrystalQuantity();
            Debug.Log("5");
            SaveSystem.AddMelee(data);
            Debug.Log("6");
            button.Unlocked();
            Debug.Log("7");
        }
    }

    public void SelectMeleeWeapon(int id, MeleeButton melee)
    {
        if (melee == meleeSelected) return;
        if(meleeSelected != null) meleeSelected.Unselected();
        meleeSelected = melee;
        SaveSystem.SelectMelee(id);
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

    public void SkillsMenu()
    {
        meleeWeaponsMenu.SetActive(false);
        skillsMenu.SetActive(true);
    }

    public void MeleeWeaponsMenu()
    {
        skillsMenu.SetActive(false);
        meleeWeaponsMenu.SetActive(true);
    }
}
