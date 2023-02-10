using System;
using UnityEngine;
using UniRx;

[DisallowMultipleComponent]
public class ObservableUIBlurTrigger : ObservableTriggerSimple, IUIBlurHandler
{
    public void OnUIBlur()
    {
        if(subject != null) subject.OnNext(Unit.Default);
    }
}
