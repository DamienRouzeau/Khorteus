using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlerBehaviour : Enemy
{
    private bool haveScream;
    [SerializeField] float dieDelay;
    [SerializeField] private float alarmDelayAfterScream;

    protected override void Start()
    {
        base.Start();
        AudioManager.instance.PlayAudio(transform, "HowlerSpawn");
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
        agent.SetDestination(transform.position);
        anim.SetTrigger("Scream");
        AudioManager.instance.PlayAudio(transform, "HowlerScream");
        StartCoroutine(ReactionTime());
        StartCoroutine(DieWithDelay());
    }

    private IEnumerator ReactionTime()
    {
        yield return new WaitForSeconds(alarmDelayAfterScream);
        WaveManager.instance.RestartCurrentWave();
    }

    private IEnumerator DieWithDelay()
    {
        yield return new WaitForSeconds(dieDelay);
        Die();
    }
}
