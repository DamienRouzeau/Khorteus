using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuInGame : MonoBehaviour
{
    [SerializeField] Player.FirstPersonController player;
    [SerializeField] GameObject pause;
    [SerializeField] Button resume;
    [SerializeField] Button settings;
    [SerializeField] Button menu;
    [SerializeField] Button quit;

    [Header("Animations")]
    [SerializeField] Animator animResume;
    [SerializeField] Animator animSettings;
    [SerializeField] Animator animMenu;
    [SerializeField] Animator animQuit;

    private void Start()
    {
        pause.SetActive(false);
    }

    public void Active()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pause.SetActive(true);
        Time.timeScale = 0;
        player.canRotate = false;
        player.inMenu = true;
        StartCoroutine(FixButtonHover());
    }

    public IEnumerator GamepadActivation()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(resume.gameObject);
    }

    IEnumerator FixButtonHover()
    {
        yield return null;
        resume.interactable = false;
        settings.interactable = false;
        menu.interactable = false;
        quit.interactable = false;
        yield return null;
        resume.interactable = true;
        settings.interactable = true;
        menu.interactable = true;
        quit.interactable = true;
    }

    public void OnResume()
    {
        Time.timeScale = 1;
        player.canRotate = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        player.inMenu = false;
        ResetButtonState(resume);
        pause.SetActive(false);
    }

    public void OnSettings()
    {
        ResetButtonState(settings);
    }

    public void OnMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void OnQuit()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    private void ResetButtonState(Button button)
    {
        button.interactable = false; // Désactive le bouton
        button.interactable = true;  // Le réactive pour forcer le reset
    }

}
