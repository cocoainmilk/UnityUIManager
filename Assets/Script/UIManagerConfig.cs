using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UIManagerConfig", menuName = "ScriptableObjects/UIManagerConfig")]
public class UIManagerConfig : ScriptableObject
{
    [SerializeField] UIManagerPrefab[] systemUIPrefabCommon = null;
    [SerializeField] GameObject[] systemUIPrefab = null;
    [SerializeField] UIManagerPrefab[] userUIPrefabCommon = null;
    [SerializeField] GameObject[] userUIPrefab = null;
    [SerializeField] bool useStartingUserUI = true;

    public IEnumerable<GameObject> SystemUIPrefab => systemUIPrefab.Concat(systemUIPrefabCommon.SelectMany(value => value.Prefab));
    public IEnumerable<GameObject> UserUIPrefab => userUIPrefab.Concat(userUIPrefabCommon.SelectMany(value => value.Prefab));

    public bool UseStartingUserUI => useStartingUserUI;
}
