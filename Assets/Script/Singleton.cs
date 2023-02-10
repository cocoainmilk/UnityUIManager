using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR
using System.IO;

public interface ISingleton
{
    void OnDestroySingleton();
}

public static class SingletonManager
{
    static List<ISingleton> list = new();

    public static void Add(ISingleton singleton)
    {
        list.Add(singleton);
    }

    public static void RemoveAndDestroy(ISingleton singleton)
    {
        list.Remove(singleton);
        Destroy(singleton);
    }

    public static void Clear()
    {
        list.ForEach(singleton => Destroy(singleton));
        list.Clear();
    }

    public static ISingleton[] ToArray()
    {
        return list.ToArray();
    }

    static void Destroy(ISingleton singleton)
    {
        if(singleton != null)
        {
            singleton.OnDestroySingleton();
            try
            {
                if(singleton is MonoBehaviour monoBehaviour && monoBehaviour.gameObject != null)
                {
                    Object.DestroyImmediate(monoBehaviour.gameObject);
                }
            }
            catch(System.Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }
}

public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
{
    static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new T();
                SingletonManager.Add(_instance);
            }

            return _instance;
        }
    }

    public void OnDestroySingleton()
    {
        _instance = null;
    }
}

public abstract class SingletonMonoBehaviourStatic<T> : MonoBehaviour, ISingleton where T : SingletonMonoBehaviourStatic<T>
{
    static T _instance;
    public static bool IsInstantiated => _instance != null;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if(_instance == null)
                {
                    GameObject o = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(o);
                    o.AddComponent(typeof(T));
                }
            }

            return _instance;
        }
    }


    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            SingletonManager.Add(_instance);
        }
    }

    public void OnDestroySingleton()
    {
        _instance = null;
    }
}

public abstract class SingletonMonoBehaviourOnlyLevel<T> : MonoBehaviour where T : SingletonMonoBehaviourOnlyLevel<T>
{
    static T _instance;
    public static bool IsInstantiated => _instance != null;
    public static T Instance
    {
        get
        {
            if(!_instance)
            {
                _instance = FindObjectOfType<T>();
                if(!_instance)
                {
                    GameObject o = new GameObject(typeof(T).Name);
                    _instance = o.AddComponent(typeof(T)) as T;
                }
            }

            return _instance;
        }
    }
}

public abstract class SingletonMonoBehaviourUnmanaged<T> : MonoBehaviour where T : SingletonMonoBehaviourUnmanaged<T>
{
    static T _instance;
    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if(_instance == null)
                {
                    GameObject o = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(o);
                    o.AddComponent(typeof(T));
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}

