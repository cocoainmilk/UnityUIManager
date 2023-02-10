using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

[DisallowMultipleComponent]
public class ObservableUIShowTrigger : ObservableTriggerSimple, IUIShowHandler
{
    public void OnUIShow()
    {
        if(subject != null) subject.OnNext(Unit.Default);
    }
}

