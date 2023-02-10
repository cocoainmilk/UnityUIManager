using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UIWaiting : MonoBehaviour, IUI
{
    [SerializeField] GameObject spinner = null;

    IDisposable disposable;

    public Enum GetId()
    {
        return UISystemType.Waiting;
    }

    private void OnDestroy()
    {
        disposable?.Dispose();
    }

    public void Set(bool noSpinner = false)
    {
        disposable?.Dispose();

        spinner.SetActive(false);

        if(!noSpinner)
        {
            disposable = Observable.Timer(TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ => spinner.SetActive(true));
        }
    }
}

