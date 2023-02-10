using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class UITest : MonoBehaviour, IUI
{
    [SerializeField] Button buttonPopup = null;
    [SerializeField] Button buttonBack = null;

    public Enum GetId()
    {
        return UIUserType.Test;
    }

    private void Awake()
    {
        // 이 UI가 열릴 때 호출됨
        this.OnUIShowAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("UITest: OnUIShow");
            })
            .AddTo(this);

        // 이 UI가 닫힐 때 호출됨
        this.OnUIHideAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("UITest: OnUIHide");
            })
            .AddTo(this);

        // buttonPopup를 누르면 팝업이 열림
        buttonPopup.OnClickAsObservable()
            .Subscribe(_ =>
            {
                UIManager.Instance.ShowPopupYesOrNo("Popup", "OK?")
                    .Subscribe(ok =>
                    {
                        Debug.Log($"UITest: Popup {ok}");
                    })
                    .AddTo(this);
            })
            .AddTo(this);

        // buttonBack 누르면 이 UI를 닫음
        buttonBack.OnClickAsObservable()
            .Subscribe(_ =>
            {
                UIManager.Instance.Back();
            })
            .AddTo(this);
    }
}