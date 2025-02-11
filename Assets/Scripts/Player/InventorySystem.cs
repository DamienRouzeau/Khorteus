using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private int fragmentNB;
    [SerializeField]
    private List<GameObject> items = new();
    private int itemInHand;

    public void AddFragment(int nb)
    {
        fragmentNB += nb;
    }

    public bool RemoveFragment(int nb)
    {
        if (fragmentNB >= nb)
        {
            fragmentNB -= nb;
            return true;
        }
        else return false;
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

