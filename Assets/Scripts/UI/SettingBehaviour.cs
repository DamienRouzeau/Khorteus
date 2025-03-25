using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingBehaviour : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject generalSection;
    [SerializeField] private GameObject audioSection;
    [SerializeField] private GameObject graphicSection;
    [SerializeField] private GameObject controlSection;
    [SerializeField] private Button generalButton;

    [Header("Audio")]
    [SerializeField] private float volumeClic;
    [SerializeField] private Slider generalVolume;
    [SerializeField] private Slider effectVolume;
    [SerializeField] private Slider monsterVolume;
    [SerializeField] private TextMeshProUGUI generalVolumeTxt;
    [SerializeField] private TextMeshProUGUI effectVolumeTxt;
    [SerializeField] private TextMeshProUGUI monsterVolumeTxt;

    [Header("Graphic")]
    [SerializeField] private TMP_Dropdown windowMode;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    List<Resolution> resolutions = new List<Resolution>();


    public void ActiveSettingsWindow()
    {
        generalSection.SetActive(true);
        audioSection.SetActive(false);
        graphicSection.SetActive(false);
        controlSection.SetActive(false);
        generalButton.Select();

    }

    private void Start()
    {
        generalSection.SetActive(true);
        audioSection.SetActive(false);
        graphicSection.SetActive(false);
        controlSection.SetActive(false);
        generalButton.Select();

        #region Get all resolutions
        Resolution[] resos = Screen.resolutions;
        foreach(Resolution reso in resos)
        {
            resolutions.Add(reso);
        }
        resolutions.Sort((res1, res2) => (res2.width * res2.height).CompareTo(res1.width * res1.height));
        List<TMP_Dropdown.OptionData> datas = new List<TMP_Dropdown.OptionData>();
        foreach (Resolution reso in resolutions)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = reso.width + " x " + reso.height;
            datas.Add(data);
            print(data.text);
        }
        resolutionDropDown.AddOptions(datas);
        #endregion
    }

    public void OnGeneral()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        generalSection.SetActive(true);
        audioSection.SetActive(false);
        graphicSection.SetActive(false);
        controlSection.SetActive(false);
    }

    public void OnAudio()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        generalSection.SetActive(false);
        audioSection.SetActive(true);
        graphicSection.SetActive(false);
        controlSection.SetActive(false);
    }

    public void OnGraphic()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        generalSection.SetActive(false);
        audioSection.SetActive(false);
        graphicSection.SetActive(true);
        controlSection.SetActive(false);
    }

    public void OnControl()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        generalSection.SetActive(false);
        audioSection.SetActive(false);
        graphicSection.SetActive(false);
        controlSection.SetActive(true);
    }

    public void OnVSync()
    {

    }

    public void OnReverseXAxis()
    {

    }

    public void OnReverseYAxis()
    {

    }

    public void SetWindowMode()
    {
        switch(windowMode.value)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            default:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    public void SetResolution()
    {
        Screen.SetResolution(resolutions[resolutionDropDown.value].width, resolutions[resolutionDropDown.value].height, false);
    }

    public void ChangeGeneralAudio()
    {
        GameData data = SaveSystem.Load();
        data.globaleVolume = generalVolume.value;
        SaveSystem.Save(data);
        generalVolumeTxt.text = RoundVolume(generalVolume.value).ToString();
    }

    public void ChangeMonsterAudio()
    {
        GameData data = SaveSystem.Load();
        data.monsterVolume = monsterVolume.value;
        SaveSystem.Save(data);
        monsterVolumeTxt.text = RoundVolume(monsterVolume.value).ToString();
    }

    public void ChangeEffectAudio()
    {
        GameData data = SaveSystem.Load();
        data.effectVolume = effectVolume.value;
        SaveSystem.Save(data);
        effectVolumeTxt.text = RoundVolume(effectVolume.value).ToString();
    }

    private float RoundVolume(float volume)
    {
        volume *= 10;
        int round = (int) volume;
        volume = round;
        volume /= 10;
        return volume;
    }
}
