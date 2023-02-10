using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ObservableTriggerSimple : ObservableTriggerBase
{
    protected Subject<Unit> subject;

    public IObservable<Unit> AsObservable()
    {
        return subject ?? (subject = new Subject<Unit>());
    }

    protected override void RaiseOnCompletedOnDestroy()
    {
        subject?.OnCompleted();
    }
}
