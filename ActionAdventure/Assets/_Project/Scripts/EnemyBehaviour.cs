using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class EnemyBehaviour : MonoBehaviour
{
    [Range(0,1)]
    [SerializeField] private float a = 0f;
    [SerializeField] private bool _dodge = false;
    [SerializeField] private bool _damage = false;

    [SerializeField] private Transform hitDirection = null;

    void Update()
    {
        if (_dodge)
        {
            foreach (var item in transform.GetComponentsInChildren<EnemyVisual>())
            {
                item.transform.localPosition = Vector3.LerpUnclamped(item.initPos, item.initPos + item.random, a);
                item.transform.localScale = Vector3.Lerp(Vector3.one * 0.125f, Vector3.zero, a - 0.5f);
            }
        }

        if (_damage)
        {
            foreach (var item in transform.GetComponentsInChildren<EnemyVisual>())
            {
                item.transform.position = Vector3.Lerp(item.initPosWorld, (item.initPosWorld - hitDirection.position).normalized * 3 + item.random2, a);
                item.transform.localScale = Vector3.Lerp(Vector3.one * 0.125f, Vector3.zero, a - 0.2f);
            }
        }
    }

    private void OnMouseDown()
    {
        
    }

    public void ResetAll()
    {
        foreach (var item in transform.GetComponentsInChildren<SmearEffect>())
        {
            item.ResetAll();
        }
    }
    public void Enable()
    {
        foreach (var item in transform.GetComponentsInChildren<EnemyVisual>())
        {
            item.Start();
        }

        foreach (var item in transform.GetComponentsInChildren<SmearEffect>(true))
        {
            item.enabled = true;
        }
    }
}
