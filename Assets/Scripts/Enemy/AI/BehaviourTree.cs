using Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    EnemyBehaviour owner;
    FirstPersonController player;

    [Header("Changers")]
    [SerializeField]
    private float playerDistanceMultiplicator;

    [Header("Hide")]
    private float currentHealth;
    [SerializeField]
    private float hideChoiceMultiplicator;
    private float hideInfluence;

    [Header("Chase Player")]
    private float playerHealth;
    [SerializeField]
    private float attackPlayerChoiceMultiplicator;
    private float attackPlayerInfluence;

    [Header("Destruct Generator")]
    private float generatorEnergy;
    [SerializeField]
    private float attackGeneratorChoiceMultiplicator;
    private float attackGeneratorInfluence;

    private void Start()
    {
        owner = GetComponent<EnemyBehaviour>();
        player = owner.player.GetComponent<FirstPersonController>();
    }

    private void FixedUpdate()
    {
        SetAllStats();
        CalculNextState();
        monsterStats nextState = GetNextState();
        if(owner.GetState() != nextState) owner.ChangeState(nextState);
    }

    private monsterStats GetNextState()
    {
        // IF HIDE INFLUENCE IS HIGHER THAN OTHERS INFLUENCES
        if (hideInfluence > attackGeneratorInfluence && hideInfluence > attackPlayerInfluence)
            return monsterStats.hiding;

        // IF ATTACK GENERATOR IS HIGHER THAN OTHERS INFLUENCES
        else if (attackGeneratorInfluence > attackPlayerInfluence)
            return monsterStats.destructor;

        // IF ATTACK PLAYER IS HIGHER OR EQUAL THAN OTHERS INFLUENCES
        else
            return monsterStats.chase;
    }

    private void CalculNextState()
    {
        float _distPlayer = Vector3.Distance(owner.transform.position, player.transform.position);
        _distPlayer *= playerDistanceMultiplicator;

        // hide factors : player near, health low
        hideInfluence = (owner.GetMaxHealth() / currentHealth) * hideChoiceMultiplicator * _distPlayer;

        // chase factors : player near, player health low, health high
        attackPlayerInfluence = (player.GetMaxHealth() / playerHealth / _distPlayer) * attackPlayerChoiceMultiplicator;

        // destruct factors : player far, generator health low 
        attackGeneratorInfluence = (100 / generatorEnergy) * attackGeneratorChoiceMultiplicator * _distPlayer;
    }

    private void SetAllStats()
    {
        currentHealth = owner.GetHealth();
        playerHealth = player.GetHealth();
        generatorEnergy = owner.generator.GetComponent<GeneratorBehaviour>().GetEnergy();
    }

    private void ResetInfluence()
    {
        hideInfluence = 0;
        attackPlayerInfluence = 0;
        attackGeneratorInfluence = 0;
    }
}
