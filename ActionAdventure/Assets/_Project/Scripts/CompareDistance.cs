using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareDistance : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float radius = 5f;

    void Start()
    {
        EntityTracker_Enemy.Instance.EnemyList.Add(this.transform);
    }

    void Update()
    {
        if ((transform.position - target.position).sqrMagnitude < radius * radius)
        {
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
        }
    }
}
