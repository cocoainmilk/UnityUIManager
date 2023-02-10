using System;
using UnityEngine;
using UniRx;

[DisallowMultipleComponent]
public class ObservableUIHideTrigger : ObservableTriggerSimple, IUIHideHandler
{
    public void OnUIHide()
    {
        if(subject != null) subject.OnNext(Unit.Default);
    }
}
