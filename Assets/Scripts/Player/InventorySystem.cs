using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private int fragmentNB;
    [SerializeField]
    private List<GameObject> items = new();
    private int itemInHand;
    [SerializeField]
    private TextMeshProUGUI crystalQuantityTxt;
    UnityEvent newItemInInventory, removeItemInInventory;


    private void Start()
    {
        UpdateTextCrystal();
        newItemInInventory = new UnityEvent();
        removeItemInInventory = new UnityEvent();
    }

    #region Fragment management
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
    #endregion

    public void Scroll(float value)
    {
        if (value > 0)
        {
            items[itemInHand].SetActive(false);
            itemInHand--;
            if (itemInHand < 0) itemInHand = items.Count - 1;
            items[itemInHand].SetActive(true);
        }
        else if (value < 0)
        {
            items[itemInHand].SetActive(false);
            itemInHand++;
            if (itemInHand > items.Count - 1) itemInHand = 0;
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
        newItemInInventory.Invoke();
    }

    public void RemoveItem(GameObject item)
    {
        if(item == items[itemInHand])
        {
            items[itemInHand].SetActive(false);
            Scroll(+1);
        }
        items.Remove(item);
        removeItemInInventory.Invoke();
    }
    #endregion

    #region Getter
    public int GetFragmentQuantity() { return fragmentNB; }
    public GameObject GetItemInHand() { return items[itemInHand]; }
    #endregion

    #region Sub/Unsub events
    public void SubAddItem(UnityAction action)
    {
        newItemInInventory.AddListener(action);
    }

    public void UnsubAddItem(UnityAction action)
    {
        newItemInInventory.RemoveListener(action);
    }

    public void SubRemoveItem(UnityAction action)
    {
        removeItemInInventory.AddListener(action);
    }

    public void UnsubRemoveItem(UnityAction action)
    {
        removeItemInInventory.RemoveListener(action);
    }
    #endregion
}

