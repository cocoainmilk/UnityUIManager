using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;

public static class ObservableTriggerExtensions
{
    static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if(component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    public static IObservable<Unit> OnUIShowAsObservable(this GameObject gameObject)
    {
        if(gameObject == null) return Observable.Empty<Unit>();
        return GetOrAddComponent<ObservableUIShowTrigger>(gameObject).AsObservable();
    }

    public static IObservable<Unit> OnUIShowAsObservable(this Component component)
    {
        return OnUIShowAsObservable(component?.gameObject);
    }

    public static IObservable<Unit> OnUIHideAsObservable(this GameObject gameObject)
    {
        if(gameObject == null) return Observable.Empty<Unit>();
        return GetOrAddComponent<ObservableUIHideTrigger>(gameObject).AsObservable();
    }

    public static IObservable<Unit> OnUIHideAsObservable(this Component component)
    {
        return OnUIHideAsObservable(component?.gameObject);
    }

    public static IObservable<Unit> OnUIFocusAsObservable(this GameObject gameObject)
    {
        if(gameObject == null) return Observable.Empty<Unit>();
        return GetOrAddComponent<ObservableUIFocusTrigger>(gameObject).AsObservable();
    }

    public static IObservable<Unit> OnUIFocusAsObservable(this Component component)
    {
        return OnUIFocusAsObservable(component?.gameObject);
    }

    public static IObservable<Unit> OnUIBlurAsObservable(this GameObject gameObject)
    {
        if(gameObject == null) return Observable.Empty<Unit>();
        return GetOrAddComponent<ObservableUIBlurTrigger>(gameObject).AsObservable();
    }

    public static IObservable<Unit> OnUIBlurAsObservable(this Component component)
    {
        return OnUIBlurAsObservable(component?.gameObject);
    }
}
