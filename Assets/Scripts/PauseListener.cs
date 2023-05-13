using UnityEngine;

public class PauseListener : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;
    
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            var isPaused = _pauseMenu.activeInHierarchy;

            if (isPaused)
            {
                Time.timeScale = 1;
                _pauseMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 0f;
                _pauseMenu.SetActive(true);
            }
        }
    }
}
