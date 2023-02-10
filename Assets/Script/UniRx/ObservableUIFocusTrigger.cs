using System;
using UnityEngine;
using UniRx;

[DisallowMultipleComponent]
public class ObservableUIFocusTrigger : ObservableTriggerSimple, IUIFocusHandler
{
    public void OnUIFocus()
    {
        if(subject != null) subject.OnNext(Unit.Default);
    }
}
