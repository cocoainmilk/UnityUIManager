using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUIShowHandler : IEventSystemHandler
{
    void OnUIShow();
}

public interface IUIHideHandler : IEventSystemHandler
{
    void OnUIHide();
}

public interface IUIFocusHandler : IEventSystemHandler
{
    void OnUIFocus();
}

public interface IUIBlurHandler : IEventSystemHandler
{
    void OnUIBlur();
}

public static class ExecuteEventsCustom
{
    public static ExecuteEvents.EventFunction<IUIShowHandler> ShowHandler =>
        (handler, eventData) => handler.OnUIShow();
    public static ExecuteEvents.EventFunction<IUIHideHandler> HideHandler =>
        (handler, eventData) => handler.OnUIHide();
    public static ExecuteEvents.EventFunction<IUIFocusHandler> FocusHandler =>
        (handler, eventData) => handler.OnUIFocus();
    public static ExecuteEvents.EventFunction<IUIBlurHandler> BlurHandler =>
        (handler, eventData) => handler.OnUIBlur();

    public static void ExecuteHierarchyAll<T>(GameObject root, BaseEventData eventData, ExecuteEvents.EventFunction<T> callbackFunction) where T : IEventSystemHandler
    {
        root.transform.ForEachRecursively(transform =>
            Array.ForEach(transform.gameObject.GetComponents<T>(), handler => callbackFunction(handler, eventData)));
    }
}

