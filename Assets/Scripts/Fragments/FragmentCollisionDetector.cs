using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentCollisionDetector : MonoBehaviour
{
    private FragmentBehaviour behaviour;

    private void Start()
    {
        behaviour = GetComponentInParent<FragmentBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee"))
        {
            behaviour.GetPlayerRef(other.GetComponentInParent<InventorySystem>());
            behaviour.Hit(other.GetComponent<MeleeBehaviour>().GetMinningDamage());
        }
    }
}
