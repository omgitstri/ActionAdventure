using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] UnityEvent _equipBow = null;
    [SerializeField] UnityEvent _disarmBow = null;

    public void EquipBow()
    {
        _equipBow.Invoke();
    }

    public void DisarmBow()
    {
        _disarmBow.Invoke();
    }
}
