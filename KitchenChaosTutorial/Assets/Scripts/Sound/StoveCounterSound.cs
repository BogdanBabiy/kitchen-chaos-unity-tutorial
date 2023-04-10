using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.onStateChanged += StoveCounterOnonStateChanged;
    }

    private void StoveCounterOnonStateChanged(object sender, StoveCounter.onStateChangedEventArg e)
    {
        bool playSound =
            e.state == StoveCounter.State.Frying
            ||
            e.state == StoveCounter.State.Fried;

        if (playSound) audioSource.Play();
        else audioSource.Pause();
    }
}
