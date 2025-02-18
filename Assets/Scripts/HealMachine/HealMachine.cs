using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealMachineStats
{
    Empty,
    Crafting,
    Done
}

public class HealMachine : MonoBehaviour
{
    private HealMachineStats stat;
    [SerializeField] private float craftTime;
    [SerializeField] private Animator anim;
    [SerializeField] private float healGiven;
    [SerializeField] private float healDecremental;
    private Audio craftAudio;

    private void Start()
    {
        stat = HealMachineStats.Empty;
    }

    public void Crafting()
    {
        anim.SetTrigger("Crafting");
        stat = HealMachineStats.Crafting;
        craftAudio = AudioManager.instance.PlayAudio(transform, "CraftHeal");
        StartCoroutine(CraftingTimer());
    }

    private IEnumerator CraftingTimer()
    {
        yield return new WaitForSeconds(craftTime);
        craftAudio.Stop();
        AudioManager.instance.PlayAudio(transform, "HealMachineOpen");
        anim.SetTrigger("Done");
        stat = HealMachineStats.Done;
    }

    public float TakeHeal()
    {
        float healReturned = healGiven;
        healGiven *= healDecremental;
        stat = HealMachineStats.Empty;
        anim.SetTrigger("Taken");
        return healReturned;
    }

   

    public HealMachineStats GetStat() { return stat; }
}
