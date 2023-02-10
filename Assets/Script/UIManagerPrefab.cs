using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManagerPrefab : ScriptableObject
{
    [SerializeField] GameObject[] prefab = null;
    public IEnumerable<GameObject> Prefab => prefab;
}
