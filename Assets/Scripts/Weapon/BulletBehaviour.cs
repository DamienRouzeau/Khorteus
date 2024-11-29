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


    public void SetPool(PoolBullet poolRef)
    {
        pool = poolRef;
    }

    private void Update()
    {
        //transform.position -= transform.right * speed * Time.deltaTime;
    }

    public void Launch()
    {
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    public void SetPosition(Vector3 position)
    {
        rb.MovePosition(position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject);
        if (collision.collider.CompareTag("monster"))
        {
            print("BBB");
            Health health = collision.gameObject.GetComponent<Health>();
            Hit(health);
        }
        else Hit();
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject);
        if (other.CompareTag("monster"))
        {
            print("BBB");
            Health health = other.gameObject.GetComponent<Health>();
            Hit(health);
        }
        else Hit();
    }


    private void Hit(Health health)
    {
        rb.velocity = Vector3.zero;
        health.TakeDamage(damage);
        pool.AddBulletToPool(this);
        gameObject.SetActive(false);
    }

    private void Hit()
    {
        rb.velocity = Vector3.zero;
        pool.AddBulletToPool(this);
        gameObject.SetActive(false);
    }
}
