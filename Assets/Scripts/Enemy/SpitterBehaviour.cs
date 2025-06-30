using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class SpitterBehaviour : Enemy
{
    UnityEvent dieEvent;
    [SerializeField] private LayerMask plafondLayer, obstacleMask;
    [SerializeField] private GameObject visual;
    [SerializeField] private SpitBehaviour spit;
    [SerializeField] private GameObject spitLauncher;

    #region START
    protected override void Start()
    {
        if (dieEvent == null)
            dieEvent = new UnityEvent();
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastAttack = hitCouldown;
        currentHealth = maxHealth;
        Rigidbody rb = GetComponent<Rigidbody>();
        Debug.Log("Rigidbody trouvé : " + rb + " | IsKinematic = " + rb.isKinematic);
        GetComponent<Rigidbody>().isKinematic = false;
    }
    public override void SetPlayerRef(Transform _player)
    {
        player = _player;
    }

    public override void SetGeneratorRef(Transform _generator, Transform _attackPos)
    {
        generator = _generator;
        generatorAttackPos = _attackPos;
    }
    #endregion

    protected override void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (priorityTarget != null)
        {
            target = priorityTarget;
            destinationSet = false;
        }

        if (target != null && !destinationSet)
        {
            agent.SetDestination(target.position);
            destinationSet = true;
        }


        Vector3 groundPosition = agent.nextPosition;
        anim.SetFloat("Speed", agent.velocity.magnitude);


        RaycastHit hit; // Ray cast to the ceiling to add the monster on it
        if (Physics.Raycast(groundPosition, Vector3.up, out hit, 50, plafondLayer))
        {
            if (hit.normal.y < 0)
            {
                transform.position = hit.point;
                transform.up = hit.normal;
            }

            bool isSeen = false;


            if (IsBeingWatched(player))
            {
                isSeen = true;
            }

            agent.isStopped = isSeen; // Stop or keep walking

            Vector3 directionToTarget = (agent.steeringTarget - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget, transform.up);
            visual.transform.rotation = Quaternion.Slerp(visual.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if(Vector3.Distance(transform.position, priorityTarget.position) <= range)
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        // Reversed gravity
        rb.velocity = -transform.up * 5f;
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hit in hits)
        {
            if(hit.CompareTag("Bullet"))
            {
                hit.GetComponent<BulletBehaviour>().Hit(this);
            }
            else if (hit.CompareTag("Melee"))
            {
                TakeDamage(hit.GetComponent<MeleeBehaviour>().GetDamage());
            }
        }
    }

    bool IsBeingWatched(Transform player)
    {
        Vector3 directionToEnemy = (transform.position - player.position).normalized;
        float dotProduct = Vector3.Dot(player.forward, directionToEnemy);

        if (dotProduct > 0.5f)
        {
            if (!Physics.Linecast(player.position, transform.position, obstacleMask))
            {
                if (Vector3.Distance(player.position, transform.position) < 12) return true;
            }
        }
        return false;
    }


    public override void SetTarget(Health _target)
    {
        targetDMG = _target;
        StartCoroutine(LoseTarget());
    }

    protected override IEnumerator LoseTarget()
    {
        yield return new WaitForSeconds(1);

        targetDMG = null;
    }

    #region ATTACK
    public override void Attack()
    {
        if (timeSinceLastAttack >= hitCouldown)
        {
            anim.SetTrigger("Attack");
            timeSinceLastAttack = 0;
            StartCoroutine(Spit());
        }
    }

    private IEnumerator Spit()
    {
        yield return new WaitForSeconds(0.25f);
        AudioManager.instance.PlayAudio(transform, "Spit", 1, UnityEngine.Random.Range(0.94f, 1.2f));
        var _spit = Instantiate(spit, spitLauncher.transform);
        _spit.Spit(player, 5);
    }

    public override void DealDamage()
    {
        if (targetDMG != null)
        {
            targetDMG.TakeDamage(damage);
        }
    }
    public override void TakeDamage(float damage)
    {
        currentHealth -= damage;
        player.GetComponentInChildren<Player.FirstPersonController>().BulletTouch();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    #endregion


    #region STATE
    public override void ChangeState(monsterStats newState)
    {
        state = newState;
    }

    public override monsterStats GetState()
    {
        return state;
    }
    #endregion


    public override void SubDie(UnityAction subscriber)
    {
        if (dieEvent == null)
            dieEvent = new UnityEvent();
        dieEvent.AddListener(subscriber);
    }
    public override void UnsubDie(UnityAction subscriber)
    {
        dieEvent.RemoveListener(subscriber);
    }


    public override void Die()
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
    public override void SetHidingSpots(List<Transform> _hidingSpots)
    {
        hidingSpots = _hidingSpots;
    }
    #endregion

    #region GETTER
    public override float GetHealth()
    {
        return currentHealth;
    }
    public override float GetMaxHealth()
    {
        return maxHealth;
    }

    #endregion
}
