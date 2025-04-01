using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitOnGroundBehaviour : MonoBehaviour, Health
{
    [SerializeField] float startHealth;
    [SerializeField] float damage;
    [SerializeField] float hitCouldown;
    [SerializeField] Collider collider;
    float timeSinceLastHit;
    private float currenHealth;
    private Player.FirstPersonController player;
    private Quaternion initialRotation;


    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Init(Player.FirstPersonController _player)
    {
        collider.enabled = true;
        currenHealth = startHealth;
        player = _player;
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        timeSinceLastHit += Time.deltaTime;
        if(timeSinceLastHit>= hitCouldown)
        {
            player.TakeDamage(damage);
            timeSinceLastHit = 0;
        }
    }

    void LateUpdate()
    {
        transform.rotation = initialRotation;
    }

    public void Die()
    {
        collider.enabled = false;
        player.UnSpitted();
        timeSinceLastHit = 0;
        gameObject.SetActive(false);
    }

    public void TakeDamage(float _damage)
    {
        currenHealth -= _damage;
        player.BulletTouch();
        if (currenHealth <= 0)
        {
            Die();
        }
    }
}
