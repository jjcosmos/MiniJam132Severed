using UnityEngine;

public class PauseListener : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenu;
    
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            var isPaused = _pauseMenu.gameObject.activeInHierarchy;

            if (isPaused)
            {
                Time.timeScale = 1;
                _pauseMenu.gameObject.SetActive(false);
            }
            else
            {
                _pauseMenu.Init();
                Time.timeScale = 0f;
                _pauseMenu.gameObject.SetActive(true);
            }
        }
    }
}
