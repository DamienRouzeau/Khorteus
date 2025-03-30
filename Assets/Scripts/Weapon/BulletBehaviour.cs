using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float damage;
    private PoolBullet pool;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float maxTimeAlive = 1;
    private float timeAlive;


    public void SetPool(PoolBullet poolRef)
    {
        pool = poolRef;
    }

    private void Update()
    {
        //transform.position -= transform.right * speed * Time.deltaTime;
        timeAlive -= Time.deltaTime;
        if (timeAlive <= 0) Hit();
    }

    public void Launch()
    {
        timeAlive = maxTimeAlive; 
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    public void SetPosition(Vector3 position)
    {
        rb.MovePosition(position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("monster"))
        {
            print("Touched C");
            Health health = collision.gameObject.GetComponent<Health>();
            Hit(health);
        }
        else
        {
            Debug.Log(collision.collider.name + " hitted with tag : " + collision.collider.tag);
            Hit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("monster"))
        {
            print("Touched T");
            Health health = other.gameObject.GetComponent<Health>();
            Hit(health);
        }
    }


    private void Hit(Health health)
    {
        rb.velocity = Vector3.zero;
        health.TakeDamage(damage);
        pool.AddBulletToPool(this);
    }

    private void Hit()
    {
        rb.velocity = Vector3.zero;
        pool.AddBulletToPool(this);
    }

    public void AddDamage(float dmg)
    {
        damage += dmg;
    }
}
