using UnityEngine;

public class PersistantMusicMan : MonoBehaviour
{

    [SerializeField] private AudioSource _src;
    [SerializeField] private float _fadeSpeed = 1f;
    [SerializeField] private float _maxVolume = 0.5f;
    
    private AudioClip _current;
    private AudioClip _queued;

    public static PersistantMusicMan Singleton;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void SetMusic(AudioClip clip)
    {
        Singleton._queued = clip;

        if (!Singleton._src.isPlaying)
        {
            Singleton._src.volume = Singleton._maxVolume;
            Singleton._current = Singleton._queued;
            Singleton._src.clip = Singleton._current;
            Singleton._src.Play();
        }
    }
    
    private void Update()
    {
        if (_current != _queued && _src.volume > 0f)
        {
            _src.volume = Mathf.Clamp(_src.volume - Time.deltaTime * _fadeSpeed, 0f, _maxVolume);

            if (_src.volume <= 0f)
            {
                _current = _queued;
                _src.clip = _current;
                _src.Play();
            }
        }
        else if (_current == _queued && _src.volume < _maxVolume)
        {
            _src.volume = Mathf.Clamp(_src.volume + Time.deltaTime * _fadeSpeed, 0f, _maxVolume);
        }
        
    }
}
