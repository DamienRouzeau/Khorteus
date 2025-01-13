using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentBehaviour : MonoBehaviour
{
    private FragmentType type;
    private float timeBeforeEvolution;
    private GameObject visual;
    private FragmentManager manager;

    public void Init(FragmentManager manag)
    {
        manager = manag;
    }

    private void FixedUpdate()
    {
        timeBeforeEvolution -= Time.fixedDeltaTime;
        if(timeBeforeEvolution <= 0)
        {
            Evolve();
        }
    }

    private void Evolve()
    {
        if(type.nextEvolution != null)
        {
            SetEvolution(type.nextEvolution);
        }
        else
        {
            DestroyFragment();
        }
    }

    public void SetEvolution(FragmentType newType)
    {
        if(type == null)
        {
            type = newType;
            type.health = type.maxHealth;
        }
        else
        {
            Destroy(visual);
            float hpTransition = (type.health * 100) / type.maxHealth;
            type = newType;
            type.health = (type.maxHealth * hpTransition) / 100;
        }
        timeBeforeEvolution = Random.Range(type.minTime, type.maxTime);
        var newVisu = Instantiate(type.visual, transform);
        visual = newVisu;
    }

    public void Hit(float dmg)
    {
        type.health -= dmg;
        if(type.health <= 0)
        {
            DestroyFragment();
        }
    }

    public void DestroyFragment()
    {
        manager.DestroyCrystal(this);
    }
}
