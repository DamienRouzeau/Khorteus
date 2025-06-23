using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cost;
    public UpgradeData data;
    [SerializeField] private Animator anim;
    private bool shouldHide;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] Color unlockedColor;

    void Start()
    {
        cost.text = data.cost.ToString();
        if(data.isUnlocked)
        {
            anim.SetBool("isUnlocked", true);
        }
    }



    public void OnClick()
    {
        UpgradesManager.instance.BuyUpgrade(data, this);
    }


    public void Unlocked()
    {
        //anim.SetBool("isUnlocked", true);
        level.color = unlockedColor;
    }

    public void Hover()
    {
        UpgradesManager.instance.ShowDescription(data.title, data.description, data.cost);
        shouldHide = true;
    }

    public void Normal()
    {
        if (shouldHide)
        {
            UpgradesManager.instance.HideDescription();
            shouldHide = false;
        }
    }

}
