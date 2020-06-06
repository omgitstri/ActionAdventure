using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MonobehaviourCallbacks : MonoBehaviour
{
    [SerializeField] UnityEvent _Start = null;
    [SerializeField] UnityEvent _Update = null;

    void Start()
    {
        _Start.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        _Update.Invoke();
    }
}
