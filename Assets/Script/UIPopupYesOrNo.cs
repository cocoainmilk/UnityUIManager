using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class UIPopupYesOrNo : MonoBehaviour, IUI
{
    [SerializeField] TextMeshProUGUI textTitle = null;
    [SerializeField] TextMeshProUGUI textContent = null;
    [SerializeField] TextMeshProUGUI textYes = null;
    [SerializeField] TextMeshProUGUI textNo = null;
    [SerializeField] Button buttonYes = null;
    [SerializeField] Button buttonNo = null;

    public Enum GetId()
    {
        return UISystemType.PopupYesOrNo;
    }

    public IObservable<bool> Set(string title, string content, string yes = null, string no = null)
    {
        return Observable.Create<Unit>(observer =>
            {
                if(textTitle != null)
                {
                    textTitle.text = title;
                }

                textContent.text = content;
                if(textYes != null)
                {
                    textYes.text = yes ?? "YES";
                }
                if(textNo != null)
                {
                    textNo.text = no ?? "NO";
                }
                observer.OnNext(Unit.Default);
                observer.OnCompleted();
                return Disposable.Empty;
            })
            .SelectMany(_ => this.OnUIHideAsObservable().Select(__ => false).DelayFrame(1))
            .Merge(buttonYes.OnClickAsObservable().Do(__ => UIManager.Instance.Back(true)).Select(__ => true))
            .Merge(buttonNo.OnClickAsObservable().Do(__ => UIManager.Instance.Back(true)).Select(__ => false))
            .Take(1);
    }
}
