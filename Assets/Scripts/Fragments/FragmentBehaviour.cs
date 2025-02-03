using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentBehaviour : MonoBehaviour
{
    private FragmentType type;
    private float timeBeforeEvolution;
    private GameObject visual;
    private FragmentManager manager;
    [SerializeField]
    private Slider healthBar;
    private float health;

    public void Init(FragmentManager manag)
    {
        manager = manag;
    }

    private void Start()
    {
        health = type.health;
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

    public void Hit(float dmg)
    {
        health -= dmg;
        Debug.Log(health);
        if(health <= 0)
        {
            DestroyFragment();
        }
        else if(!healthBar.gameObject.activeInHierarchy)
        {
            healthBar.gameObject.SetActive(true);
        }
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        healthBar.value = health / type.maxHealth;
    }

    public int GetQuantity() { return type.quantity; }

    public float GetHealth() { return health; }

    public void DestroyFragment()
    {
        manager.DestroyCrystal(this);
    }
}
