using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FragmentTransfert : MonoBehaviour
{
    private static FragmentTransfert Instance { get; set; }
    public static FragmentTransfert instance => Instance;

    private int fragmentsSaved = 0;
    [SerializeField] private TextMeshProUGUI crystalsSavedTxt;
    private Coroutine keepSaving;
    private bool saveALot;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    private void Start()
    {
        crystalsSavedTxt.text = fragmentsSaved.ToString();
    }

    private void Update()
    {
        if(saveALot)
        {
            AddCrystalSaved(1);
        }
    }

    public void AddCrystalSaved(int nb)
    {
        fragmentsSaved += nb;
        crystalsSavedTxt.text = fragmentsSaved.ToString();
        AudioManager.instance.PlayAudio(transform, "CrystalSave");
    }

    private IEnumerator CheckKeepSaving()
    {
        yield return new WaitForSeconds(1);
        saveALot = true;
    }

    public int GetFragmentsSaved() { return fragmentsSaved; }


}
