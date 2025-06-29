using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private BoxCollider col;
    private bool canHit = true;

    [Header("Data")]
    [SerializeField] private float damage;
    [SerializeField] private float minningDamage;
    [SerializeField] private float weight;

    public void Hit()
    {
        if (!canHit) return;
        col.enabled = true;
        canHit = false;
        anim.SetTrigger("Hit");
    }

    public void HitEnd()
    {
        canHit = true;
        col.enabled = false;
    }

    #region Getter
    public bool GetCanHit() { return canHit; }
    public float GetDamage() { return damage; }
    public float GetMinningDamage() { return minningDamage; }
    public float GetWeight() { return weight; }
    #endregion
}
