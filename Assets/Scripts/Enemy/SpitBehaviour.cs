using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitBehaviour : MonoBehaviour
{
    Transform target;
    float damage;
    [SerializeField] private float speed;
    [SerializeField] float timeAlive;
    [SerializeField] float range;

    public void Spit(Transform _target, float dmg)
    {
        target = _target;
        damage = dmg;
        transform.localPosition = Vector3.zero;
        transform.parent = null;
    }

    private void Update()
    {
        timeAlive -= Time.deltaTime;
        if (timeAlive < 0) Destroy(gameObject);
        if(target != null)
        {
            Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position, transform.up);
            transform.rotation = lookRotation;

            transform.position += transform.forward * Time.deltaTime * speed;

        }
        if(Vector3.Distance(transform.position, target.position) < range)
        {
            HitPlayer();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            HitPlayer();
        }
    }

    private void HitPlayer()
    {
        Player.FirstPersonController player = target.GetComponent<Player.FirstPersonController>();
        player.GetSpitted();
        player.TakeDamage(damage);
        Destroy(this.gameObject);
    }
}
