using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBehaviour : MonoBehaviour, Health
{
    UnityEvent dieEvent;
    [Header("MOVE TARGETS")]
    private NavMeshAgent agent;
    public Transform player;
    public Transform generator;
    public Transform generatorAttackPos;
    private List<Transform> hidingSpots = new List<Transform>();
    private Transform target;
    private bool destinationSet;

    [Header("HEALTH")]
    public float currentHealth;
    [SerializeField]
    private float maxHealth;

    [Header("ATTACK")]
    public float damage;
    [SerializeField]
    private float hitCouldown;
    private float timeSinceLastAttack;
    [SerializeField]
    private Animator anim;
    public Health targetDMG;
    [SerializeField]
    private Collider attackCollider;
    [SerializeField]
    private float range;

    [Header("MISCELANEOUS")]
    [SerializeField]
    private monsterStats state;
    [SerializeField]
    private Rigidbody rb;


    #region START
    void Start()
    {
        if (dieEvent == null) dieEvent = new UnityEvent();
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastAttack = hitCouldown;
        currentHealth = maxHealth;
    }
    public void SetPlayerRef(Transform _player)
    {
        player = _player;
    }

    public void SetGeneratorRef(Transform _generator, Transform _attackPos)
    {
        generator = _generator;
        generatorAttackPos = _attackPos;
    }
    #endregion

    void Update()
    {
        switch (state) // Set target destination
        {
            case monsterStats.chase:
                target = player;
                destinationSet = false;
                break;

            case monsterStats.hiding:
                Transform _closest = hidingSpots[0];
                foreach (Transform pos in hidingSpots)
                {
                    if (Vector3.Distance(pos.position, player.position) < Vector3.Distance(_closest.position, player.position))
                        _closest = pos;
                }
                target = _closest;
                destinationSet = false;
                break;

            case monsterStats.destructor:
                target = generatorAttackPos;
                destinationSet = false;
                break;
        }

        if (target != null && !destinationSet)
        {
            agent.SetDestination(target.position);
            destinationSet = true;

        }
        timeSinceLastAttack += Time.deltaTime;

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void SetTarget(Health _target)
    {
        targetDMG = _target;
        StartCoroutine(LoseTarget());
    }

    private IEnumerator LoseTarget()
    {
        yield return new WaitForSeconds(1);

        targetDMG = null;
    }

    #region ATTACK
    public void Attack()
    {
        if (timeSinceLastAttack >= hitCouldown)
        {
            anim.SetTrigger("Attack");
            timeSinceLastAttack = 0;
        }

    }

    public void DealDamage()
    {
        if (targetDMG != null)
        {
            targetDMG.TakeDamage(damage);
        }
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        player.GetComponentInChildren<Player.FirstPersonController>().BulletTouch();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    #endregion

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Attack();
        }
        else if (collision.collider.CompareTag("generator") && target == generatorAttackPos)
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
        else if (other.CompareTag("generator") && target == generatorAttackPos)
        {
            Attack();
        }
    }

    #region STATE
    public void ChangeState(monsterStats newState)
    {
        state = newState;
    }

    public monsterStats GetState()
    {
        return state;
    }
    #endregion


    public void SubDie(UnityAction subscriber)
    {
        dieEvent.AddListener(subscriber);
    }
    public void UnsubDie(UnityAction subscriber)
    {
        dieEvent.RemoveListener(subscriber);
    }


    public void Die()
    {
        Audio deathAudio = AudioManager.instance.PlayAudio(transform, "MonsterDie", 3, UnityEngine.Random.Range(0.95f, 1.05f));
        deathAudio.transform.parent = null;
        dieEvent.Invoke();
        if (WaveManager.instance == null)
        {

        }
        else
        { WaveManager.instance.EnemyDied(this.gameObject); }
        Destroy(this.gameObject);
    }

    #region SETTER
    public void SetHidingSpots(List<Transform> _hidingSpots)
    {
        hidingSpots = _hidingSpots;
    }
    #endregion

    #region GETTER
    public float GetHealth()
    {
        return currentHealth;
    }
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    #endregion
}
