using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class DebugPanel : MonoBehaviour
    {
        private string _stringToEdit = "start";
        private List<DebugCheckpoint> _checkPoints;
        private Rigidbody _playerRb;

        private void Start()
        {
            _checkPoints = FindObjectsOfType<DebugCheckpoint>().ToList();
            _playerRb = FindObjectOfType<Character>().GetComponent<Rigidbody>();
        }

        private void OnGUI()
        {
            return;
            
            _stringToEdit = GUILayout.TextField(_stringToEdit, 32);
            if (GUILayout.Button("GoTo"))
            {
                var found = _checkPoints.FirstOrDefault(c => c.Id == _stringToEdit);
                if (found != null)
                {
                    _playerRb.position = found.transform.position;
                }
            }

            foreach (var debugCheckpoint in _checkPoints)
            {
                GUILayout.Label(debugCheckpoint.Id);
            }
        }
    }
}