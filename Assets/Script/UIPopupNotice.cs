using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UIPopupNotice : MonoBehaviour, IUI
{
    [SerializeField] TextMeshProUGUI textTitle = null;
    [SerializeField] TextMeshProUGUI textContent = null;
    [SerializeField] TextMeshProUGUI textOk = null;
    [SerializeField] Button buttonOk = null;

    public Enum GetId()
    {
        return UISystemType.PopupNotice;
    }

    public IObservable<Unit> Set(string title, string content, string ok = null)
    {
        return Observable.Create<Unit>(observer =>
            {
                if(textTitle != null) textTitle.text = title;
                if(textContent != null) textContent.text = content;
                if(textOk != null) textOk.text = ok ?? "OK";
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
                return Disposable.Empty;
            })
            .SelectMany(_ => this.OnUIHideAsObservable().DelayFrame(1))
            .Merge(buttonOk.OnClickAsObservable().Do(__ => UIManager.Instance.Back(true)))
            .Take(1);
    }
}
