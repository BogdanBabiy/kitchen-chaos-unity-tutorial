using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start()
    {
        stoveCounter.onStateChanged += StoveCounter_onStateChanged;
    }

    private void StoveCounter_onStateChanged(object sender, StoveCounter.onStateChangedEventArg e)
    {
        bool showVisual = 
            e.state == StoveCounter.State.Frying 
            || 
            e.state == StoveCounter.State.Fried;
        
        stoveOnGameObject.SetActive(showVisual);
        particlesGameObject.SetActive(showVisual);
    }
}
