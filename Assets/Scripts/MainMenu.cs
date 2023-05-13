using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator _blackoutAnim;
    [SerializeField] private CanvasGroup _mainGroup;
    [SerializeField] private GameObject _settingsMenu;
    
    public void LoadMainScene()
    {
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
    }

    private IEnumerator LoadSceneRoutine()
    {
        _mainGroup.interactable = false;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Gameplay");
    }
}
