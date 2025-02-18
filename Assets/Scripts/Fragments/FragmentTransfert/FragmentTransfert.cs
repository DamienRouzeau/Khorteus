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

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    private void Start()
    {
        crystalsSavedTxt.text = fragmentsSaved.ToString();
    }

    public void AddCrystalSaved(int nb)
    {
        fragmentsSaved += nb;
        crystalsSavedTxt.text = fragmentsSaved.ToString();
        AudioManager.instance.PlayAudio(transform, "CrystalSave");
    }

    public int GetFragmentsSaved() { return fragmentsSaved; }


}
