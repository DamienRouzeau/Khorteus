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
        if (collision.collider.CompareTag("generator"))
        {
            GeneratorBehaviour.instance.RemoveEnergie(enemy.damage/20);
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
        if (other.CompareTag("generator"))
        {
            GeneratorBehaviour.instance.RemoveEnergie(enemy.damage/20);
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
