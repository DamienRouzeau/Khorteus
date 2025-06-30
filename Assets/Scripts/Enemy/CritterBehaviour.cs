using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CritterBehaviour : Enemy
{
    UnityEvent dieEvent;

    #region START
    protected override void Start()
    {
        if (dieEvent == null)
            dieEvent = new UnityEvent();
        agent = GetComponent<NavMeshAgent>();
        timeSinceLastAttack = hitCouldown;
        currentHealth = maxHealth;
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
        }

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
        Debug.Log("[Damage] Get " + damage + " damages");
        currentHealth -= damage;
        player.GetComponentInChildren<Player.FirstPersonController>().BulletTouch();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    #endregion

    protected override void OnCollisionStay(Collision collision)
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

    protected override void OnTriggerStay(Collider other)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Melee"))
        {
            TakeDamage(other.GetComponent<MeleeBehaviour>().GetDamage());
        }
    }

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
