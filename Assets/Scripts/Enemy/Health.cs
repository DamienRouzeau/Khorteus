using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth<=0)
        {
            Die();
        }
    }

    public void Die()
    {

    }


}