using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator _blackoutAnim;
    [SerializeField] private CanvasGroup _mainGroup;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private Button _firstSelection;
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioSource _src;
    [SerializeField] private AudioClip _startClip;

    private void Start()
    {
        _settingsMenu.GetComponent<PauseMenu>().Init();
        PersistantMusicMan.SetMusic(_mainMenuMusic);
    }

    public void LoadMainScene()
    {
        _src.PlayOneShot(_startClip);
        _blackoutAnim.SetTrigger("tToBlack");
        StartCoroutine(LoadSceneRoutine());
    }

    public void ShowSettings()
    {
        _settingsMenu.SetActive(true);
        _mainGroup.interactable = false;
    }

    public void HideSettings()
    {
        _settingsMenu.SetActive(false);
        _mainGroup.interactable = true;
        _firstSelection.Select();
    }

    private IEnumerator LoadSceneRoutine()
    {
        _mainGroup.interactable = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Gameplay");
    }
}
