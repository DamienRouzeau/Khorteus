using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;




public abstract class Enemy : MonoBehaviour, Health
{
    UnityEvent dieEvent;
    [Header("MOVE TARGETS")]
    protected NavMeshAgent agent;
    public Transform player;
    public Transform generator;
    public Transform priorityTarget;
    public Transform generatorAttackPos;
    protected List<Transform> hidingSpots = new List<Transform>();
    protected Transform target;
    protected bool destinationSet;

    [Header("HEALTH")]
    public float currentHealth;
    [SerializeField]
    protected float maxHealth;

    [Header("ATTACK")]
    public float damage;
    [SerializeField]
    protected float hitCouldown;
    protected float timeSinceLastAttack;
    [SerializeField]
    protected Animator anim;
    public Health targetDMG;
    [SerializeField]
    protected Collider attackCollider;
    [SerializeField]
    protected float range;

    [Header("MISCELANEOUS")]
    public monsterTypes type;
    [SerializeField]
    protected monsterStats state;
    [SerializeField]
    protected Rigidbody rb;


    #region START
    protected virtual void Start()
    {
        if (dieEvent == null)
            dieEvent = new UnityEvent();
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastAttack = hitCouldown;
        currentHealth = maxHealth;
    }
    public virtual void SetPlayerRef(Transform _player)
    {
        player = _player;
    }

    public virtual void SetGeneratorRef(Transform _generator, Transform _attackPos)
    {
        generator = _generator;
        generatorAttackPos = _attackPos;
    }
    #endregion

    protected virtual void  Update()
    {
        if (priorityTarget != null)
        {
            target = priorityTarget;
            destinationSet = false;
        }
        else
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
        }

        if (target != null && !destinationSet)
        {
            agent.SetDestination(target.position);
            destinationSet = true;

        }
        timeSinceLastAttack += Time.deltaTime;

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    public virtual void SetTarget(Health _target)
    {
        targetDMG = _target;
        StartCoroutine(LoseTarget());
    }

    protected virtual IEnumerator LoseTarget()
    {
        yield return new WaitForSeconds(1);

        targetDMG = null;
    }

    #region ATTACK
    public virtual void Attack()
    {
        if (timeSinceLastAttack >= hitCouldown)
        {
            anim.SetTrigger("Attack");
            timeSinceLastAttack = 0;
        }

    }

    public virtual void DealDamage()
    {
        if (targetDMG != null)
        {
            targetDMG.TakeDamage(damage);
        }
    }
    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        player.GetComponentInChildren<Player.FirstPersonController>().BulletTouch();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    #endregion

    protected virtual void OnCollisionStay(Collision collision)
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

    protected virtual void OnTriggerStay(Collider other)
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
    public virtual void ChangeState(monsterStats newState)
    {
        state = newState;
    }

    public virtual monsterStats GetState()
    {
        return state;
    }
    #endregion


    public virtual void SubDie(UnityAction subscriber)
    {
        if (dieEvent == null)
            dieEvent = new UnityEvent();
        dieEvent.AddListener(subscriber);
    }
    public virtual void UnsubDie(UnityAction subscriber)
    {
        dieEvent.RemoveListener(subscriber);
    }


    public virtual void Die()
    {
        Audio deathAudio = AudioManager.instance.PlayAudio(transform, "MonsterDie", 3, UnityEngine.Random.Range(0.95f, 1.05f));
        deathAudio.transform.parent = null;
        if (dieEvent == null)
            dieEvent = new UnityEvent();
        dieEvent.Invoke();
        if (WaveManager.instance == null)
        {

        }
        else
        { WaveManager.instance.EnemyDied(this.gameObject); }
        Destroy(this.gameObject);
    }

    #region SETTER
    public virtual void SetHidingSpots(List<Transform> _hidingSpots)
    {
        hidingSpots = _hidingSpots;
    }
    #endregion

    #region GETTER
    public virtual float GetHealth()
    {
        return currentHealth;
    }
    public virtual float GetMaxHealth()
    {
        return maxHealth;
    }

    #endregion
}
