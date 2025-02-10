using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DesktopType : MonoBehaviour
{
    public string turretType;
    [SerializeField]
    private float timeToCraft;
    private float currentCraftingTimer;
    [SerializeField]
    private int fragmentNeeded;
    [SerializeField]
    private Slider buildBar;
    private bool canCraft;

    public bool Craft(InventorySystem inventory)
    {
        if (!canCraft)
        {
            if (inventory.GetFragmentQuantity() >= fragmentNeeded)
            {
                inventory.RemoveFragment(fragmentNeeded);
                canCraft = true;
            }
        }
        else
        {
            currentCraftingTimer += Time.deltaTime;
            buildBar.gameObject.SetActive(true);
            buildBar.value = currentCraftingTimer / timeToCraft;
            if(currentCraftingTimer >= timeToCraft)
            {
                //craft finished
                buildBar.gameObject.SetActive(false);
                buildBar.value = 0;
                return true;
            }
        }
        return false;
    }
}
