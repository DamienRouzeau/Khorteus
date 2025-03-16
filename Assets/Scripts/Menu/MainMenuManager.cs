using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject mainMenuSection;
    [SerializeField] private GameObject upgradeSection;
    [SerializeField] private GameObject creditsSection;
    [SerializeField] private GameObject settingsSection;
    [SerializeField] private Button[] buttons;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera playCam;
    [SerializeField] private CinemachineVirtualCamera mainMenuCam;
    [SerializeField] private CinemachineVirtualCamera upgradeCam;
    [SerializeField] private CinemachineVirtualCamera settingsCam;
    [SerializeField] private CinemachineVirtualCamera creditsCam;

    [Header("Animations")]
    [SerializeField] private Animator tutoAnim;

    [Header("Audio")]
    [SerializeField] private float volumeClic;
    private bool isBeggining = true;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Audio audio = AudioManager.instance.PlayAudio(transform, "MenuAmbiant", 0.07f);
        OnMainMenu();

        foreach (Button btn in buttons)
        {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = btn.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((data) => PlayHoverSound());

            trigger.triggers.Add(entry);
        }
    }

    void PlayHoverSound()
    {
        AudioManager.instance.PlayAudio(transform, "Hovered");
    }


    public void OnMainMenu()
    {
        if (!isBeggining)
        {
            AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
            AudioManager.instance.PlayAudio(transform, "Whoosh");
        }
        isBeggining = false;
        mainMenuSection.SetActive(true);
        upgradeSection.SetActive(false);
        creditsSection.SetActive(false);
        settingsSection.SetActive(false);

        mainMenuCam.Priority = 1;
        upgradeCam.Priority = 0;
        settingsCam.Priority = 0;
        creditsCam.Priority = 0;
    }

    public void OnPlay()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        AudioManager.instance.PlayAudio(transform, "Whoosh");
        mainMenuCam.Priority = 0;
        upgradeCam.Priority = 0;
        settingsCam.Priority = 0;
        creditsCam.Priority = 0;
        playCam.Priority = 1;
        mainMenuSection.SetActive(false);
        StartCoroutine(StartGame(1));
    }

    public void OnPlayTuto()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        AudioManager.instance.PlayAudio(transform, "Whoosh");
        mainMenuCam.Priority = 0;
        upgradeCam.Priority = 0;
        settingsCam.Priority = 0;
        creditsCam.Priority = 0;
        playCam.Priority = 1;
        mainMenuSection.SetActive(false);
        StartCoroutine(StartGame(2));
    }

    private IEnumerator StartGame(int sceneID)
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneID);
    }


    public void OnUpdate()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        AudioManager.instance.PlayAudio(transform, "Whoosh");
        mainMenuCam.Priority = 0;
        upgradeCam.Priority = 1;
        mainMenuSection.SetActive(false);
        upgradeSection.SetActive(true);
    }

    public void OnSettings()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        AudioManager.instance.PlayAudio(transform, "Whoosh");
        mainMenuCam.Priority = 0;
        settingsCam.Priority = 1;
        mainMenuSection.SetActive(false);
        settingsSection.SetActive(true);
    }

    public void OnCredits()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        AudioManager.instance.PlayAudio(transform, "Whoosh");
        mainMenuCam.Priority = 0;
        creditsCam.Priority = 1;
        mainMenuSection.SetActive(false);
        creditsSection.SetActive(true);
    }

    public void OnQuit()
    {
        AudioManager.instance.PlayAudio(transform, "Clic", volumeClic);
        Application.Quit();
    }

}
