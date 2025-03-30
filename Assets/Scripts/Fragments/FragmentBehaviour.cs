using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentBehaviour : MonoBehaviour
{
    public FragmentType type;
    private float timeBeforeEvolution;
    private GameObject visual;
    private FragmentManager manager;
    [SerializeField]
    //private Slider healthBar;
    private float health;
    private int qttleft;
    [SerializeField] private float healthStep = -1;
    bool canEvolve = true;

    public void Init(FragmentManager manag)
    {
        manager = manag;
    }

    private void Start()
    {
        health = type.health;
        if(healthStep == -1) healthStep = health / type.quantity;
        qttleft = type.quantity;
    }

    private void FixedUpdate()
    {
        timeBeforeEvolution -= Time.fixedDeltaTime;
        if (timeBeforeEvolution <= 0 && canEvolve)
        {
            Evolve();
        }
    }

    private void Evolve()
    {
        if (type.nextEvolution != null)
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
        if (type == null)
        {
            type = newType;
            health = type.maxHealth;
        }
        else
        {
            Destroy(visual);
            float hpTransition = (health * 100) / type.maxHealth;
            type = newType;
            health = (type.maxHealth * hpTransition) / 100;
        }
        timeBeforeEvolution = Random.Range(type.minTime, type.maxTime);
        var newVisu = Instantiate(type.visual, transform);
        visual = newVisu;
    }

    public int Hit(float dmg)
    {
        canEvolve = false;
        health -= dmg;
        if (health <= 0)
        {
            DestroyFragment();
            return qttleft;
        }
        print("step : " + (type.maxHealth - (healthStep * qttleft)));
        if (health < type.maxHealth - (healthStep * (type.quantity - qttleft + 1)) && qttleft > 0)
        {
            qttleft--;
            return 1;
        }
        return 0;
    }

    public float GetHealthValue()
    {
        float value = health / type.maxHealth;
        return value;
    }

    public int GetQuantity() { return qttleft; }

    public float GetHealth() { return health; }

    public void DestroyFragment()
    {
        manager.DestroyCrystal(this);
    }
}
