using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class UIManager : SingletonMonoBehaviourOnlyLevel<UIManager>
{
    public class UIInfo
    {
        public Enum Id { get; private set; }
        public GameObject GameObject { get; private set; }
        public Canvas Canvas { get; private set; }

        public UIInfo(Enum id, GameObject go)
        {
            Id = id;
            GameObject = go;
            Canvas = go.GetComponent<Canvas>();
        }
    }

    const float DefaultPlaneDistance = 100;

    [SerializeField] UIManagerConfig config = null;
    [SerializeField] new Camera camera = null;

    Transform systemUIRoot = null;
    Transform userUIRoot = null;

    Dictionary<Enum, UIInfo> instances = new Dictionary<Enum, UIInfo>();

    UIInfo topUI;
    List<(UIInfo UI, List<bool> Activated)> stack = new List<(UIInfo, List<bool>)>();

    int waitingUICount = 0;

    // 초기화. 모든 UI 프리팹을 인스턴스화하고 시작 UI를 액티브한다.
    public void Init()
    {
        if(camera == null)
        {
            camera = Camera.main;
        }
        var startingUIId = CreateUI();
        ShowStartingUI(startingUIId);
    }

    // Init()과 동일하지만 모든 UI를 한 프레임 렌더링하여 최초로 UI를 열 때 발생하는 랙을 없앤다.
    public IObservable<Unit> InitWarm()
    {
        return Observable.ReturnUnit()
            .Select(_ =>
            {
                if(camera == null)
                {
                    camera = Camera.main;
                }

                var startingUIId = CreateUI();
                foreach(var info in instances.Values)
                {
                    info.GameObject.SetActive(true);
                }

                return startingUIId;
            })
            .DelayFrame(1)
            .Do(startingUIId =>
            {
                ShowStartingUI(startingUIId);
            })
            .AsUnitObservable();
    }

    private void Update()
    {
        CheckAndroidBackButton();
    }

    // 모든 UI 프리팹을 인스턴스화 한다.
    Enum CreateUI()
    {
        systemUIRoot = new GameObject("System").transform;
        systemUIRoot.SetParent(transform);
        userUIRoot = new GameObject("User").transform;
        userUIRoot.SetParent(transform);

        foreach(var prefab in config.SystemUIPrefab)
        {
            if(prefab)
            {
                var go = Instantiate(prefab, systemUIRoot);
                var id = go.GetComponent<IUI>().GetId();
                var info = new UIInfo(id, go);
                info.Canvas.worldCamera = camera;
                instances.Add(id, info);

                // Waiting UI는 입력 차단 역할을 하기 때문에 모든 UI 보다 위에 있도록 보장
                if(id.Equals(UISystemType.Waiting))
                {
                    info.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    info.Canvas.sortingOrder = 100;
                }
            }
        }

        Enum startingUIId = null;
        foreach(var prefab in config.UserUIPrefab)
        {
            if(prefab != null)
            {
                var go = Instantiate(prefab, userUIRoot);
                var id = go.GetComponent<IUI>().GetId();
                var info = new UIInfo(id, go);
                info.Canvas.worldCamera = camera;
                instances.Add(id, info);

                // 가장 앞에 세팅한 UI가 시작 UI가 됨
                if(config.UseStartingUserUI && startingUIId == null)
                {
                    startingUIId = id;
                }
            }
        }

        return startingUIId;
    }

    void ShowStartingUI(Enum startingUIId)
    {
        foreach(var info in instances.Values)
        {
            info.GameObject.SetActive(false);
        }

        if(startingUIId != null)
        {
            Navigate(startingUIId);
        }
    }

    // 모든 UI를 감추고 지정한 UI를 액티브한다.
    public GameObject Navigate(Enum id)
    {
        return Show(id, false);
    }

    // 현재 UI 위에 새로운 UI를 보여준다. hide가 true일 경우 현재 UI를 감춘다.
    // 이후 Back()을 호출하면 이전 UI를 보여준다.
    public GameObject Push(Enum id, bool hide)
    {
        return Show(id, true, hide);
    }

    GameObject Show(Enum id, bool isStack, bool hide = false)
    {
        var newUI = FindUI(id);
        if(newUI == null)
        {
            throw new Exception($"UIManager.Show: invalid id. {id}");
        }

        if(stack.Exists(stack => stack.UI == newUI))
        {
            throw new Exception($"UIManager.Show: alreay stacked. {id}");
        }

        newUI.Canvas.planeDistance = DefaultPlaneDistance;
        newUI.Canvas.sortingOrder = 0;

        // 현재 보이고 있는 UI가 있는 경우
        if(topUI != null)
        {
            // 새로운 UI를 현재 UI 위에 보여줄 때
            if(isStack)
            {
                if(topUI.GameObject == newUI.GameObject)
                {
                    ExecuteEventsCustom.ExecuteHierarchyAll(topUI.GameObject, null, ExecuteEventsCustom.HideHandler);
                    ExecuteEventsCustom.ExecuteHierarchyAll(topUI.GameObject, null, ExecuteEventsCustom.ShowHandler);
                    return topUI.GameObject;
                }

                // 현재 보이고 있는 UI의 OnUIBlur 이벤트를 호출
                ExecuteEventsCustom.ExecuteHierarchyAll(topUI.GameObject, null, ExecuteEventsCustom.BlurHandler);

                // 이전 UI에 대한 active 상태 처리
                List<bool> activated = null;
                if(hide)
                {
                    activated = stack.Select(stack => stack.UI.GameObject.activeSelf).ToList();
                    activated.Add(true);
                }
                stack.Add((topUI, activated));
                if(hide)
                {
                    stack.ForEach(stack => stack.UI.GameObject.SetActive(false));
                }

                // 새로운 UI를 가장 위로 보이도록
                float planeDistance = stack.Count;
                newUI.Canvas.planeDistance = DefaultPlaneDistance - planeDistance;
                newUI.Canvas.sortingOrder = stack.Count;
            }
            // 열려있는 모든 UI를 닫고 새로운 UI를 보여줄 때
            else
            {
                // 열려있는 모든 UI의 OnUIHide 이벤트를 호출하고 inactive한다.
                ExecuteEventsCustom.ExecuteHierarchyAll(topUI.GameObject, null, ExecuteEventsCustom.HideHandler);
                topUI.GameObject.SetActive(false);
                foreach(var stackUI in stack)
                {
                    ExecuteEventsCustom.ExecuteHierarchyAll(stackUI.UI.GameObject, null, ExecuteEventsCustom.HideHandler);
                    stackUI.UI.GameObject.SetActive(false);
                }
                stack.Clear();
            }
        }

        // 새로운 UI를 액티브하고 OnUIShow 이벤트를 호출한다.
        newUI.GameObject.SetActive(true);
        ExecuteEventsCustom.ExecuteHierarchyAll(newUI.GameObject, null, ExecuteEventsCustom.ShowHandler);

        topUI = newUI;

        return newUI.GameObject;
    }

    // 현재 UI를 닫고 이전 상태로 돌아간다.
    public void Back(bool noHideNotify = false)
    {
        if(stack.Count > 0)
        {
            var oldTopUI = topUI;
            var newTopUI = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if(newTopUI.Activated != null)
            {
                for(int i = 0; i < newTopUI.Activated.Count - 1 && i < stack.Count; ++i)
                {
                    stack[i].UI.GameObject.SetActive(newTopUI.Activated[i]);
                }
            }

            if(!noHideNotify)
            {
                ExecuteEventsCustom.ExecuteHierarchyAll(oldTopUI.GameObject, null, ExecuteEventsCustom.HideHandler);
            }

            oldTopUI.GameObject.SetActive(false);

            newTopUI.UI.GameObject.SetActive(true);
            ExecuteEventsCustom.ExecuteHierarchyAll(newTopUI.UI.GameObject, null, ExecuteEventsCustom.FocusHandler);

            topUI = newTopUI.UI;
        }
    }

    public void ShowWaiting(bool noSpinner = false)
    {
        var ui = FindUI(UISystemType.Waiting);
        ui.GameObject.SetActive(true);
        ui.GameObject.GetComponent<UIWaiting>()
            .Set(noSpinner);
        ++waitingUICount;
    }

    public void BackWaiting(bool all = false)
    {
        if(all)
        {
            waitingUICount = 0;
        }
        else
        {
            --waitingUICount;
        }

        if(waitingUICount < 0)
        {
            Debug.LogError($"UIManager.BackWaiting : waitingUICount < 0");
            waitingUICount = 0;
        }

        if(waitingUICount <= 0)
        {
            var ui = FindUI(UISystemType.Waiting);
            ui.GameObject.SetActive(false);
        }
    }

    public IObservable<bool> ShowPopupYesOrNo(string title, string content, string yes = null, string no = null)
    {
        return Observable.Create<GameObject>(observer =>
        {
            GameObject ui = Push(UISystemType.PopupYesOrNo, false);
            if(ui != null)
            {
                observer.OnNext(ui);
                observer.OnCompleted();
            }
            else
            {
                observer.OnError(new NullReferenceException());
            }

            return Disposable.Empty;
        })
        .ContinueWith(ui => ui.GetComponent<UIPopupYesOrNo>().Set(title, content, yes, no));
    }

    public IObservable<Unit> ShowPopupNotice(string title, string content, string ok = null)
    {
        return Observable.Create<GameObject>(observer =>
        {
            GameObject ui = Push(UISystemType.PopupNotice, false);
            if(ui != null)
            {
                observer.OnNext(ui);
                observer.OnCompleted();
            }
            else
            {
                observer.OnError(new NullReferenceException());
            }

            return Disposable.Empty;
        })
        .ContinueWith(ui => ui.GetComponent<UIPopupNotice>().Set(title, content, ok));
    }

    UIInfo FindUI(Enum id)
    {
        instances.TryGetValue(id, out UIInfo ui);
        return ui;
    }

    // 안드로이드 백버튼을 누를 때 현재 UI를 닫거나 게임 종료 UI를 보여준다.
    void CheckAndroidBackButton()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if(waitingUICount > 0)
            {
                return;
            }

            if(stack.Count > 0)
            {
                Back();
            }
            else
            {
                TryExitByAndroidBackButton();
            }
        }
    }

    void TryExitByAndroidBackButton()
    {
        ShowPopupYesOrNo(
                "게임 종료",
                "게임을 종료하시겠습니까?",
                "예",
                "아니오"
            )
            .Subscribe(yes =>
            {
                if(yes)
                {
                    Application.Quit();
                }
            })
            .AddTo(this);
    }
}
