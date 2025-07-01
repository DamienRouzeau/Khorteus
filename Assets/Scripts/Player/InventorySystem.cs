using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private int fragmentNB;
    [SerializeField]
    private List<GameObject> items = new();
    private int itemInHand;
    private List<int> nbItem = new();
    [SerializeField]
    private TextMeshProUGUI crystalQuantityTxt;
    UnityEvent newItemInInventory, removeItemInInventory;
    [SerializeField] private Image itemInHandImg, nextItemImg, previousItemImg;
    [SerializeField] private List<Sprite> icons;
    [SerializeField] private TextMeshProUGUI textQTT;
    [SerializeField] private Player.FirstPersonController player;


    private void Start()
    {
        nbItem.Add(1); //Add for Gun
        nbItem.Add(1); //Add for Melee

        UpdateTextCrystal();
        newItemInInventory = new UnityEvent();
        removeItemInInventory = new UnityEvent();
        StartCoroutine(UpdateImageAfterOneFrame());
    }

    private IEnumerator UpdateImageAfterOneFrame()
    {
        yield return null;
        UpdateImages();
        UpdateQTT();
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
            Debug.Log("Item in hand = " + itemInHand);
            items[itemInHand].SetActive(true);
        }
        else if (value < 0)
        {
            items[itemInHand].SetActive(false);
            itemInHand++;
            if (itemInHand > items.Count - 1) itemInHand = 0;
            Debug.Log("Item in hand = " + itemInHand);
            items[itemInHand].SetActive(true);
        }
        UpdateImages();
        UpdateQTT();

    }

    #region Add / Remove
    public void AddItem(GameObject item)
    {
        if (items.Contains(item))
        {
            nbItem[items.IndexOf(item)]++;
        }
        else
        {
            items.Add(item);
            nbItem.Add(1);
            items[itemInHand].SetActive(false);
            itemInHand = items.IndexOf(item);
            items[itemInHand].SetActive(true);
            newItemInInventory.Invoke();
        }
        UpdateImages();
        UpdateQTT();
    }

    public void AddItemAndKeepItemInHand(GameObject item)
    {
        if (items.Contains(item))
        {
            nbItem[items.IndexOf(item)]++;
        }
        else
        {
            items.Add(item);
            nbItem.Add(1);
            newItemInInventory.Invoke();
        }
        UpdateImages();
        UpdateQTT();
    }

    public void RemoveItem(GameObject item)
    {
        if (item == items[itemInHand] && nbItem[itemInHand] == 1)
        {
            nbItem.RemoveAt(itemInHand);
            items[itemInHand].SetActive(false);
            Scroll(+1);
            items.Remove(item);
        }
        else if (nbItem[items.IndexOf(item)] == 1)
        {
            int index = items.IndexOf(item);
            nbItem.RemoveAt(index);
            items.Remove(item);
        }
        else
        {
            nbItem[items.IndexOf(item)]--;
        }

        removeItemInInventory.Invoke();
        UpdateImages();
        UpdateQTT();
    }

    #endregion

    private void UpdateImages()
    {
        // Affiche l'item actuellement en main
        itemInHandImg.sprite = icons[GetSprite(items[itemInHand].name)];

        // Cas où le joueur n’a qu’un seul item
        if (items.Count < 2)
        {
            previousItemImg.gameObject.SetActive(false);
            nextItemImg.gameObject.SetActive(false);
            return;
        }

        // Affiche les images des slots précédent et suivant
        previousItemImg.gameObject.SetActive(true);
        nextItemImg.gameObject.SetActive(true);

        // Cas où le joueur a exactement deux items : previous = next
        if (items.Count == 2)
        {
            int otherIndex = (itemInHand + 1) % 2;
            Sprite otherSprite = icons[GetSprite(items[otherIndex].name)];

            previousItemImg.sprite = otherSprite;
            nextItemImg.sprite = otherSprite;

            Debug.Log("2 items: previous & next = " + items[otherIndex].name);
        }
        // Cas général : 3 items ou plus
        else
        {
            int nextIndex = (itemInHand + 1) % items.Count;
            int previousIndex = (itemInHand - 1 + items.Count) % items.Count;

            nextItemImg.sprite = icons[GetSprite(items[nextIndex].name)];
            previousItemImg.sprite = icons[GetSprite(items[previousIndex].name)];

            Debug.Log("Current: " + items[itemInHand].name);
            Debug.Log("Next: " + items[nextIndex].name);
            Debug.Log("Previous: " + items[previousIndex].name);
        }
        UpdateQTT();
    }

    public void UpdateQTT()
    {
        switch (items[itemInHand].name)
        {
            case "Gun":
                textQTT.text = player.GetBulletLeft().ToString() + "/" + player.GetMaxBullet().ToString();
                break;


            case "Sniper":
                textQTT.text = nbItem[itemInHand].ToString();
                break;


            case "MachineGun":
                textQTT.text = nbItem[itemInHand].ToString();
                break;

            default:
                textQTT.text = nbItem[itemInHand].ToString();
                break;
        }
    }

    /*private void UpdateImages()
    {

        itemInHandImg.sprite = icons[GetSprite(items[itemInHand].name)];

        if(items.Count == 2)
        {
            previousItemImg.gameObject.SetActive(true);
            nextItemImg.gameObject.SetActive(true);
            if (itemInHand + 1 < items.Count)
            {
                int nextIndex = itemInHand + 1;
                nextItemImg.sprite = icons[GetSprite(items[nextIndex].name)];
                previousItemImg.sprite = icons[GetSprite(items[nextIndex].name)];
                Debug.Log("From scenario 1 : next name = " + items[nextIndex].name);
            }
            else
            {
                nextItemImg.sprite = icons[GetSprite(items[0].name)];
                previousItemImg.sprite = icons[GetSprite(items[0].name)];
                Debug.Log("From scenario 2 : next name = " + items[0].name);
            }
        }
        else if(items.Count > 2)
        {
            int nextIndex = (itemInHand + 1) % items.Count;
            int previousIndex = (itemInHand - 1 + items.Count) % items.Count;

            itemInHandImg.sprite = icons[GetSprite(items[itemInHand].name)];
            nextItemImg.sprite = icons[GetSprite(items[nextIndex].name)];
            previousItemImg.sprite = icons[GetSprite(items[previousIndex].name)];

            nextItemImg.gameObject.SetActive(true);
            previousItemImg.gameObject.SetActive(true);

            Debug.Log("InHand: " + items[itemInHand].name);
            Debug.Log("Next: " + items[nextIndex].name);
            Debug.Log("Previous: " + items[previousIndex].name);
        }
        else
        {
            previousItemImg.gameObject.SetActive(false);
            nextItemImg.gameObject.SetActive(false);
        }
    }*/

    private int GetSprite(string name)
    {
        switch (name)
        {
            case "Gun":
                Debug.Log("From function : return 0 because name was " + name);
                return 0;

            case "Sniper":
                Debug.Log("From function : return 1 because name was " + name);
                return 1;

            case "MachineGun":
                Debug.Log("From function : return 2 because name was " + name);
                return 2;

            default:
                return 3;
        }
    }

    #region Getter
    public int GetFragmentQuantity() { return fragmentNB; }
    public GameObject GetItemInHand() { return items[itemInHand]; }

    public int GetNbItem() { return items.Count; }
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

