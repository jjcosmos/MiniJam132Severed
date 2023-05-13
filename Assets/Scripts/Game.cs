using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Animator _zoneTextAnimator;
    [SerializeField] private TMP_Text _zoneText;
    
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

    public void EnteredEndZone(int id)
    {
        _character.SetCanMove(false);
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
            
            // Play new zone music if not null
        }
    }
}