using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasProgressGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;
    private void Start()
    {
        if (!hasProgressGameObject.TryGetComponent(out hasProgress))
        {
            Debug.LogError($"Couldnt find Component of type IHasProgress for {hasProgressGameObject}");
        }
        
        hasProgress.onProgressChanged += HasProgress_onProgressChanged;
        barImage.fillAmount = 0f;
        
        Hide();
    }

    private void HasProgress_onProgressChanged(object sender, IHasProgress.onProgressChangedEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized == 1f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
