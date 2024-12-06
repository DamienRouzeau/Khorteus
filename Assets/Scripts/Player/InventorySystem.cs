using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    private int fragmentNB;

    public void AddFragment(int nb)
    {
        fragmentNB += nb;
    }

    public bool RemoveFragment(int nb)
    {
        if (fragmentNB > nb)
        {
            fragmentNB -= nb;
            return true;
        }
        else return false;
        
    }
}
