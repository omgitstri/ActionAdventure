using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    public Vector3 initPos = Vector3.zero;
    public Vector3 initPosWorld = Vector3.zero;
    public Vector3 random = Vector3.zero;
    public Vector3 random2 = Vector3.zero;

    [ContextMenu(nameof(Start))]
    public void Start()
    {
        initPos = transform.localPosition;
        initPosWorld = transform.position;
        random = Random.insideUnitSphere * 5;
        random2 = Random.insideUnitSphere * 0.5f;

    }

}
