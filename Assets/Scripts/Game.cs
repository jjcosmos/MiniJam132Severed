using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    [SerializeField] private Animator _zoneTextAnimator;
    [SerializeField] private TMP_Text _zoneText;
    [SerializeField] private PlayableDirector _endPlayable;
    [SerializeField] private AudioClip _newAreaClip;
    [SerializeField] private AudioSource _src;
    
    private Character _character;
    private NamedZone _currentZone;
    private NamedZone _queuedZone;

    public static Game Singleton;

    private void Start()
    {
        // Greedy singleton
        if (Singleton != null)
        {
            Destroy(Singleton.gameObject);
        }

        Singleton = this;
        
        _character = FindObjectOfType<Character>();
        
    }

    private void Update()
    {
       CheckZoneChange(); 
    }

    public void EnteredZone(NamedZone zone)
    {
        _queuedZone = zone;
    }

    public void NotifyEnd()
    {
        StartCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        _endPlayable.Play();
        var duration = _endPlayable.duration;
        while (duration > 0f)
        {
            duration -= Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }

    private void CheckZoneChange()
    {
        // Only show zone change when player stops moving
        if (_currentZone != _queuedZone && _character.Body.velocity.magnitude < 0.1f)
        {
            _zoneText.text = _queuedZone.ZoneName;
            _zoneTextAnimator.SetTrigger("tShow");
            _currentZone = _queuedZone;
            
            // Play Zone Sfx
            _src.PlayOneShot(_newAreaClip);
            
            // Play new zone music if not null
            if(_currentZone.CustomMusic)
                PersistantMusicMan.SetMusic(_currentZone.CustomMusic);
        }
    }
}
