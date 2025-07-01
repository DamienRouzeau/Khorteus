using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MeleeButton : MonoBehaviour
{
    public MeleeData data;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private GameObject buyButton;
    [SerializeField] private TextMeshProUGUI selectTxt;
    [SerializeField] private Image background;
    [SerializeField] private Sprite selectedBG, unselectedBG;


    public void OnBuy()
    {
        UpgradesManager.instance.BuyMeleeWeapon(data, this);
    }

    public void Unlocked()
    {
        buyButton.SetActive(false);
        selectButton.SetActive(true);
    }

    public void Selected()
    {
        selectTxt.text = "Selected";
        background.sprite = selectedBG;
        UpgradesManager.instance.SelectMeleeWeapon(data.upgradeID, this);
    }

    public void Unselected()
    {
        selectTxt.text = "Select";
        background.sprite = unselectedBG;
    }
}
