using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlerBehaviour : Enemy
{
    private bool haveScream;
    [SerializeField] float dieDelay;

    protected override void Start()
    {
        base.Start();
        //Play incomming sound
    }

    protected override void Update()
    {
        if (priorityTarget != null)
        {
            target = priorityTarget;
            destinationSet = false;
        }

        if (target != null && !destinationSet)
        {
            agent.SetDestination(target.position);
            destinationSet = true;
            float targetDist = Vector3.Distance(target.position, transform.position);
            if(targetDist<= range && !haveScream)
            {
                Scream();
            }
        }

        timeSinceLastAttack += Time.deltaTime;

        anim.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void Scream()
    {
        haveScream = true;
        WaveManager.instance.RestartCurrentWave();
        agent.SetDestination(transform.position);
        anim.SetTrigger("Scream");
        //Play scream sound
        StartCoroutine(DieWithDelay());
    }

    private IEnumerator DieWithDelay()
    {
        yield return new WaitForSeconds(dieDelay);
        Die();
    }
}
