using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class EndZone : MonoBehaviour
{ 
    [SerializeField] private CinemachineVirtualCamera _endCam;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character player))
        {
            player.SetCanMove(false);
            player.Body.velocity *= 0.5f;
            _endCam.Priority = 100;
            
            Game.Singleton.NotifyEnd();
        }
    }
}
