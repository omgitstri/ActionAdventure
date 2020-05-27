using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MonobehaviourCallbacks : MonoBehaviour
{
    [SerializeField] UnityEvent _Start = null;

    void Start()
    {
        _Start.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
