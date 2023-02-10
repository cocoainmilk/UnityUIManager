using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UIMain : MonoBehaviour, IUI
{
    [SerializeField] Button button = null;

    public Enum GetId()
    {
        return UIUserType.Main;
    }

    private void Awake()
    {
        // 버튼을 누르면 Test UI를 띄운다.
        button.OnClickAsObservable()
            .Subscribe(_ =>
            {
                UIManager.Instance.Push(UIUserType.Test, true);
            })
            .AddTo(this);
    }
}
