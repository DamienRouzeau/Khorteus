using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]
    private EnemyBehaviour enemy;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Health target = collision.gameObject.GetComponent<Health>();
            enemy.SetTarget(target);
            enemy.DealDamage();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            enemy.SetTarget(null);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health target = other.gameObject.GetComponent<Health>();
            enemy.SetTarget(target);
            enemy.DealDamage();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.SetTarget(null);
        }
    }
}
