using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamedZone : MonoBehaviour
{
    public string ZoneName;
    public AudioClip CustomMusic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Character player))
        {
            Game.Singleton.EnteredZone(this);
        }
    }
}
