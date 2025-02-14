using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject mainMenuSection;
    [SerializeField] private GameObject upgradeSection;
    [SerializeField] private GameObject creditsSection;
    [SerializeField] private GameObject settingsSection;

    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera playCam;
    [SerializeField] private CinemachineVirtualCamera mainMenuCam;
    [SerializeField] private CinemachineVirtualCamera upgradeCam;
    [SerializeField] private CinemachineVirtualCamera settingsCam;
    [SerializeField] private CinemachineVirtualCamera creditsCam;

    private void Start()
    {
        OnMainMenu();
    }

    public void OnMainMenu()
    {
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
        mainMenuCam.Priority = 0;
        upgradeCam.Priority = 0;
        settingsCam.Priority = 0;
        creditsCam.Priority = 0;
        playCam.Priority = 1;
        mainMenuSection.SetActive(false);
        StartCoroutine(StartGame());
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(0);
    }


    public void OnUpdate()
    {
        mainMenuCam.Priority = 0;
        upgradeCam.Priority = 1;
        mainMenuSection.SetActive(false);
        upgradeSection.SetActive(true);
    }

    public void OnSettings()
    {
        mainMenuCam.Priority = 0;
        settingsCam.Priority = 1;
        mainMenuSection.SetActive(false);
        settingsSection.SetActive(true);
    }

    public void OnCredits()
    {
        mainMenuCam.Priority = 0;
        creditsCam.Priority = 1;
        mainMenuSection.SetActive(false);
        creditsSection.SetActive(true);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
