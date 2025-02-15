using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private int fragmentNB;
    [SerializeField]
    private List<GameObject> items = new();
    private int itemInHand;
    [SerializeField]
    private TextMeshProUGUI crystalQuantityTxt;


    private void Start()
    {
        UpdateTextCrystal();
    }
    public void AddFragment(int nb)
    {
        fragmentNB += nb;
        UpdateTextCrystal();
    }

    public bool RemoveFragment(int nb)
    {
        if (fragmentNB >= nb)
        {
            fragmentNB -= nb;
            UpdateTextCrystal();
            return true;
        }
        else return false;
    }

    private void UpdateTextCrystal()
    {
        crystalQuantityTxt.text = fragmentNB.ToString();
    }

    public void Scroll(float value)
    {
        if (value > 0)
        {
            items[itemInHand].SetActive(false);
            itemInHand--;
            if (itemInHand < 0) itemInHand = items.Count - 1;
            print(itemInHand);
            items[itemInHand].SetActive(true);
        }
        else if (value < 0)
        {
            items[itemInHand].SetActive(false);
            itemInHand++;
            if (itemInHand > items.Count - 1) itemInHand = 0;
            print(itemInHand);
            items[itemInHand].SetActive(true);
        }
    }

    #region Add / Remove
    public void AddItem(GameObject item)
    {
        items.Add(item);
        items[itemInHand].SetActive(false);
        itemInHand = items.IndexOf(item);
        items[itemInHand].SetActive(true);
    }

    public void RemoveItem(GameObject item)
    {
        if(item == items[itemInHand])
        {
            items[itemInHand].SetActive(false);
            Scroll(+1);
        }
        items.Remove(item);
    }
    #endregion

    public int GetFragmentQuantity() { return fragmentNB; }
    public GameObject GetItemInHand() { return items[itemInHand]; }
}

