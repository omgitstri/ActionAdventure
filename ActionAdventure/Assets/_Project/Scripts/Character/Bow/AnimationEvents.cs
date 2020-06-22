using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] UnityEvent _equipBow = null;
    [SerializeField] UnityEvent _disarmBow = null;
    [SerializeField] UnityEvent _drawBow = null;
    [SerializeField] UnityEvent _releaseBow = null;
    [SerializeField] Transform _bow = null;
    [SerializeField] GameObject _arrow = null;

    public void EquipBow()
    {
        _equipBow.Invoke();
    }

    public void DisarmBow()
    {
        _disarmBow.Invoke();
        _releaseBow.Invoke();

    }

    public void Shoot()
    {
        var go = Instantiate(_arrow);
        go.transform.SetParent(_bow);

        go.transform.localScale = Vector3.one;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<Rigidbody>().AddRelativeForce(go.transform.up * 50, ForceMode.Impulse);

        go.transform.SetParent(null);

        _bow.gameObject.SetActive(false);
    }

    public void SetArrow()
    {
        _bow.gameObject.SetActive(true);
    }

    public void DrawBow()
    {
        _drawBow.Invoke();
    }

    public void ReleaseBow()
    {
        _releaseBow.Invoke();
    }
}
