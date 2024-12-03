using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour, Health
{
    private NavMeshAgent agent;
    public Transform player;

    [Header("HEALTH")]
    private float currentHealth;
    [SerializeField]
    private float maxHealth;

    [Header("Attack")]
    [SerializeField]
    private float damage;
    [SerializeField]
    private float hitCouldown;
    private float timeSinceLastAttack;
    [SerializeField]
    private Animator anim;
    public Health target;
    [SerializeField]
    private Collider attackCollider;
    [SerializeField]
    private float range;

    [Header("MISCELANEOUS")]
    [SerializeField]
    private Rigidbody rb;




    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastAttack = hitCouldown;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //float xdist = Mathf.Abs(transform.position.x - player.transform.position.x);
        //float ydist = Mathf.Abs(transform.position.y - player.transform.position.y);
        //float zdist = Mathf.Abs(transform.position.z - player.transform.position.z);
        //bool isclose = false;
        //if (xdist <= range && ydist <= range && zdist <= range) isclose = true;
        //else isclose = false;

        if (player != null)
        {
            agent.SetDestination(player.position);
        }
        //else if (isclose)
        //{
        //    agent.bra;
        //}
        timeSinceLastAttack += Time.deltaTime;
        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void SetPlayerRef(Transform _player)
    {
        player = _player;
    }

    public void SetTarget(Health _target)
    {
        target = _target;
        StartCoroutine(LoseTarget());
    }

    private IEnumerator LoseTarget()
    {
        yield return new WaitForSeconds(1);
        target = null;
    }

    public void Attack()
    {
        if(timeSinceLastAttack >= hitCouldown)
        {
            anim.SetTrigger("Attack");
            timeSinceLastAttack = 0;
        }

    }

    public void DealDamage()
    {
        if(target != null)
        {
            target.TakeDamage(damage);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            Attack();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        WaveManager.instance.EnemyDied(this.gameObject);
        Destroy(this.gameObject);
    }
}
